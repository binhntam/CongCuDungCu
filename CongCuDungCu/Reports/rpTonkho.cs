using System.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using CongCuDungCu.Models;

namespace CongCuDungCu.Reports
{
    public partial class rpTonKho : DevExpress.XtraReports.UI.XtraReport
    {
        public rpTonKho()
        {
            InitializeComponent();
        }
        protected string IDDonVi { get; set; }
        public void getIDDonVi(string userName)
        {
            IDDonVi = CCDCDataProvider.GetMadvql_ByUserName(userName);
        }


        public void Fill(DateTime dtime)
        {
            var db = new CCDCDataContext();
            var soCTMuaMois = (from ct in db.Chungtus
                               where ct.Maloaiphieu == "N00" && ct.Madvql == IDDonVi
                               group ct by ct.SoCT
                                into g
                               select new
                               {
                                   SoCT = g.Key,

                               });
            var listMaCCDCMuaMois = (from ctTH in soCTMuaMois
                                     join ctietTH in db.ChitietCTs on ctTH.SoCT equals ctietTH.SoCT
                                     select new
                                     {
                                         SoCT = ctTH.SoCT,
                                         DonViSuDung = ctietTH.Madonvi,
                                         MaCCDC = ctietTH.MaCCDC,
                                         TenCCDC = ctietTH.TenCCDC,
                                         SoLuong = ctietTH.Soluong,
                                         NgaySuDung = ctietTH.NgaySD,
                                         DonViTinh = ctietTH.Madvt,
                                         TinhTrang = ctietTH.Matinhtrang,
                                     }).ToList();
            var SumMuaMoi = from p in listMaCCDCMuaMois
                            group p by p.MaCCDC
                             into g
                            select new
                            {
                                MaCCDC = g.Key,
                                SoLuongMoi = g.Sum(k => k.SoLuong),
                                SoCT = g.Select(k => k.SoCT).FirstOrDefault(),
                                DonViSuDung = g.Select(k => k.DonViSuDung).FirstOrDefault(),
                                TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                                NgaySuDung = g.Select(k => k.NgaySuDung).FirstOrDefault(),
                                DonViTinh = g.Select(k => k.DonViTinh).FirstOrDefault(),
                                TinhTrang = g.Select(k => k.TinhTrang).FirstOrDefault(),
                            };
            var soCTThuHois = (from ct in db.Chungtus
                               where ct.Maloaiphieu == "N01" && ct.Madvql == IDDonVi
                               group ct by ct.SoCT
                                into g
                               select new
                               {
                                   SoCT = g.Key,
                                   });

            var listMaCCDCThuHois = (from ctTH in soCTThuHois
                                     join ctietTH in db.ChitietCTs on ctTH.SoCT equals ctietTH.SoCT

                                     select new
                                     {
                                         SoCT = ctTH.SoCT,
                                         DonViSuDung = ctietTH.Madonvi,
                                         MaCCDC = ctietTH.MaCCDC,
                                         TenCCDC = ctietTH.TenCCDC,
                                         SoLuong = ctietTH.Soluong,
                                         NgaySuDung = ctietTH.NgaySD,
                                         DonViTinh = ctietTH.Madvt,
                                         TinhTrang = ctietTH.Matinhtrang,
                                     }).ToList();
            var SumThuHoi = from p in listMaCCDCThuHois
                            group p by p.MaCCDC
                             into g
                            select new
                            {
                                MaCCDC = g.Key,
                                SoLuongThuHoi = g.Sum(k => k.SoLuong),
                                SoCT = g.Select(k => k.SoCT).FirstOrDefault(),
                                DonViSuDung = g.Select(k => k.DonViSuDung).FirstOrDefault(),
                                TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                                NgaySuDung = g.Select(k => k.NgaySuDung).FirstOrDefault(),
                                DonViTinh = g.Select(k => k.DonViTinh).FirstOrDefault(),
                                TinhTrang = g.Select(k => k.TinhTrang).FirstOrDefault(),
                            };
            var resultNhap = from p in SumMuaMoi
                         join p2 in SumThuHoi on p.MaCCDC equals p2.MaCCDC
                             into banGroup
                         from ban in banGroup.DefaultIfEmpty()
                         select new
                             {
                             MaCCDC = p.MaCCDC,
                             SoLuongMoi = (int)(p.SoLuongMoi),
                             SoLuongThuHoi = (ban != null) ? ban.SoLuongThuHoi : 0,
                             SoCT = p.SoCT,
                             DonViSuDung = p.DonViSuDung,
                             TenCCDC = p.TenCCDC,
                             NgaySuDung = p.NgaySuDung,
                             DonViTinh = p.DonViTinh,
                             TinhTrang = p.TinhTrang,
                         };
            var afterNhap = from p in resultNhap
                        group p by p.MaCCDC
                             into g
                        select new
                            {
                            MaCCDC = g.Key,
                            SoLuongNhap = g.Sum(k => k.SoLuongMoi + k.SoLuongThuHoi),
                            SoCT = g.Select(k => k.SoCT).FirstOrDefault(),
                            DonViSuDung = g.Select(k => k.DonViSuDung).FirstOrDefault(),
                            TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                            NgaySuDung = g.Select(k => k.NgaySuDung).FirstOrDefault(),
                            DonViTinh = g.Select(k => k.DonViTinh).FirstOrDefault(),
                            TinhTrang = g.Select(k => k.TinhTrang).FirstOrDefault(),
                        };
            //XUAT
            var soCTXuats = (from ct in db.Chungtus
                             where ct.Maloaiphieu == "X00" && ct.Madvql == IDDonVi
                             group ct by ct.SoCT
                              into g
                             select new
                             {
                                 SoCT = g.Key,
                                 });
            var listMaCCDCXuats = (from ctX in soCTXuats
                                   join ctietTH in db.ChitietCTs on ctX.SoCT equals ctietTH.SoCT
                                   select new
                                   {
                                       SoCT = ctX.SoCT,
                                       DonViSuDung = ctietTH.Madonvi,
                                       MaCCDC = ctietTH.MaCCDC,
                                       TenCCDC = ctietTH.TenCCDC,
                                       SoLuong = ctietTH.Soluong,
                                       NgaySuDung = ctietTH.NgaySD,
                                       DonViTinh = ctietTH.Madvt,
                                       TinhTrang = ctietTH.Matinhtrang,
                                   }).ToList();

            var SumXuat = from p in listMaCCDCXuats
                          group p by p.MaCCDC
                           into g
                          select new
                          {
                              MaCCDC = g.Key,
                              SoLuongXuat = g.Sum(k => k.SoLuong),
                              SoCT = g.Select(k => k.SoCT).FirstOrDefault(),
                              DonViSuDung = g.Select(k => k.DonViSuDung).FirstOrDefault(),
                              TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                              NgaySuDung = g.Select(k => k.NgaySuDung).FirstOrDefault(),
                              DonViTinh = g.Select(k => k.DonViTinh).FirstOrDefault(),
                              TinhTrang = g.Select(k => k.TinhTrang).FirstOrDefault(),
                          };
            //THANH LY
            var soCTThanhLys = (from ct in db.Chungtus
                             where ct.Maloaiphieu == "X01" && ct.Madvql == IDDonVi
                             group ct by ct.SoCT
                                 into g
                                 select new
                                 {
                                     SoCT = g.Key,
                                 });
            var listMaCCDCThanhLys = (from ctX in soCTThanhLys
                                   join ctietTH in db.ChitietCTs on ctX.SoCT equals ctietTH.SoCT
                                   select new
                                   {
                                       SoCT = ctX.SoCT,
                                       DonViSuDung = ctietTH.Madonvi,
                                       MaCCDC = ctietTH.MaCCDC,
                                       TenCCDC = ctietTH.TenCCDC,
                                       SoLuong = ctietTH.Soluong,
                                       NgaySuDung = ctietTH.NgaySD,
                                       DonViTinh = ctietTH.Madvt,
                                       TinhTrang = ctietTH.Matinhtrang,
                                   }).ToList();

            var SumThanhLy = from p in listMaCCDCThanhLys
                          group p by p.MaCCDC
                              into g
                              select new
                              {
                                  MaCCDC = g.Key,
                                  SoLuongThanhLy = g.Sum(k => k.SoLuong),
                                  SoCT = g.Select(k => k.SoCT).FirstOrDefault(),
                                  DonViSuDung = g.Select(k => k.DonViSuDung).FirstOrDefault(),
                                  TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                                  NgaySuDung = g.Select(k => k.NgaySuDung).FirstOrDefault(),
                                  DonViTinh = g.Select(k => k.DonViTinh).FirstOrDefault(),
                                  TinhTrang = g.Select(k => k.TinhTrang).FirstOrDefault(),
                              };
            //Xuat+ Thanh ly= Xuat2
            var resultXuat2 = from p in SumXuat
                              join p2 in SumThanhLy on p.MaCCDC equals p2.MaCCDC
                                 into banGroup
                             from ban in banGroup.DefaultIfEmpty()
                             select new
                             {
                                 MaCCDC = p.MaCCDC,
                                 SoLuongXuat = (int)(p.SoLuongXuat),
                                 SoLuongThanhLy = (ban != null) ? ban.SoLuongThanhLy : 0,
                                 SoCT = p.SoCT,
                                 DonViSuDung = p.DonViSuDung,
                                 TenCCDC = p.TenCCDC,
                                 NgaySuDung = p.NgaySuDung,
                                 DonViTinh = p.DonViTinh,
                                 TinhTrang = p.TinhTrang,
                             };
            var afterXuat2 = from p in resultXuat2
                            group p by p.MaCCDC
                                into g
                                select new
                                {
                                    MaCCDC = g.Key,
                                    SoLuongXuat2 = g.Sum(k => k.SoLuongXuat + k.SoLuongThanhLy),
                                    SoCT = g.Select(k => k.SoCT).FirstOrDefault(),
                                    DonViSuDung = g.Select(k => k.DonViSuDung).FirstOrDefault(),
                                    TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                                    NgaySuDung = g.Select(k => k.NgaySuDung).FirstOrDefault(),
                                    DonViTinh = g.Select(k => k.DonViTinh).FirstOrDefault(),
                                    TinhTrang = g.Select(k => k.TinhTrang).FirstOrDefault(),
                                };
            //
            var resultTon = from p in afterNhap
                            join p2 in afterXuat2 on p.MaCCDC equals p2.MaCCDC
                             into banGroup
                         from ban in banGroup.DefaultIfEmpty()
                         select new
                            {
                             MaCCDC = p.MaCCDC,
                             SoLuongNhap = (int)(p.SoLuongNhap),
                             SoLuongXuat2 = (ban != null) ? ban.SoLuongXuat2 : 0,
                             SoCT = p.SoCT,
                             DonViSuDung = p.DonViSuDung,
                             TenCCDC = p.TenCCDC,
                             NgaySuDung = p.NgaySuDung,
                             DonViTinh = p.DonViTinh,
                             TinhTrang = p.TinhTrang,
                         };
            var afterTonKho = from p in resultTon
                        group p by p.MaCCDC
                               into g
                        select new
                              {
                            MaCCDC = g.Key,
                            SoLuong = g.Sum(k => k.SoLuongNhap - k.SoLuongXuat2),
                            SoCT = g.Select(k => k.SoCT).FirstOrDefault(),
                            DonViSuDung = g.Select(k => k.DonViSuDung).FirstOrDefault(),
                            TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                            NgaySuDung = g.Select(k => k.NgaySuDung).FirstOrDefault(),
                            DonViTinh = g.Select(k => k.DonViTinh).FirstOrDefault(),
                            TinhTrang = g.Select(k => k.TinhTrang).FirstOrDefault(),
                        };
            var afterAgain = from p in afterTonKho
                             where p.SoLuong > 0
                             select new
                             {
                                 MaCCDC = p.MaCCDC,
                                 SoLuong = p.SoLuong,
                                 DonViSuDung = p.DonViSuDung,
                                 TenCCDC = p.TenCCDC,
                                 NgaySuDung = p.NgaySuDung,
                                 DonViTinh = p.DonViTinh,
                                 TinhTrang = p.TinhTrang,
                                 SoCT = p.SoCT,
                             };
            DataSet ds = dsCCDC;
            afterAgain.CopyToDataTable(ds.Tables["CCDC"]);
        }


        private int stt = 0;
        private void Detail_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            stt++;
            colSTT.Text = stt.ToString();
        }

        private void ReportHeader_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            colNgayThang.Text = "Ninh Thuận, ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
        }

        private void ReportFooter_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
        }
    }
}
