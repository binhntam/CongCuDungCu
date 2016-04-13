using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Linq.Mapping;
using System.Reflection;


namespace CongCuDungCu.Models
{
    public partial class CCDCDataContext
    {
        [Function(Name = "GetDate", IsComposable = true)]
        public DateTime GetSystemDate()
        {
            var med = MethodBase.GetCurrentMethod() as MethodInfo;
            return (DateTime)ExecuteMethodCall(this, med, new object[] { }).ReturnValue;
        }
    }
    public static class CCDCDataProvider
    {
        private const string CCDCDataContextKey = "CCDCDataContext";

        public static CCDCDataContext DB
        {
            get
            {
                if (HttpContext.Current.Items[CCDCDataContextKey] == null)
                {
                    HttpContext.Current.Items[CCDCDataContextKey] = new CCDCDataContext();
                }
                return (CCDCDataContext)HttpContext.Current.Items[CCDCDataContextKey];
            }
        }

        private static string getMaPhatSinh(string maLoaiPhieu)
        {
            var newID = string.Empty;
            var query11 = (from result in DB.Chungtus
                           select result).OrderByDescending(p => p.STTCT).First();
            var iQuery11 = (int)query11.STTCT + 1;
            newID = maLoaiPhieu + iQuery11.ToString();
            return newID;
        }
        private static int getFirstTTCT()
        {
            var newID = 0;
            var query11 = (from result in DB.Chungtus
                           select result).OrderByDescending(p => p.STTCT).First();
            newID = (int)query11.STTCT  + 1;
            return newID;
        }
        private static int getFirstTTCTCT()
        {
            var newID = 0;
            var query11 = (from result in DB.ChitietCTs
                           select result).OrderByDescending(p => p.STTCTCT).First();
            newID = (int)query11.STTCTCT + 1;
            return newID;
        }
        public static string getTenDvi_BySTTCT(int sttct)
        {
            return (string)(from donvi in DB.Chungtus
                            where donvi.STTCT == sttct
                            select donvi.Tu).FirstOrDefault();
        }
        public static string getTenDvi_ByID(string madonvi)
        {
            return (string)(from donvi in DB.DMDonvis
                            where donvi.Madonvi == madonvi
                            select donvi.Tendonvi).FirstOrDefault();
        }
        public static string GetMadvql_ByUserName(string userName)
        {
            return (string)(from donvi in DB.DMDonvis
                            join user in DB.Users on donvi.Madonvi equals user.MaDonVi
                            where user.UserName == userName
                            select donvi.Madvql).FirstOrDefault();
        }
        public static string DMDonVis_byMaDonVi(string Madvql, string tenDonVi)
        {                   
          
            return (from result in DB.DMDonvis
                    where result.Madvql == Madvql && result.Tendonvi.Contains(tenDonVi)
                    select result.Madonvi).FirstOrDefault();
        }
        public static string getDen_fromCHUNGTU(int sttct)
        {
            return (from result in DB.Chungtus
                    where result.STTCT == sttct
                   select result.Den).FirstOrDefault();
        }
        public static string getMaDonVi_fromChiTietCT(string maDonviQL, int sttct)
        {
            var re1 = getDen_fromCHUNGTU(sttct);
            var re2 = DMDonVis_byMaDonVi(maDonviQL, getDen_fromCHUNGTU(sttct));
            return DMDonVis_byMaDonVi(maDonviQL, getDen_fromCHUNGTU(sttct));
        }

        public static IEnumerable GetFullName_ByUserName(string userName)
        {
            return (from result in DB.Users
                    where result.UserName == userName
                    select new
                    {
                        Nguoithuchien = result.FullName,
                    }).ToList();
        }

        public static IEnumerable GetCCDC_ByMaCCDC(string _MaCCDC)
        {
            return (from result in DB.DMCCDCs
                    where result.MaCCDC == _MaCCDC
                    select result.TenCCDC).SingleOrDefault();
        }
        public static IEnumerable GetCCDC_ByMaCCDCofDVi(string maDvql, string maDvi)
        {
           
            var SumXuat = (from p in DB.ChitietCTs
                          where p.Madvql == maDvql && p.Madonvi == maDvi && p.SoCT.StartsWith("X0")                       
                          group p by p.MaCCDC
                              into g
                              select new
                              {
                                  MaCCDC = g.Key,  
                                  TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),                               
                              });
            return SumXuat;
        }
        public static IList<EditableCCDC> GetEditableCCDCs3(string maDVQL, string maDonVi)
        {
            var tu = (from p in DB.DMDonvis
                      where p.Madonvi == maDonVi
                      select p.Tendonvi).FirstOrDefault();
            var soCTXuats = (from ct in DB.Chungtus
                             where ct.Den.Contains(tu) && ct.Maloaiphieu == "X00" && ct.Madvql == maDVQL
                             group ct by ct.SoCT
                                 into g
                                 select new
                                 {
                                     SoCT = g.Key,
                                 });
            var listMaCCDCXuats = (from ctX in soCTXuats
                                   join ctietTH in DB.ChitietCTs on ctX.SoCT equals ctietTH.SoCT
                                   select new
                                   {                                   
                                       MaCCDC = ctietTH.MaCCDC,
                                       TenCCDC = ctietTH.TenCCDC,
                                       SoLuong = ctietTH.Soluong,                                     
                                   }).ToList();
            var SumXuats = from p in listMaCCDCXuats
                          group p by p.MaCCDC
                              into g
                              select new
                              {
                                  MaCCDC = g.Key,
                                  SoLuongXuat = g.Sum(k => k.SoLuong),                                
                                  TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),                             
                              };
            //var SumXuats = (from p in DB.ChitietCTs
            //              where p.Madvql == maDVQL && p.Madonvi == maDonVi && p.SoCT.StartsWith("X00")                       
            //              group p by p.MaCCDC
            //                  into g
            //                  select new
            //                  {
            //                      MaCCDC = g.Key,  
            //                      TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
            //                      SoLuongXuat = g.Sum(k => k.Soluong),
            //                  });
            var soCTThuHois = (from ct in DB.Chungtus
                               where ct.Tu.Contains(tu) && ct.Maloaiphieu == "N01" && ct.Madvql == maDVQL
                               group ct by ct.SoCT
                                   into g
                                   select new
                                   {
                                       SoCT = g.Key,
                                   });
            var listMaCCDCThuHois = (from ctTH in soCTThuHois
                                     join ctietTH in DB.ChitietCTs on ctTH.SoCT equals ctietTH.SoCT
                                     select new
                                     {                                        
                                         MaCCDC = ctietTH.MaCCDC,
                                         TenCCDC = ctietTH.TenCCDC,
                                         SoLuong = ctietTH.Soluong,                                        
                                     }).ToList();
            var SumThuHois = from p in listMaCCDCThuHois
                            group p by p.MaCCDC
                                into g
                                select new
                                {
                                    MaCCDC = g.Key,
                                    SoLuongThuHoi = g.Sum(k => k.SoLuong),                                  
                                    TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),                               
                                };
            //var SumThuHois = from p in DB.ChitietCTs
            //                 where p.Madvql == maDVQL && p.Madonvi == maDonVi && p.SoCT.StartsWith("N01")
            //                 group p by p.MaCCDC
            //                     into g
            //                     select new
            //                     {
            //                         MaCCDC = g.Key,
            //                         TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
            //                         SoLuongThuHoi = g.Sum(k => k.Soluong),
            //                     };
            var result = from p in SumXuats
                         join p2 in SumThuHois on p.MaCCDC equals p2.MaCCDC
                             into banGroup
                         from ban in banGroup.DefaultIfEmpty()
                         select new
                         {
                             MaCCDC = p.MaCCDC,
                             SoLuongXuat = (int)(p.SoLuongXuat),
                             SoLuongThuHoi = (ban != null) ? ban.SoLuongThuHoi : 0,                           
                             TenCCDC = p.TenCCDC,
                            
                         };
            //var result = from p in SumXuats
            //             join p2 in SumThuHois on p.MaCCDC equals p2.MaCCDC
            //                 into banGroup
            //             from ban in banGroup.DefaultIfEmpty()
            //             select new
            //             {
            //                 MaCCDC = p.MaCCDC,
            //                 SoLuongXuat = (int)(p.SoLuongXuat),
            //                 SoLuongThuHoi = (ban != null) ? ban.SoLuongThuHoi : 0,
            //                 TenCCDC = p.TenCCDC,
            //             };
            var after = from p in result
                        group p by p.MaCCDC
                            into g
                            select new
                            {
                                MaCCDC = g.Key,
                                SoLuong = g.Sum(k => k.SoLuongXuat - k.SoLuongThuHoi),                              
                                TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),                               
                            };
            //var after = (from p in result
            //             group p by p.MaCCDC
            //                 into g
            //                 select new EditableCCDC
            //                 {
            //                     MaCCDC = g.Key,
            //                     SoLuong = (int)g.Sum(k => k.SoLuongXuat - k.SoLuongThuHoi),
            //                     TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
            //                 }).OrderBy(p => p.TenCCDC).ToList();
            var afterAgain = (from p in after
                              where p.SoLuong > 0
                              select new EditableCCDC
                              {
                                  MaCCDC = p.MaCCDC,
                                  SoLuong = (int)p.SoLuong,                                
                                  TenCCDC = p.TenCCDC,
                              }).OrderBy(p => p.TenCCDC).ToList(); ;
            return afterAgain;
        }
        public static IList<EditableCCDC> GetEditableCCDCs4(string maDVQL, string maDonVi)
        {
           
            var SumMuaMois = from p in DB.ChitietCTs
                             where p.Madvql == maDVQL && p.Madonvi == maDonVi && p.SoCT.StartsWith("N00")
                            group p by p.MaCCDC
                                into g
                                select new
                                {
                                    MaCCDC = g.Key,
                                    TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                                    SoLuongMoi = g.Sum(k => k.Soluong),
                                };
            var SumThuHois = from p in DB.ChitietCTs
                             where p.Madvql == maDVQL && p.Madonvi == maDonVi && p.SoCT.StartsWith("N01")
                             group p by p.MaCCDC
                                 into g
                                 select new
                                 {
                                     MaCCDC = g.Key,
                                     TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                                     SoLuongThuHoi = g.Sum(k => k.Soluong),
                                 };
            var result = from p in SumMuaMois
                         join p2 in SumThuHois on p.MaCCDC equals p2.MaCCDC
                             into banGroup
                         from ban in banGroup.DefaultIfEmpty()
                         select new
                         {
                             MaCCDC = p.MaCCDC,
                             SoLuongMoi = (int)(p.SoLuongMoi),
                             SoLuongThuHoi = (ban != null) ? ban.SoLuongThuHoi : 0,                           
                             TenCCDC = p.TenCCDC,                           
                         };
            var after = (from p in result
                        group p by p.MaCCDC
                            into g
                            select new EditableCCDC
                            {
                                MaCCDC = g.Key,
                                SoLuong = (int)g.Sum(k => k.SoLuongMoi + k.SoLuongThuHoi),                              
                                TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                            }).OrderBy(p => p.TenCCDC).ToList();

            return after;
        }
        public static int GetEditableCCDCs5(string maCCDC,string maDVQL, string maDonVi)
        {

            var Sums = from p in DB.ChitietCTs
                             where p.Madvql == maDVQL && p.Madonvi == maDonVi && p.SoCT.StartsWith("N") && p.MaCCDC==maCCDC
                             group p by p.MaCCDC
                                 into g
                                 select new 
                                 {
                                     MaCCDC = g.Key,
                                     TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                                     SoLuong= g.Sum(k => k.Soluong),
                                 };
            //var list = (from result in Sums
            //            select new EditableCCDC
            //            {
            //                //  STTDMCCDC = result.STTDMCCDC,
            //                MaCCDC = result.MaCCDC,
            //                TenCCDC = result.TenCCDC,
            //                SoLuong = (int)result.SoLuong,

            //            }).OrderBy(p => p.TenCCDC).ToList();
            int re = Sums.Select(p => p.SoLuong.Value).FirstOrDefault();
            return re;
        }
        public static string GetMadvt_fromDMCCDC(string _MaCCDC)
        {
            var re = (from result in DB.DMCCDCs
                      where result.MaCCDC == _MaCCDC
                      select result.Madvt).SingleOrDefault();
            return (from result in DB.DMCCDCs
                    where result.MaCCDC == _MaCCDC
                    select result.Madvt).SingleOrDefault();
        }
        #region UPDATE
        public static void Save()
        {
            DB.SubmitChanges();
        }
        public static void AddNew(DMCCDC ct)
        {
            DB.DMCCDCs.InsertOnSubmit(ct);
        }
        public static void AddNew(Chungtu ct)
        {
            DB.Chungtus.InsertOnSubmit(ct);
        }
        public static void AddNew(ChitietCT ct)
        {
            DB.ChitietCTs.InsertOnSubmit(ct);
        }
        public static void AddNew(DMDonvi ct)
        {
            DB.DMDonvis.InsertOnSubmit(ct);
        }
        public static void Delete(ChitietCT ct)
        {
            DB.ChitietCTs.DeleteOnSubmit(ct);
        }
        public static void Delete(DMDonvi ct)
        {
            DB.DMDonvis.DeleteOnSubmit(ct);
        }
        public static void Delete(Chungtu ct)
        {
            DB.Chungtus.DeleteOnSubmit(ct);
        }
        #endregion

        #region DANH SACH

        public static IEnumerable DMNoiSXs()
        {
            return (from result in DB.DMNoiSXes
                    select new
                    {
                        STTNSX = result.STTNSX,
                        Manoisx = result.Manoisx,
                        Tennoisx = result.Tennoisx,
                    }).OrderBy(p => p.STTNSX).ToList();
        }


        public static IEnumerable DMDonViQLs()
        {
            return (from result in DB.DMDVQLs
                    select new
                    {
                        STTDVQL = result.STTDVQL,
                        Madvql = result.Madvql,
                        Tendvql = result.Tendvql,
                        Diachi = result.Diachi,
                        Email = result.Email,
                        Fax = result.Fax,
                        Dienthoai = result.Dienthoai,
                        Taikhoan = result.Taikhoan,
                        MST = result.MST,
                    }).OrderBy(p => p.STTDVQL).ToList();
        }


        public static IEnumerable DMNhaCungCaps()
        {
            return (from result in DB.DMNhaccs
                    select new
                    {
                        STTDMNCC = result.STTDMNCC,
                        Manhacc = result.Manhacc,
                        Tennhacc = result.Tennhacc,
                        Diachi = result.Diachi,
                        Email = result.Email,
                        Fax = result.Fax,
                        Dienthoai = result.Dienthoai,
                        Taikhoan = result.Taikhoan,
                        MST = result.MST,
                    }).OrderBy(p => p.STTDMNCC).ToList();
        }

        #region BieuMau9KK
        public static IList<EditableDonVi> listDonVis_byMadvql(string maDVQL)
        {
            return (from result in DB.DMDonvis
                    where result.Madvql == maDVQL
                    select new EditableDonVi
                    {
                        STTDMDV = result.STTDMDV,
                        Madonvi = result.Madonvi,
                        Tendonvi = result.Tendonvi,
                        Madvql = result.Madvql,
                    }).ToList();
        }
        public static string getMadvql_MaDonVi(string maDonvi)
        {
            return (from result in DB.DMDonvis
                    where result.Madonvi == maDonvi
                    select result.Madvql).SingleOrDefault();
                  
        }
        #endregion

        #region DMDonVis
        public static IEnumerable DMDonVis()
        {
            return (from result in DB.DMDonvis

                    select new
                    {
                        STTDMDV = result.STTDMDV,
                        Madonvi = result.Madonvi,
                        Tendonvi = result.Tendonvi,
                        Dienthoai = result.Dienthoai,
                        Madvql = result.Madvql,
                    }).OrderBy(p => p.Tendonvi).ToList();
        }
        public static IList<EditableDonVi> listDonVis(string Madvql)
        {
            return (from result in DB.DMDonvis
                    where result.Madvql == Madvql
                    select new EditableDonVi
                    {
                        STTDMDV = result.STTDMDV,
                        Madonvi = result.Madonvi,
                        Tendonvi = result.Tendonvi,

                    }).OrderBy(p => p.STTDMDV).ToList();
        }

        public static IList<EditableDonVi> GetEditableDMDonVis(string maDVQL)
        {

            var list = (from result in DB.DMDonvis
                        where result.Madvql == maDVQL
                        select new EditableDonVi
                        {
                            STTDMDV = result.STTDMDV,
                            Madonvi = result.Madonvi,
                            Tendonvi = result.Tendonvi,
                            Madvql = result.Madvql,

                        }).OrderByDescending(p => p.STTDMDV).ToList();

            return list;
        }


        public static void UpdateDonVi(EditableDonVi ccdc)
        {
            var editableCTiet = GetEditableDMDonVis(ccdc.Madvql).Where(p => p.STTDMDV == ccdc.STTDMDV).SingleOrDefault();
            if (editableCTiet == null)
            {
                return;
            }

            editableCTiet.Madonvi = ccdc.Madonvi;
            editableCTiet.Tendonvi = ccdc.Tendonvi;
            editableCTiet.Madvql = ccdc.Madvql;

            var edit = DB.DMDonvis.SingleOrDefault(d => d.STTDMDV == ccdc.STTDMDV);


            edit.Madonvi = ccdc.Madonvi;
            edit.Tendonvi = ccdc.Tendonvi;
            edit.Madvql = ccdc.Madvql;
            Save();
        }
        public static void InsertDonVi(EditableDonVi ccdc)
        {
            var editableCCDC = new EditableDonVi();

            editableCCDC.Madonvi = ccdc.Madonvi;
            editableCCDC.Tendonvi = ccdc.Tendonvi;
            editableCCDC.Madvql = ccdc.Madvql;

            GetEditableDMDonVis(ccdc.Madvql).Add(editableCCDC);

            var insert = new DMDonvi();
            insert.Madonvi = ccdc.Madonvi;
            insert.Tendonvi = ccdc.Tendonvi;
            insert.Madvql = ccdc.Madvql;
            AddNew(insert);
            Save();
        }
        public static void DeleteDonVi(int STTDMDV, string maDvql)
        {
            var editableCTiet = GetEditableDMDonVis(maDvql).Where(p => p.STTDMDV == STTDMDV).SingleOrDefault();
            if (editableCTiet != null)
            {
                GetEditableDMDonVis(maDvql).Remove(editableCTiet);
            }
            var tram = DB.DMDonvis.SingleOrDefault(d => d.STTDMDV == STTDMDV);
            Delete(tram);
            Save();
        }
        public static IEnumerable DMDonViByUsers(string userNam)
        {
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userNam);
            return (from result in DB.DMDonvis
                    where result.Madvql == Madvql
                    select new
                    {
                        STTDMDV = result.STTDMDV,
                        Madonvi = result.Madonvi,
                        Tendonvi = result.Tendonvi,
                        Dienthoai = result.Dienthoai,
                        Madvql = result.Madvql,
                    }).OrderBy(p => p.Tendonvi).ToList();
        }
        public static IEnumerable DMDonViByUsers2(string userNam)
        {
            var Madvql = CCDCDataProvider.GetMadvql_ByUserName(userNam);
            return (from result in DB.DMDonvis
                    where result.Madvql == Madvql && result.Madonvi.StartsWith("KHO-")
                    select new
                    {
                        STTDMDV = result.STTDMDV,
                        Madonvi = result.Madonvi,
                        Tendonvi = result.Tendonvi,
                        Dienthoai = result.Dienthoai,
                        Madvql = result.Madvql,
                    }).OrderBy(p => p.STTDMDV).ToList();
        }
        #endregion

        public static IEnumerable DMDonVis_byMadvql(string Madvql)
        {
            return (from result in DB.DMDonvis
                    where result.Madvql == Madvql
                    select new
                    {
                        Den = result.Tendonvi,
                    }).ToList();
        }

        public static IEnumerable DMDonVis_byMadvql_TU(string Madvql)
        {
            return (from result in DB.DMDonvis
                    where result.Madvql == Madvql
                    select new
                    {
                        Tu = result.Tendonvi,
                    }).ToList();
        }

        public static IEnumerable DMLoaiphieus()
        {
            return (from result in DB.DMLoaiphieus
                    select new
                    {
                        STTDMLP = result.STTDMLP,
                        Maloaiphieu = result.Maloaiphieu,
                        Tenloaiphieu = result.Tenloaiphieu,
                    }).OrderBy(p => p.STTDMLP).ToList();
        }

        public static IEnumerable DMKhos()
        {
            return (from result in DB.DMKhos
                    select new
                    {
                        STTDMKHO = result.STTDMKHO,
                        Makho = result.Makho,
                        Tenkho = result.Tenkho,
                        Madvql = result.Madvql,
                    }).OrderBy(p => p.STTDMKHO).ToList();
        }
        public static IEnumerable DMKhos_byMadvql(string Madvql)
        {
            return (from result in DB.DMKhos
                    where result.Madvql == Madvql
                    select new
                    {
                        Tu = result.Tenkho,
                    }).ToList();
        }
        public static IEnumerable DMKhos_byMadvql_DEN(string Madvql)
        {
            return (from result in DB.DMKhos
                    where result.Madvql == Madvql
                    select new
                    {
                        Den = result.Tenkho,
                    }).ToList();
        }


        public static IEnumerable DMNguoidungs()
        {
            return (from result in DB.DMNguoidungs
                    select new
                    {
                        STT = result.STT,
                        Manguoidung = result.Manguoidung,
                        Tennguoidung = result.Tennguoidung,

                    }).OrderBy(p => p.STT).ToList();
        }

        public static IEnumerable DMNhomnguoidungs()
        {
            return (from result in DB.DMNhomnguoidungs
                    select new
                    {
                        STT = result.STT,
                        Manhom = result.Manhom,
                        Tennhom = result.Tennhom,
                        Ghichu = result.Ghichu,
                    }).OrderBy(p => p.STT).ToList();
        }

        public static IEnumerable DMDvts()
        {
            return (from result in DB.DMDvts
                    select new
                    {
                        STT = result.STT,
                        Madvt = result.Madvt,
                        Tendvt = result.Tendvt,
                    }).OrderBy(p => p.STT).ToList();
        }

        public static IEnumerable DMbaocaos()
        {
            return (from result in DB.DMbaocaos
                    select new
                    {
                        STTBC = result.STTBC,
                        Madvt = result.Madvql,
                        Ctycon = result.Ctycon,
                        Ctyme = result.Ctyme,
                        CD1D1 = result.CD1D1,
                        CD1D2 = result.CD1D2,
                        CD2D1 = result.CD2D1,
                        CD2D2 = result.CD2D2,
                        CD3D1 = result.CD3D1,
                        CD3D2 = result.CD3D2,
                        TenCD1 = result.TenCD1,
                        TenCD2 = result.TenCD2,
                        TenCD3 = result.TenCD3,
                    }).OrderBy(p => p.STTBC).ToList();
        }


        public static IEnumerable DMTinhtrangs()
        {
            return (from result in DB.DMTinhtrangs
                    select new
                    {
                        STTDMTT = result.STTDMTT,
                        Matinhtrang = result.Matinhtrang,
                        Tentinhtrang = result.Tentinhtrang,

                    }).OrderBy(p => p.STTDMTT).ToList();
        }


        public static IEnumerable DMLoaiccdcs()
        {
            return (from result in DB.DMLoaiccdcs
                    select new
                    {
                        STT = result.STT,
                        Maloaiccdc = result.Maloaiccdc,
                        Tenloaiccdc = result.Tenloaiccdc,
                    }).OrderBy(p => p.STT).ToList();
        }


        public static IEnumerable DMLoaiTs()
        {
            return (from result in DB.DMLoaiTs
                    select new
                    {
                        STT = result.STT,
                        Maloaits = result.Maloaits,
                        Tenloaits = result.Tenloaits,
                    }).OrderBy(p => p.STT).ToList();
        }


        public static IEnumerable DMNguonvons()
        {
            return (from result in DB.DMNguonvons
                    select new
                    {
                        STTDMNGUONVON = result.STTDMNGUONVON,
                        Manguonvon = result.Manguonvon,
                        Tennguonvon = result.Tennguonvon,
                    }).OrderBy(p => p.STTDMNGUONVON).ToList();
        }
        #endregion

      
        public static IList<EditableCCDC> CCDCs(string maCCDC, string maDvql)
        {
            return (from result in listCCDCs(maDvql)
                    where result.MaCCDC == maCCDC
                    select result).OrderBy(p => p.STTDMCCDC).ToList();
        }
        public static IList<EditableCCDC> listCCDCs(string maDvql)
        {
            return (from result in DB.DMCCDCs
                    where result.Madvql == maDvql
                    select new EditableCCDC
                    {
                        STTDMCCDC = result.STTDMCCDC,
                        MaCCDC = result.MaCCDC,
                        TenCCDC = result.TenCCDC,
                        Madvt = result.Madvt,
                        Manhacc = result.Manhacc,
                        Manoisx = result.Manoisx,
                        Maloaits = result.Maloaits,
                        Maloaiccdc = result.Maloaiccdc,
                        Manguonvon = result.Manguonvon,
                        Madvql = result.Madvql,
                        Ghichu = result.Ghichu,
                    }).OrderByDescending(p => p.STTDMCCDC).ToList();
        }

        public static IList<EditableCCDC> GetEditableCCDCs()
        {
            const string SessionKey = "DXDemoEmployees";
            var list = HttpContext.Current.Session[SessionKey] as IList<EditableCCDC>;


            HttpContext.Current.Session[SessionKey] = list = (from result in DB.DMCCDCs
                                                                  select new EditableCCDC
                                                                  {
                                                                      STTDMCCDC = result.STTDMCCDC,
                                                                      MaCCDC = result.MaCCDC,
                                                                      TenCCDC = result.TenCCDC,
                                                                      Chitiet = result.Chitiet,

                                                                  }).OrderBy(p => p.STTDMCCDC).ToList();

            return list;
        }
        public static IList<EditableCCDC> GetEditableCCDC2s(string maDVQL)
        {
           
            var list = (from result in DB.DMCCDCs
                        where result.Madvql == maDVQL
                            select new EditableCCDC
                            {
                                STTDMCCDC = result.STTDMCCDC,
                                MaCCDC = result.MaCCDC,
                                TenCCDC = result.TenCCDC,
                                Chitiet = result.Chitiet,

                            }).OrderBy(p => p.TenCCDC).ToList();

            return list;
        }
      
        public static IList<EditableCCDC> GetEditableDetailCCDC(string _MaCCDC)
        {
            var CCDCList1 = (from result in DB.DMCCDCs
                             where result.MaCCDC == _MaCCDC
                             orderby result.STTDMCCDC descending
                             select new EditableCCDC
                             {
                                 STTDMCCDC = result.STTDMCCDC,
                                 MaCCDC = result.MaCCDC,
                                 TenCCDC = result.TenCCDC,
                                 Madvt = result.Madvt,
                                 Manhacc = result.Manhacc,
                                 Manoisx = result.Manoisx,
                                 Maloaits = result.Maloaits,
                                 Maloaiccdc = result.Maloaiccdc,
                                 Manguonvon = result.Manguonvon,
                                 Madvql = result.Madvql,

                                 Ghichu = result.Ghichu,
                             }).OrderBy(p => p.STTDMCCDC).ToList();

            return CCDCList1;
        }

        public static void UpdateCCDC(EditableCCDC ccdc)
        {
            var editableCTiet = CCDCs(ccdc.MaCCDC, ccdc.Madvql).Where(p => p.STTDMCCDC == ccdc.STTDMCCDC).SingleOrDefault();
            if (editableCTiet == null)
            {
                return;
            }

            editableCTiet.TenCCDC = ccdc.TenCCDC;
            editableCTiet.Madvt = ccdc.Madvt;
            editableCTiet.Manhacc = ccdc.Manhacc;
            editableCTiet.Manoisx = ccdc.Manoisx;
            editableCTiet.Maloaits = ccdc.Maloaits;
            editableCTiet.Maloaiccdc = ccdc.Maloaiccdc;
            editableCTiet.Manguonvon = ccdc.Manguonvon;
            editableCTiet.Madvql = ccdc.Madvql;
            editableCTiet.Ghichu = ccdc.Ghichu;
            var edit = DB.DMCCDCs.SingleOrDefault(d => d.STTDMCCDC == ccdc.STTDMCCDC);


            edit.TenCCDC = ccdc.TenCCDC;
            edit.Madvt = ccdc.Madvt;
            edit.Manhacc = ccdc.Manhacc;
            edit.Manoisx = ccdc.Manoisx;
            edit.Maloaits = ccdc.Maloaits;
            edit.Maloaiccdc = ccdc.Maloaiccdc;
            edit.Manguonvon = ccdc.Manguonvon;
            edit.Madvql = ccdc.Madvql;
            edit.Ghichu = ccdc.Ghichu;
            Save();
        }
        public static void InsertCCDC(EditableCCDC ccdc)
        {
            var editableCCDC = new EditableCCDC();

            editableCCDC.MaCCDC = ccdc.MaCCDC;
            editableCCDC.TenCCDC = ccdc.TenCCDC;
            editableCCDC.Madvt = ccdc.Madvt;
            editableCCDC.Manhacc = ccdc.Manhacc;
            editableCCDC.Manoisx = ccdc.Manoisx;
            editableCCDC.Maloaits = ccdc.Maloaits;
            editableCCDC.Maloaiccdc = ccdc.Maloaiccdc;
            editableCCDC.Manguonvon = ccdc.Manguonvon;
            editableCCDC.Madvql = ccdc.Madvql;
            editableCCDC.Ghichu = ccdc.Ghichu;

            listCCDCs(ccdc.Madvql).Add(editableCCDC);

            var insert = new DMCCDC();
            insert.MaCCDC = ccdc.MaCCDC;
            insert.TenCCDC = ccdc.TenCCDC;
            insert.Madvt = ccdc.Madvt;
            insert.Manhacc = ccdc.Manhacc;
            insert.Manoisx = ccdc.Manoisx;
            insert.Maloaits = ccdc.Maloaits;
            insert.Maloaiccdc = ccdc.Maloaiccdc;
            insert.Manguonvon = ccdc.Manguonvon;
            insert.Madvql = ccdc.Madvql;
            insert.Ghichu = ccdc.Ghichu;
            AddNew(insert);
            Save();
        }


        public static IList<EditableChungTu> listChungtus()
        {
            return (from obj1 in DB.Chungtus
                    select new EditableChungTu
                    {
                        STTCT = obj1.STTCT,
                        Maloaiphieu = obj1.Maloaiphieu,
                        SoCT = obj1.SoCT,
                        Tu = obj1.Tu,
                        Den = obj1.Den,
                        Ngayvietphieu = obj1.Ngayvietphieu,
                        Nguoigiaonhan = obj1.Nguoigiaonhan,
                        Nguoithuchien = obj1.Nguoithuchien,
                        Lydo = obj1.Lydo,
                        Madvql = obj1.Madvql,
                    }).OrderByDescending(p => p.STTCT).ToList();
        }
        public static EditableChungTu GetEditableChungtus(int STTCT)
        {
            return (from result in listChungtus()
                    where result.STTCT == STTCT
                    select result).FirstOrDefault();
        }
        public static string getSoCT(int sttct)
        {
            return (from result in DB.Chungtus
                    where result.STTCT == sttct
                    select result.SoCT).SingleOrDefault();
        }

        public static IEnumerable PhieuNhapKhos(string maDvql, string maLoaiPhieu)
        {
            return (from result in DB.Chungtus
                    where result.Madvql == maDvql && result.Maloaiphieu == maLoaiPhieu
                    select result).OrderByDescending(p => p.STTCT).ToList();
        }


        public static void UpdateChungTuNhap(EditableChungTu chungtu)
        {
            var editableCTiet = listChungtus().Where(p => p.STTCT == chungtu.STTCT).SingleOrDefault();
            if (editableCTiet == null)
            {
                return;
            }
            editableCTiet.Ngayvietphieu = chungtu.Ngayvietphieu;
            editableCTiet.Maloaiphieu = chungtu.Maloaiphieu;
            editableCTiet.Tu = chungtu.Tu;
            editableCTiet.Den = chungtu.Den;
            editableCTiet.Nguoithuchien = chungtu.Nguoithuchien;
            editableCTiet.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            editableCTiet.Lydo = chungtu.Lydo;
            editableCTiet.Madvql = chungtu.Madvql;
            var edit = DB.Chungtus.SingleOrDefault(d => d.STTCT == chungtu.STTCT);

            edit.Ngayvietphieu = chungtu.Ngayvietphieu;
            edit.Maloaiphieu = chungtu.Maloaiphieu;
            edit.Tu = chungtu.Tu;
            edit.Den = chungtu.Den;
            edit.Nguoithuchien = chungtu.Nguoithuchien;
            edit.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            edit.Lydo = chungtu.Lydo;
            Save();
        }
        public static void InsertChungTuNhap(EditableChungTu chungtu)
        {
            var soCT = getMaPhatSinh(chungtu.Maloaiphieu);
        //    HttpContext.Current.Session["newsoCT"] = soCT;
            chungtu.SoCT = soCT;
            var sttct = getFirstTTCT();
            HttpContext.Current.Session["STTCT"] = sttct;        
          //  chungtu.STTCT = sttct;

            var editableCTu = new EditableChungTu();
           // editableCTu.STTCT = chungtu.STTCT;
            editableCTu.SoCT = chungtu.SoCT;
            editableCTu.Ngayvietphieu = chungtu.Ngayvietphieu;
            editableCTu.Maloaiphieu = chungtu.Maloaiphieu;
            editableCTu.Tu = chungtu.Tu;
            editableCTu.Den = chungtu.Den;
            editableCTu.Madvql = chungtu.Madvql;
            editableCTu.Nguoithuchien = chungtu.Nguoithuchien;
            editableCTu.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            editableCTu.Lydo = chungtu.Lydo;

            listChungtus().Add(editableCTu);

            Chungtu insert = new Chungtu();
          //  insert.STTCT = chungtu.STTCT;
            insert.SoCT = chungtu.SoCT;
            insert.Ngayvietphieu = chungtu.Ngayvietphieu;
            insert.Maloaiphieu = chungtu.Maloaiphieu;
            insert.Tu = chungtu.Tu;
            insert.Den = chungtu.Den;
            insert.Nguoithuchien = chungtu.Nguoithuchien;
            insert.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            insert.Lydo = chungtu.Lydo;
            insert.Madvql = chungtu.Madvql;
            AddNew(insert);
            Save();
        }
     
        public static void DeleteChungTu(int STTCT)
        {
            var editableCTiet = listChungtus().Where(p => p.STTCT == STTCT).SingleOrDefault();
            if (editableCTiet != null)
            {
                listChungtus().Remove(editableCTiet);
            }
            var tram = DB.Chungtus.SingleOrDefault(d => d.STTCT == STTCT);
            if (tram != null)
            {
                Delete(tram);
                Save();
            }
            
        }

        public static IEnumerable getChitietCT_BySTTCTCT(int sttCTCT)
        {
            return (from result in DB.ChitietCTs
                    where result.STTCTCT == sttCTCT
                    select result).ToList();
        }
        public static IList<EditableCTChungTu> listCTChungtus(int sttCT)
        {
            var soCT = getSoCT(sttCT);
            return (from obj1 in DB.ChitietCTs
                    where obj1.SoCT == soCT
                    select new EditableCTChungTu
                    {
                        STTCTCT = obj1.STTCTCT,
                        MaCCDC = obj1.MaCCDC,
                        TenCCDC = obj1.TenCCDC,
                        SoCT = obj1.SoCT,
                        Soluong = obj1.Soluong,
                        Dongia = obj1.Dongia,
                        NgaySD = obj1.NgaySD,
                        NgayHHBH = obj1.NgayHHBH,
                        Ghichu = obj1.Ghichu,
                        Madvql = obj1.Madvql,
                        Madvt = obj1.Madvt,
                        Matinhtrang = obj1.Matinhtrang,
                        NguonVon=obj1.NguonVon,
                    }).OrderByDescending(p => p.STTCTCT).ToList();
        }
        public static void UpdateDetail(EditableCTChungTu chitiet)
        {
            getSoCT(chitiet.STTCT );
            var editableCTiet = listCTChungtus(chitiet.STTCT).Where(e => e.STTCTCT == chitiet.STTCTCT).SingleOrDefault();
            if (editableCTiet == null)
            {
                return;
            }
            var maDVT = GetMadvt_fromDMCCDC(chitiet.MaCCDC.Split('_')[0]);
            editableCTiet.MaCCDC = chitiet.MaCCDC;
            editableCTiet.TenCCDC = chitiet.TenCCDC;
            editableCTiet.Soluong = chitiet.Soluong;
            editableCTiet.NgaySD = chitiet.NgaySD;
            editableCTiet.NgayHHBH = chitiet.NgayHHBH;
            editableCTiet.Matinhtrang = chitiet.Matinhtrang;
            editableCTiet.Ghichu = chitiet.Ghichu;
            editableCTiet.Madvql = chitiet.Madvql;
            editableCTiet.Madonvi = chitiet.Madonvi;
            editableCTiet.NguonVon = chitiet.NguonVon;
            /**/
            editableCTiet.Madvt = maDVT;
            /**/
            var edit = DB.ChitietCTs.SingleOrDefault(d => d.STTCTCT == chitiet.STTCTCT);
            edit.MaCCDC = chitiet.MaCCDC;
            edit.TenCCDC = chitiet.TenCCDC;
            edit.Soluong = chitiet.Soluong;
            edit.NgaySD = chitiet.NgaySD;
            edit.NgayHHBH = chitiet.NgayHHBH;
            edit.Matinhtrang = chitiet.Matinhtrang;
            edit.Ghichu = chitiet.Ghichu;
            edit.Madonvi = chitiet.Madonvi;
            edit.NguonVon = chitiet.NguonVon;
            /**/
            edit.Madvt = maDVT;
            /**/
            Save();
        }
        public static void InsertDetail(EditableCTChungTu chitiet)
        {
            var sttctct = getFirstTTCTCT();
            //chitiet.STTCTCT = sttctct;
            
            var maDVT = GetMadvt_fromDMCCDC(chitiet.MaCCDC.Split('_')[0]);
            var editableChiTiet = new EditableCTChungTu();
         //   editableChiTiet.STTCTCT = chitiet.STTCTCT;
            editableChiTiet.SoCT = chitiet.SoCT;
            editableChiTiet.MaCCDC = chitiet.MaCCDC;
            editableChiTiet.TenCCDC = chitiet.TenCCDC;
            editableChiTiet.Soluong = chitiet.Soluong;
            editableChiTiet.NgaySD = chitiet.NgaySD;
            editableChiTiet.NgayHHBH = chitiet.NgayHHBH;
            editableChiTiet.Matinhtrang = chitiet.Matinhtrang;
            editableChiTiet.Ghichu = chitiet.Ghichu;
            editableChiTiet.Madvql = chitiet.Madvql;
            editableChiTiet.Madvt = maDVT;
            editableChiTiet.Madonvi = chitiet.Madonvi;
            editableChiTiet.NguonVon = chitiet.NguonVon;
            listCTChungtus(chitiet.STTCT).Add(editableChiTiet);

            var  insert = new ChitietCT();
          //  insert.STTCTCT = chitiet.STTCTCT;
            insert.SoCT = chitiet.SoCT;
            insert.MaCCDC = chitiet.MaCCDC;
            insert.TenCCDC = chitiet.TenCCDC;
            insert.Soluong = chitiet.Soluong;
            insert.NgaySD = chitiet.NgaySD;
            insert.NgayHHBH = chitiet.NgayHHBH;
            insert.Matinhtrang = chitiet.Matinhtrang;
            insert.Ghichu = chitiet.Ghichu;
            insert.Madvql = chitiet.Madvql;
            insert.Madvt = maDVT;
            insert.Madonvi = chitiet.Madonvi;
            insert.NguonVon = chitiet.NguonVon;
           AddNew(insert);
            Save();
        }
        public static void DeleteDetail(int STTCT, int STTCTCT)
        {
            var editableCTiet = listCTChungtus(STTCT).Where(e => e.STTCTCT == STTCTCT).SingleOrDefault();
            if (editableCTiet != null)
            {
                listCTChungtus(STTCT).Remove(editableCTiet);
            }
            var tram = DB.ChitietCTs.SingleOrDefault(d => d.STTCTCT == STTCTCT);
            Delete(tram);
            Save();
        }

        /**/
        /**/

        public static IEnumerable PhieuXuatKhos(string maDvql, string maLoaiPhieu)
        {
            return (from result in DB.Chungtus
                    where result.Madvql == maDvql && result.Maloaiphieu == maLoaiPhieu
                    select result).OrderByDescending(p => p.STTCT).ToList();
        }

        public static void UpdateChungTuXuat(EditableChungTu chungtu)
        {
            var editableCTiet = listChungtus().Where(p => p.STTCT == chungtu.STTCT).SingleOrDefault();
            if (editableCTiet == null)
            {
                return;
            }
            editableCTiet.Ngayvietphieu = chungtu.Ngayvietphieu;
            editableCTiet.Maloaiphieu = chungtu.Maloaiphieu;
            editableCTiet.Tu = chungtu.Tu;
            editableCTiet.Den = chungtu.Den;
            editableCTiet.Nguoithuchien = chungtu.Nguoithuchien;
            editableCTiet.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            editableCTiet.Lydo = chungtu.Lydo;
            editableCTiet.Madvql = chungtu.Madvql;
            var edit = DB.Chungtus.SingleOrDefault(d => d.STTCT == chungtu.STTCT);

            edit.Ngayvietphieu = chungtu.Ngayvietphieu;
            edit.Maloaiphieu = chungtu.Maloaiphieu;
            edit.Tu = chungtu.Tu;
            edit.Den = chungtu.Den;
            edit.Nguoithuchien = chungtu.Nguoithuchien;
            edit.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            edit.Lydo = chungtu.Lydo;
            Save();
        }
        public static void InsertChungTuXuat(EditableChungTu chungtu)
        {
           var soCT = getMaPhatSinh(chungtu.Maloaiphieu);
           // HttpContext.Current.Session["newsoCT"] = soCT;
            chungtu.SoCT = soCT;
            var sttct = getFirstTTCT();
            HttpContext.Current.Session["STTCT"] = sttct;
          //  chungtu.STTCT = sttct;
            var editableCTu = new EditableChungTu();
           // editableCTu.STTCT = chungtu.STTCT;
            editableCTu.SoCT = chungtu.SoCT;
            editableCTu.Ngayvietphieu = chungtu.Ngayvietphieu;
            editableCTu.Maloaiphieu = chungtu.Maloaiphieu;
            editableCTu.Tu = chungtu.Tu;
            editableCTu.Den = chungtu.Den;
            editableCTu.Madvql = chungtu.Madvql;
            editableCTu.Nguoithuchien = chungtu.Nguoithuchien;
            editableCTu.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            editableCTu.Lydo = chungtu.Lydo;

            listChungtus().Add(editableCTu);

            var insert = new Chungtu();
          //  insert.STTCT = chungtu.STTCT;
            insert.SoCT = chungtu.SoCT;
            insert.Ngayvietphieu = chungtu.Ngayvietphieu;
            insert.Maloaiphieu = chungtu.Maloaiphieu;
            insert.Tu = chungtu.Tu;
            insert.Den = chungtu.Den;
            insert.Nguoithuchien = chungtu.Nguoithuchien;
            insert.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            insert.Lydo = chungtu.Lydo;
            insert.Madvql = chungtu.Madvql;
            AddNew(insert);
            Save();
        }

        /**/
        /**/

        public static IList<EditableChungTu> PhieuThuHois(string maDvql, string maLoaiPhieu)
        {
            return (from result in listChungtus()
                    where result.Madvql == maDvql && result.Maloaiphieu == maLoaiPhieu
                    select result).OrderByDescending(p => p.STTCT).ToList();
        }

        public static void UpdateChungTuThuHoi(EditableChungTu chungtu)
        {
            var editableCTiet = listChungtus().Where(p => p.STTCT == chungtu.STTCT).SingleOrDefault();
            if (editableCTiet == null)
            {
                return;
            }
            editableCTiet.Ngayvietphieu = chungtu.Ngayvietphieu;
            editableCTiet.Maloaiphieu = chungtu.Maloaiphieu;
            editableCTiet.Tu = chungtu.Tu;
            editableCTiet.Den = chungtu.Den;
            editableCTiet.Nguoithuchien = chungtu.Nguoithuchien;
            editableCTiet.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            editableCTiet.Lydo = chungtu.Lydo;
            editableCTiet.Madvql = chungtu.Madvql;
            var edit = DB.Chungtus.SingleOrDefault(d => d.STTCT == chungtu.STTCT);

            edit.Ngayvietphieu = chungtu.Ngayvietphieu;
            edit.Maloaiphieu = chungtu.Maloaiphieu;
            edit.Tu = chungtu.Tu;
            edit.Den = chungtu.Den;
            edit.Nguoithuchien = chungtu.Nguoithuchien;
            edit.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            edit.Lydo = chungtu.Lydo;
            Save();
        }
        public static void InsertChungTuThuHoi(EditableChungTu chungtu)
        {
            var soCT = getMaPhatSinh(chungtu.Maloaiphieu);

          //  HttpContext.Current.Session["newsoCT"] = soCT;
            chungtu.SoCT = soCT;
            var sttct = getFirstTTCT();
            HttpContext.Current.Session["STTCT"] = sttct;
         //   chungtu.STTCT = sttct;
            var editableCTu = new EditableChungTu();
           // editableCTu.STTCT = chungtu.STTCT;
            editableCTu.SoCT = chungtu.SoCT;
            editableCTu.Ngayvietphieu = chungtu.Ngayvietphieu;
            editableCTu.Maloaiphieu = chungtu.Maloaiphieu;
            editableCTu.Tu = chungtu.Tu;
            editableCTu.Den = chungtu.Den;
            editableCTu.Madvql = chungtu.Madvql;
            editableCTu.Nguoithuchien = chungtu.Nguoithuchien;
            editableCTu.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            editableCTu.Lydo = chungtu.Lydo;

            listChungtus().Add(editableCTu);
                
            var insert = new Chungtu();
          //  insert.STTCT = chungtu.STTCT;
            insert.SoCT = chungtu.SoCT;
            insert.Ngayvietphieu = chungtu.Ngayvietphieu;
            insert.Maloaiphieu = chungtu.Maloaiphieu;
            insert.Tu = chungtu.Tu;
            insert.Den = chungtu.Den;
            insert.Nguoithuchien = chungtu.Nguoithuchien;
            insert.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            insert.Lydo = chungtu.Lydo;
            insert.Madvql = chungtu.Madvql;
            AddNew(insert);
            Save();
        }

        /**/
        /**/

        public static IList<EditableChungTu> PhieuThanhLys(string maDvql, string maLoaiPhieu)
        {
            return (from result in listChungtus()
                    where result.Madvql == maDvql && result.Maloaiphieu == maLoaiPhieu
                    select result).OrderByDescending(p => p.STTCT).ToList();
        }


        public static void UpdateChungTuThanhLy(EditableChungTu chungtu)
        {
            var editableCTiet = listChungtus().Where(p => p.STTCT == chungtu.STTCT).SingleOrDefault();
            if (editableCTiet == null)
            {
                return;
            }
            editableCTiet.Ngayvietphieu = chungtu.Ngayvietphieu;
            editableCTiet.Maloaiphieu = chungtu.Maloaiphieu;
            editableCTiet.Tu = chungtu.Tu;
            editableCTiet.Den = chungtu.Den;
            editableCTiet.Nguoithuchien = chungtu.Nguoithuchien;
            editableCTiet.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            editableCTiet.Lydo = chungtu.Lydo;
            editableCTiet.Madvql = chungtu.Madvql;
            var edit = DB.Chungtus.SingleOrDefault(d => d.STTCT == chungtu.STTCT);

            edit.Ngayvietphieu = chungtu.Ngayvietphieu;
            edit.Maloaiphieu = chungtu.Maloaiphieu;
            edit.Tu = chungtu.Tu;
            edit.Den = chungtu.Den;
            edit.Nguoithuchien = chungtu.Nguoithuchien;
            edit.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            edit.Lydo = chungtu.Lydo;
            Save();
        }
        public static void InsertChungTuThanhLy(EditableChungTu chungtu)
        {
            var soCT = getMaPhatSinh(chungtu.Maloaiphieu);

          //  HttpContext.Current.Session["newsoCT"] = soCT;
            chungtu.SoCT = soCT;
            var sttct = getFirstTTCT();
            HttpContext.Current.Session["STTCT"] = sttct;
          //  chungtu.STTCT = sttct;
            var editableCTu = new EditableChungTu();
          //  editableCTu.STTCT = chungtu.STTCT;
            editableCTu.SoCT = chungtu.SoCT;
            editableCTu.Ngayvietphieu = chungtu.Ngayvietphieu;
            editableCTu.Maloaiphieu = chungtu.Maloaiphieu;
            editableCTu.Tu = chungtu.Tu;
            editableCTu.Den = chungtu.Den;
            editableCTu.Madvql = chungtu.Madvql;
            editableCTu.Nguoithuchien = chungtu.Nguoithuchien;
            editableCTu.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            editableCTu.Lydo = chungtu.Lydo;

            listChungtus().Add(editableCTu);

            var insert = new Chungtu();
          //  insert.STTCT = chungtu.STTCT;
            insert.SoCT = chungtu.SoCT;
            insert.Ngayvietphieu = chungtu.Ngayvietphieu;
            insert.Maloaiphieu = chungtu.Maloaiphieu;
            insert.Tu = chungtu.Tu;
            insert.Den = chungtu.Den;
            insert.Nguoithuchien = chungtu.Nguoithuchien;
            insert.Nguoigiaonhan = chungtu.Nguoigiaonhan;
            insert.Lydo = chungtu.Lydo;
            insert.Madvql = chungtu.Madvql;
            AddNew(insert);
            Save();
        }
    }

    public class EditableCCDC
    {
        public int STTDMCCDC { get; set; }

        [Display(Name = "MaCCDC")]
        public string MaCCDC { get; set; }
        /**/
        [Required(ErrorMessage = "Không được để trống")]
        [Display(Name = "TenCCDC")]
        public string TenCCDC { get; set; }
        /**/
        [Display(Name = "Chitiet")]
        public string Chitiet { get; set; }
        /**/
        [Display(Name = "Madvt")]
        public string Madvt { get; set; }
        /**/
        [Display(Name = "Manoisx")]
        public string Manoisx { get; set; }
        /**/
        [Display(Name = "Manhacc")]
        public string Manhacc { get; set; }
        /**/
        [Display(Name = "Ghichu")]
        public string Ghichu { get; set; }
        /**/
        [Display(Name = "Maloaits")]
        public string Maloaits { get; set; }
        /**/
        [Display(Name = "Maloaiccdc")]
        public string Maloaiccdc { get; set; }
        /**/
        [Display(Name = "Manguonvon")]
        public string Manguonvon { get; set; }
        /**/
        [Display(Name = "Hinhanh")]
        public string Hinhanh { get; set; }
        /**/
        [Display(Name = "Madvql")]
        public string Madvql { get; set; }
        public int SoLuong { get; set; }
    }
    public class EditableChungTu
    {
        public int STTCT { get; set; }
        /**/
     
        public string Maloaiphieu { get; set; }
        /**/
        [Display(Name = "SoCT")]
        public string SoCT { get; set; }
        /**/
        [Required(ErrorMessage = "Không được để trống")]
        [Display(Name = "Tu")]
        public string Tu { get; set; }
        /**/
        [Required(ErrorMessage = "Không được để trống")]
        [Display(Name = "Den")]
        public string Den { get; set; }
        /**/
        [Required(ErrorMessage = "Không được để trống")]
        [Display(Name = "Ngayvietphieu")]
        public DateTime? Ngayvietphieu { get; set; }
        /**/
        public string Nguoigiaonhan { get; set; }
        public string Nguoithuchien { get; set; }
        public string Lydo { get; set; }
        public string Madvql { get; set; }
    }
    public class EditableCTChungTu
    {
        public int STTCT { get; set; }
        public int STTCTCT { get; set; }
        public string SoCT { get; set; }
        /**/
        [Display(Name = "MaCCDC")]
        public string MaCCDC { get; set; }
        /**/
          [Required(ErrorMessage = "Không được để trống")]
        [Display(Name = "TenCCDC")]
        public string TenCCDC { get; set; }
        /**/
          [Required(ErrorMessage = "Không được để trống")]
        [Display(Name = "Soluong")]
        public int? Soluong { get; set; }
        /**/
        [Display(Name = "Dongia")]
        public decimal? Dongia { get; set; }
        /**/
        [Display(Name = "NgaySD")]
        public DateTime? NgaySD { get; set; }
        /**/
        [Display(Name = "NgayHHBH")]
        public DateTime? NgayHHBH { get; set; }
        /**/
        public string Ghichu { get; set; }
        public string Madvql { get; set; }
        public string Madvt { get; set; }
        public string Madonvi { get; set; }
        public string Matinhtrang { get; set; }

          [Required(ErrorMessage = "Không được để trống")]
        [Display(Name = "NguonVon")]
        public string NguonVon { get; set; }
    }
    public class EditableDonVi
    {
        public int STTDMDV { get; set; }

        /**/
        [Display(Name = "Madonvi")]
        public string Madonvi { get; set; }
        /**/
        [Display(Name = "Tendonvi")]
        public string Tendonvi { get; set; }

        public string Madvql { get; set; }
        /**/
    }
}

