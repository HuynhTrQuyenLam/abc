using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnQuanLyTapHoa.Models
{
    public class MatHangMua
    {
        QLBANDTEntities db = new QLBANDTEntities();
        public int Masp { get; set; }
        public string Tensp { get; set; }
        public string Hinh1 { get; set; }
        public double Gia { get; set; }
        public int SoLuong { get; set; }
        public int Payment { get; set; }
        public double ThanhTien()
        {
            return SoLuong * Gia;
        }
        public MatHangMua(int MaSP)
        {
            this.Masp = MaSP;
            var sp = db.SanPhams.Single(s => s.MaSP == this.Masp);
            this.Tensp = sp.TenSP;
            this.Hinh1 = sp.Hinh1;
            this.Gia = double.Parse(sp.GiaSp.ToString());
            this.SoLuong = 1;
        }
    }
}