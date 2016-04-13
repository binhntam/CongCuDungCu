using System.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using CongCuDungCu.Models;

namespace CongCuDungCu.Reports
{
    public partial class rpDonViDuocCap : DevExpress.XtraReports.UI.XtraReport
    {
        public rpDonViDuocCap()
        {
            InitializeComponent();
        }
        private static CCDCDataContext GetDb()
        {
            var db = new CCDCDataContext();
            return db;
        }
        protected string IDDonVi { get; set; }
        public void getIDDonVi(string userName)
        {
            IDDonVi = CCDCDataProvider.GetMadvql_ByUserName(userName);
        }


        public void Fill(string maDonViCo)
        {
            var db = GetDb();


            var tu = (from p in db.DMDonvis
                      where p.Madonvi == maDonViCo
                      select p.Tendonvi).FirstOrDefault();

            var soCTXuats = (from ct in db.Chungtus

                             where ct.Den.Contains(tu) && ct.Maloaiphieu == "X00" && ct.Madvql == IDDonVi
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
            var result = from p in SumXuat
                         select new
                         {
                             MaCCDC = p.MaCCDC,
                             SoLuongXuat = (int)(p.SoLuongXuat),
                             SoCT = p.SoCT,
                             DonViSuDung = p.DonViSuDung,
                             TenCCDC = p.TenCCDC,
                             NgaySuDung = p.NgaySuDung,
                             DonViTinh = p.DonViTinh,
                             TinhTrang = p.TinhTrang,
                         };
            var after = from p in result
                        group p by p.MaCCDC
                         into g
                        select new
                        {
                            MaCCDC = g.Key,
                            SoLuong = g.Sum(k => k.SoLuongXuat),
                            SoCT = g.Select(k => k.SoCT).FirstOrDefault(),
                            DonViSuDung = g.Select(k => k.DonViSuDung).FirstOrDefault(),
                            TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                            NgaySuDung = g.Select(k => k.NgaySuDung).FirstOrDefault(),
                            DonViTinh = g.Select(k => k.DonViTinh).FirstOrDefault(),
                            TinhTrang = g.Select(k => k.TinhTrang).FirstOrDefault(),

                        };
            DataSet ds = dsCCDC;
            after.CopyToDataTable(ds.Tables["CCDC"]);
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
