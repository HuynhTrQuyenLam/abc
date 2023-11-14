using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnQuanLyTapHoa.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        QLBANDTEntities db = new QLBANDTEntities();
        public List<MatHangMua> LayGioHang()
        {
            List<MatHangMua> gioHang = Session["GioHang"] as List<MatHangMua>;
            if (gioHang == null)
            {
                gioHang = new List<MatHangMua>();
                Session["GioHang"] = gioHang;
            }
            return gioHang;
        }

        public ActionResult ThemSanPhamVaoGio(int MaSP)
        {
            List<MatHangMua> gioHang = LayGioHang();
            MatHangMua sanPham = gioHang.FirstOrDefault(s => s.Masp == MaSP);
            if (sanPham == null)
            {
                sanPham = new MatHangMua(MaSP);
                gioHang.Add(sanPham);
            }
            else
            {
                sanPham.SoLuong++;
            }
            return RedirectToAction("HienThiGioHang", "Cart", new { id = MaSP });
        }
        private int TinhTongSL()
        {
            int tongSL = 0;
            List<MatHangMua> gioHang = LayGioHang();
            if (gioHang != null)
                tongSL = gioHang.Sum(sp => sp.SoLuong);
            return tongSL;
        }
        private double TinhTongTien()
        {
            double TongTien = 0;
            List<MatHangMua> gioHang = LayGioHang();
            if (gioHang != null)
                TongTien = gioHang.Sum(sp => sp.ThanhTien());
            return TongTien;
        }

        private double TinhTongTienvnd()
        {
            double TongTien = 0;
            decimal b = 23000;
            List<MatHangMua> gioHang = LayGioHang();
            if (gioHang != null)
            {
                TongTien = gioHang.Sum(sp => sp.ThanhTien());
            }
            decimal c = (decimal)TongTien / b;
            return (double)c;
        }





        public ActionResult HienThiGioHang()
        {
            List<MatHangMua> gioHang = LayGioHang();
            if (gioHang == null || gioHang.Count == 0)
            {
                return RedirectToAction("ProductList", "SanPhams");
            }
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return View(gioHang);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return PartialView();
        }
        public ActionResult XoaMatHang(int MaSP)
        {
            List<MatHangMua> gioHang = LayGioHang();
            var sanpham = gioHang.FirstOrDefault(s => s.Masp == MaSP);
            if (sanpham != null)
            {
                gioHang.RemoveAll(s => s.Masp == MaSP);
                return RedirectToAction("HienThiGioHang");
            }
            if (gioHang.Count == 0)
                return RedirectToAction("ProductList", "SanPhams");
            return RedirectToAction("HienThiGioHang");
        }

        public ActionResult CapNhatMatHang(int MaSP, int SoLuong)
        {
            List<MatHangMua> gioHang = LayGioHang();
            var sanpham = gioHang.FirstOrDefault(s => s.Masp == MaSP);

            if (sanpham != null)
            {
                sanpham.SoLuong = SoLuong;
            }
            return RedirectToAction("HienThiGioHang", "Cart");
        }

        public ActionResult DatHang()
        {
            List<MatHangMua> gioHang = LayGioHang();
            if (gioHang == null || gioHang.Count == 0)
                return RedirectToAction("ProductList", "SanPhams");
            ViewBag.TongSL = TinhTongSL();
            ViewBag.TongTien = TinhTongTien();
            return View(gioHang);
        }



        public ActionResult DongYDatHang(bool isDirectPayment)
        {

            if (isDirectPayment)
            {
                // Thực hiện thao tác cập nhật dữ liệu ở đây
                User khach = Session["TaiKhoan"] as User;
                List<MatHangMua> gioHang = LayGioHang();
                DONDATHANG DonHang = new DONDATHANG();
                DonHang.MaUser = khach.MaUser;
                DonHang.NgayDH = DateTime.Now;
                DonHang.Trigia = (int)TinhTongTien();
                DonHang.Dagiao = false;
                DonHang.Tennguoinhan = khach.TenUser;
                DonHang.Diachinhan = khach.DiaChi;
                DonHang.Dienthoainhan = khach.sdt;
                DonHang.HinhThucTT = true;
                DonHang.HTGiaohang = false;
                db.DONDATHANGs.Add(DonHang);
                db.SaveChanges();

                foreach (var sanpham in gioHang)
                {
                    CTDATHANG chitiet = new CTDATHANG();
                    {
                        chitiet.SODH = DonHang.SODH;
                        chitiet.MaSP = sanpham.Masp;
                        chitiet.Soluong = sanpham.SoLuong;
                        chitiet.Dongia = (int)sanpham.Gia;
                    }

                    db.CTDATHANGs.Add(chitiet);
                    Session["GioHang"] = null;
                    foreach (var p in db.SanPhams.Where(s => s.MaSP == chitiet.MaSP)) // lấy ID Product có trong giỏ hàng
                    {
                        var update_quan_pro = p.SoLuong - chitiet.Soluong; //Số lượng tồn mới  = số lượng tồn - số lượng đã mua 
                        if (p.SoLuong > 0)
                        {
                            p.SoLuong = (int)update_quan_pro; //Thực hiện cập nhật lại số lượng tồn cho cột Quantity của bảng Product

                        }
                    }
                }

                db.SaveChanges();
                Session["GioHang"] = null;
                return RedirectToAction("HoanThanhDonHang");

            }
            else
            {
                return RedirectToAction("PaymentWithPaypal", "Cart");
            }


        }


        public ActionResult HoanThanhDonHang()
        {
            return View();
        }
    }
}