using System.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using CongCuDungCu.Models;

namespace CongCuDungCu.Reports
{
    public partial class rpNhapKho1Phieu : DevExpress.XtraReports.UI.XtraReport
    {
        public rpNhapKho1Phieu()
        {
            InitializeComponent();
        }
     

        public void Fill(int sttct)
        {
            var db = new CCDCDataContext();

            var soCTMuaMois = (from ct in db.Chungtus
                               where ct.STTCT == sttct
                               select ct.SoCT).FirstOrDefault();
            var lyDo = (from ct in db.Chungtus
                               where ct.STTCT == sttct
                               select ct.Lydo).FirstOrDefault();
            var nhap = (from ct in db.Chungtus
                        where ct.STTCT == sttct
                        select ct.Den).FirstOrDefault();
            var listMaCCDCMuaMois = (from ctietTH in db.ChitietCTs
                                     where ctietTH.SoCT == soCTMuaMois
                                     select new
                                     {
                                         SoCT = ctietTH.SoCT,
                                         DonViSuDung = ctietTH.Madonvi,
                                         MaCCDC = ctietTH.MaCCDC,
                                         TenCCDC = ctietTH.TenCCDC,
                                         SoLuong = ctietTH.Soluong,
                                         NgaySuDung = ctietTH.NgaySD,
                                         DonViTinh = ctietTH.Madvt,
                                         TinhTrang = ctietTH.Matinhtrang,
                                         LyDo=lyDo,                                         
                                     }).ToList();
         
         
            DataSet ds = dsCCDC;
            listMaCCDCMuaMois.CopyToDataTable(ds.Tables["CCDC"]);
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

     
    }
}
