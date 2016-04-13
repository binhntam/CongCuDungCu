using System.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using CongCuDungCu.Models;

namespace CongCuDungCu.Reports
{
    public partial class rpMuaMoi : DevExpress.XtraReports.UI.XtraReport
    {
        public rpMuaMoi()
        {
            InitializeComponent();
        }
        protected string idDonVi { get; set; }
        protected string IDDonVi
        {
            get
            {
                return idDonVi;
            }
            set
            {
                idDonVi = value;
            }
        }
        public void getIDDonViCo(string userName)
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
                                SoLuongXuat = g.Sum(k => k.SoLuong),
                                SoCT = g.Select(k => k.SoCT).FirstOrDefault(),
                                DonViSuDung = g.Select(k => k.DonViSuDung).FirstOrDefault(),
                                TenCCDC = g.Select(k => k.TenCCDC).FirstOrDefault(),
                                NgaySuDung = g.Select(k => k.NgaySuDung).FirstOrDefault(),
                                DonViTinh = g.Select(k => k.DonViTinh).FirstOrDefault(),
                                TinhTrang = g.Select(k => k.TinhTrang).FirstOrDefault(),
                            };
            DataSet ds = dsCCDC;
            SumMuaMoi.CopyToDataTable(ds.Tables["CCDC"]);
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
