using System.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using CongCuDungCu.Models;

namespace CongCuDungCu.Reports
{
    public partial class rpDonViCo : DevExpress.XtraReports.UI.XtraReport
    {
        public rpDonViCo()
        {
            InitializeComponent();
        }

        private static CCDCDataContext GetDb()
        {
            var db = new CCDCDataContext();
            return db;
        }
        protected string IDDonVi { get; set; }
        public void getIDDonViCo(string userName)
        {
            IDDonVi = CCDCDataProvider.GetMadvql_ByUserName(userName);
        }
        protected string TenDonViSD { get; set; }
        public void getTenDonViSD(string maDonViCo)
        {
            TenDonViSD = CCDCDataProvider.getTenDvi_ByID(maDonViCo);
        }
        public void Fill(string maDonViCo)
        {
            var db = GetDb();


            var tu = (from p in db.DMDonvis
                      where p.Madonvi == maDonViCo
                      select p.Tendonvi).FirstOrDefault();

            var soCTThuHois = (from ct in db.Chungtus
                              where ct.Tu.Contains(tu) && ct.Maloaiphieu == "N01" && ct.Madvql == IDDonVi
                             group ct by ct.SoCT
                                into g
                            select new
                               {
                                 SoCT = g.Key,
                                });
            var soCTXuats = (from ct in db.Chungtus

                             where ct.Den.Contains(tu) && ct.Maloaiphieu == "X00" && ct.Madvql == IDDonVi
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
                                         NguonVon = ctietTH.NguonVon,
                                         Ghichu = ctietTH.Ghichu,
                                     }).ToList();
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
                                         NguonVon = ctietTH.NguonVon,
                                         Ghichu = ctietTH.Ghichu,
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
                              NguonVon = g.Select(k => k.NguonVon).FirstOrDefault(),
                              Ghichu = g.Select(k => k.Ghichu).FirstOrDefault(),
                          };
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
                              NguonVon = g.Select(k => k.NguonVon).FirstOrDefault(),
                            //  Ghichu = g.Select(k => k.Ghichu).FirstOrDefault(),
                          };

            var result = from p in SumXuat
                         join p2 in SumThuHoi on p.MaCCDC equals p2.MaCCDC
                             into banGroup
                             from ban in banGroup.DefaultIfEmpty()
                         select new
                         {
                             MaCCDC = p.MaCCDC,
                             SoLuongXuat = (int)(p.SoLuongXuat),
                             SoLuongThuHoi = (ban != null) ? ban.SoLuongThuHoi : 0,
                             SoCT = p.SoCT,
                             DonViSuDung = p.DonViSuDung,
                             TenCCDC = p.TenCCDC,
                             NgaySuDung = p.NgaySuDung,
                             DonViTinh = p.DonViTinh,
                             TinhTrang = p.TinhTrang,
                             NguonVon = p.NguonVon,
                             Ghichu = p.Ghichu,
                         };
            var after = from p in result
                          group p by p.MaCCDC
                         into g
                          select new
                        {
                              MaCCDC = g.Key,
                              SoLuong = g.Sum(k => k.SoLuongXuat - k.SoLuongThuHoi),
                              SoCT = g.Select(k => k.SoCT).FirstOrDefault(),
                              DonViSuDung = g.Select(k => k.DonViSuDung).FirstOrDefault(),
                              TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                              NgaySuDung = g.Select(k => k.NgaySuDung).FirstOrDefault(),
                              DonViTinh = g.Select(k => k.DonViTinh).FirstOrDefault(),
                              TinhTrang = g.Select(k => k.TinhTrang).FirstOrDefault(),
                              NguonVon = g.Select(k => k.NguonVon).FirstOrDefault(),
                              Ghichu = g.Select(k => k.Ghichu).FirstOrDefault(),
                          };
            var afterAgain = (from p in after
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
                                 NguonVon = p.NguonVon,
                                 Ghichu = p.Ghichu,
                             }).OrderBy(p=>p.TenCCDC );

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
            colDonvi.Text = "Đơn vị: " + TenDonViSD;
        }

        private void ReportFooter_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
        }
    }
}
