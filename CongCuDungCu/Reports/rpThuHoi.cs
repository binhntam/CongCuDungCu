using System.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using CongCuDungCu.Models;

namespace CongCuDungCu.Reports
{
    public partial class rpThuHoi : DevExpress.XtraReports.UI.XtraReport
    {
        public rpThuHoi()
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
        public void getIDDonVi(string userName)
        {
            IDDonVi = CCDCDataProvider.GetMadvql_ByUserName(userName);
        }

        public void Fill(DateTime dtime)
        {
            var db = new CCDCDataContext();
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


            DataSet ds = dsCCDC;
            listMaCCDCThuHois.CopyToDataTable(ds.Tables["CCDC"]);
        }
        private string getDonViSuDung(string SoCT)
        {
            return (from ct in CongCuDungCu.Models.DataSource.db.Chungtus
                    where ct.SoCT == SoCT
                    select ct.Den).FirstOrDefault();
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
