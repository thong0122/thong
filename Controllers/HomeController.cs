using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebBanQuanAo.Models;
using PagedList;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Diagnostics;
using System.Data.Entity;
using System.Web.UI;
using System.Drawing.Printing;

namespace WebBanQuanAo.Controllers
{
    public class HomeController : Controller
    {
        DbDataContext db = new DbDataContext();
        // GET: Home
        public ActionResult Index()
        {
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            List<SanPham> sanPhamList = db.SanPhams.Take(8).ToList();
            ViewBag.DanhMucList = danhMucList;
            ViewBag.SanPhamList = sanPhamList;
            return View();
        }
        public ActionResult Shop(int? uid, int? page)
        {
            Session["UID"] = uid;
            int pageSize = 6; 
            int pageNumber = (page ?? 1); 
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = danhMucList;
            var q = db.SanPhams.Where(u => u.MaDanhMuc == uid).ToPagedList(pageNumber, pageSize);
            return View(q);
        }
        public ActionResult SendMail(string name, string email, string subject, string message)
        {
            try
            {
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string smtpUsername = "thanthong";
                string smtpPassword = "123@thong";

                using (SmtpClient client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(email);
                    mailMessage.To.Add("kienpro0987@gmail.com"); 
                    mailMessage.Subject = subject;
                    mailMessage.Body = $"Name: {name}\nEmail: {email}\nMessage: {message}";

                    client.Send(mailMessage);
                }

                Session["EmailSuccessMessage"] = "Email đã được gửi thành công.";
            }
            catch (Exception ex)
            {
                Session["EmailSuccessMessage"] = $"Đã xảy ra lỗi trong quá trình gửi email: {ex.Message}";
            }

            return RedirectToAction("Contact");
        }

        public ActionResult Contact()
        {
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = danhMucList;
            return View();
        }
        [AcceptVerbs("GET", "POST")]
        public ActionResult TimKiem(int? uid, int? page, string s)
        {
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = danhMucList;
            Session["s"] = s;
            Session["UID"] = uid;
            int pageSize = 6;
            int pageNumber = (page ?? 1); 
            var query = db.SanPhams.Where(c => c.MaDanhMuc == uid && c.TenSanPham.Contains(s)).ToPagedList(pageNumber, pageSize);
            return View(query);
        }
        [AcceptVerbs("GET", "POST")]
        public ActionResult SapXepTangDan(int? uid, int? page)
        {
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = danhMucList;
            Session["UID"] = uid;
            int pageSize = 6; 
            int pageNumber = (page ?? 1); 
            var query = db.SanPhams.Where(c => c.MaDanhMuc == uid).OrderBy(g => g.Gia).ToPagedList(pageNumber, pageSize);
            return View(query);
        }
        [AcceptVerbs("GET", "POST")]
        public ActionResult SapXepGiamDan(int? uid, int? page)
        {
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = danhMucList;
            Session["UID"] = uid;
            int pageSize = 6;
            int pageNumber = (page ?? 1); 
            var query = db.SanPhams.Where(c => c.MaDanhMuc == uid).OrderByDescending(g => g.Gia).ToPagedList(pageNumber, pageSize);
            return View(query);
        }
        [AcceptVerbs("GET", "POST")]
        public ActionResult LocTheoGia(int? idloc, int? page, bool? priceAll, bool? price1, bool? price2, bool? price3, bool? price4)
        {
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = danhMucList;
            Session["price1"] = false;
            Session["price2"] = false;
            Session["price3"] = false;
            Session["price4"] = false;
            int pageSize = 6; 
            int pageNumber = (page ?? 1); 
            var query = db.SanPhams.Where(c => c.MaDanhMuc == idloc);
            if (priceAll != true)
            {
                if (price1 == true)
                {
                    query = query.Where(p => p.Gia >= 0 && p.Gia <= 100000);
                    Session["price1"] = price1;
                }
                if (price2 == true)
                {
                    query = query.Where(p => p.Gia > 100000 && p.Gia <= 200000);
                    Session["price2"] = price2;
                }
                if (price3 == true)
                {
                    query = query.Where(p => p.Gia > 200000 && p.Gia <= 500000);
                    Session["price3"] = price3;
                }
                if (price4 == true)
                {
                    query = query.Where(p => p.Gia > 500000);
                    Session["price4"] = price4;
                }
            }

            var shop = query.ToPagedList(pageNumber, pageSize);
            return View(shop);
        }
        
        public ActionResult Details(int id, string succes)
        {
            ViewData["success"] = succes;
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = danhMucList;
            var q = db.SanPhams.SingleOrDefault(u=>u.MaSanPham==id);
            List<SanPham> list = db.SanPhams.Where(u=>u.Gia==q.Gia).ToList();
            ViewBag.List = list;
            return View(q);
        }
        [AcceptVerbs("GET", "POST")]
        public ActionResult AddToCart(int productId, string size, string color, int quantity)
        {
            NguoiDung nguoiDung = Session["TaiKhoan"] as NguoiDung;
            int userId = nguoiDung.MaNguoiDung;

            GioHang existingCartItem = db.GioHangs.SingleOrDefault(c => c.MaNguoiDung == userId && c.MaSanPham == productId && c.KichThuoc == size && c.Mau == color);
            var sp = db.SanPhams.SingleOrDefault(c => c.MaSanPham == productId);

            if (existingCartItem != null)
            {
                existingCartItem.SoLuong += quantity;
            }
            else
            {
                decimal gia = 0;
                if (sp.Gia != null)
                {
                    gia = (decimal)sp.Gia;
                }

                GioHang newCartItem = new GioHang
                {
                    MaNguoiDung = userId,
                    MaSanPham = productId,
                    KichThuoc = size,
                    Mau = color,
                    SoLuong = quantity,
                    TenSanPham = sp.TenSanPham,
                    Gia = gia,
                    ThanhTien = quantity * gia
                };

                db.GioHangs.InsertOnSubmit(newCartItem);
            }

            db.SubmitChanges();

            ViewData["success"] = "Sản phẩm đã được thêm vào giỏ hàng.";

            return RedirectToAction("Details", new { id = productId, succes= ViewData["success"] });
        }


        private bool IsPasswordValid(string password)
        {
            var regex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$");
            return regex.IsMatch(password);
        }
        public ActionResult TaoTK(string username, string name, string email, string password, string confirmPassword,string sdt,string dc)
        {
            if (String.IsNullOrEmpty(username))
            {
                ViewData["Err4"] = "Tên đăng nhập không được để trống";
                ViewData["Err3"] = "Đăng ký thất bại bấm vào nút đăng ký để xem lý do";
            }
            else if (String.IsNullOrEmpty(name))
            {
                ViewData["Err6"] = "Họ tên không được để trống";
                ViewData["Err3"] = "Đăng ký thất bại bấm vào nút đăng ký để xem lý do";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Err7"] = "Email không được để trống";
                ViewData["Err3"] = "Đăng ký thất bại bấm vào nút đăng ký để xem lý do";
            }
            else if (String.IsNullOrEmpty(sdt))
            {
                ViewData["Err8"] = "số điện thoại không được để trống";
                ViewData["Err3"] = "Đăng ký thất bại bấm vào nút đăng ký để xem lý do";
            }
            else if (String.IsNullOrEmpty(dc))
            {
                ViewData["Err9"] = "Địa chỉ không được để trống";
                ViewData["Err3"] = "Đăng ký thất bại bấm vào nút đăng ký để xem lý do";
            }
            else if (String.IsNullOrEmpty(password))
            {
                ViewData["Errpass"] = "Mật khẩu không được để trống";
                ViewData["Err3"] = "Đăng ký thất bại bấm vào nút đăng ký để xem lý do";
            }
            else if (!IsPasswordValid(password))
            {
                ViewData["Errpass"] = "Mật khẩu phải 8 chũ cái trong đó có 1 kí tự viết hoa, 1 kí tự số và 1 kí tự đặc biệt.";
                ViewData["Err3"] = "Đăng ký thất bại bấm vào nút đăng ký để xem lý do";
            }
            else if (password != confirmPassword)
            {
                ViewData["Err5"] = "Mật khẩu không trùng khớp.";
                ViewData["Err3"] = "Đăng ký thất bại bấm vào nút đăng ký để xem lý do";
            }
            else
            {
                var nguoiDung = new NguoiDung()
                {
                    HoVaTen = name,
                    TenDangNhap = username,
                    Email = email,
                    MatKhau = password,
                    DiaChi = dc,
                    SoDienThoai = sdt
                };

                db.NguoiDungs.InsertOnSubmit(nguoiDung);

                db.SubmitChanges();
                ViewData["Err3"] = "Đăng ký thành công xin mời đăng nhập";
                return View("Login");
            }
            return View("Login");
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetCartItemCount()
        {
            int cartItemCount;
            NguoiDung nguoiDung = Session["TaiKhoan"] as NguoiDung;
            if (nguoiDung != null)
            {
                int userId = nguoiDung.MaNguoiDung;
                cartItemCount = db.GioHangs.Count(c => c.MaNguoiDung == userId);
            }
            else
            {
                cartItemCount = 0;
            }




            return Json(cartItemCount, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tenDN = collection["TenDN"];
            var matKhau = collection["Matkhau"];
            if (String.IsNullOrEmpty(tenDN))
            {
                ViewData["Err1"] = "Bạn chưa nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matKhau))
            {
                ViewData["Err2"] = "Phải nhập mật khẩu";
            }
            else
            {
                NguoiDung kh = db.NguoiDungs.SingleOrDefault(n => n.TenDangNhap == tenDN && n.MatKhau == matKhau);

                if (kh != null)
                {
                    ViewData["Err3"] = "Chúc mừng đăng nhập thành công";
                    Session["TaiKhoan"] = kh;
                    return Redirect("~/Home/Index");
                }
                else
                {
                    ViewData["Err3"] = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Remove("TaiKhoan");

            return RedirectToAction("Index"); 

        }
        public ActionResult Cart(string err)
        {
            if (err != null)
            {
                ViewData["Err"] = err;
            }
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = danhMucList;
            NguoiDung nguoiDung = Session["TaiKhoan"] as NguoiDung;
            int userId = nguoiDung.MaNguoiDung;
            var cart = db.GioHangs.Where(u=>u.MaNguoiDung == userId).ToList();
            decimal tongTien = cart.Sum(item => item.ThanhTien);

            ViewBag.TongTien = tongTien;
            return View(cart);
        }
        [AcceptVerbs("GET", "POST")]
        public ActionResult XoaGH (int id)
        {
            var sp = db.GioHangs.SingleOrDefault(u => u.MaGioHang == id);
            db.GioHangs.DeleteOnSubmit(sp);
            db.SubmitChanges();
            return RedirectToAction("Cart");
        }
        [AcceptVerbs("GET", "POST")]
        public ActionResult ThemGH(int id)
        {
            var sp = db.GioHangs.SingleOrDefault(u => u.MaGioHang == id);
            sp.SoLuong++;
            sp.ThanhTien = sp.SoLuong * sp.Gia;
            db.SubmitChanges();
            return RedirectToAction("Cart");
        }
        public ActionResult ThanhToanKhiNhanHang()
        {
            var nguoidung = Session["TaiKhoan"] as NguoiDung;
            int makh = nguoidung.MaNguoiDung;
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = danhMucList;

                List<GioHang> gioHangItems = GetGioHangItems();
            foreach (var gioHang in gioHangItems)
            {
                var sp = db.SanPhams.SingleOrDefault(u => u.MaSanPham == gioHang.MaSanPham);
                if (sp.SoLuongTon < gioHang.SoLuong)
                {
                    return RedirectToAction("Cart", new { err = "Không đủ hàng trong kho" });
                }
            }
            decimal tongTien = gioHangItems.Sum(item => item.ThanhTien);

            ViewBag.TongTien = tongTien;
            ViewData["success"] = "Thanh toán thành công đơn hàng sẽ giao tới cho quý khách trong vòng 3-5 ngày.";
            DonHang dh = new DonHang
            {
                MaDonHang = DateTime.Now.Ticks,
                NgayDatHang = DateTime.Now,
                MaNguoiDung = makh,
                TongTien = tongTien,
                DiaChiGiaoHang = nguoidung.DiaChi,
                TrangThai = "Thanh toán khi nhận hàng"
            };
            db.DonHangs.InsertOnSubmit(dh);
            db.SubmitChanges();
            foreach (var gioHang in gioHangItems)
            {
                ChiTietDonHang chiTietDonHang = new ChiTietDonHang
                {
                    MaDonHang = dh.MaDonHang,
                    MaSanPham = gioHang.MaSanPham,
                    SoLuong = gioHang.SoLuong,
                    Gia = gioHang.Gia,
                    Size = gioHang.KichThuoc,
                    Mau = gioHang.Mau
                };
                var sp = db.SanPhams.SingleOrDefault(u => u.MaSanPham == gioHang.MaSanPham);
                sp.SoLuongTon -= gioHang.SoLuong;

                db.ChiTietDonHangs.InsertOnSubmit(chiTietDonHang);
                db.SubmitChanges();
            }
            db.GioHangs.DeleteAllOnSubmit(gioHangItems);
            db.SubmitChanges();
            return View(gioHangItems);
        }
        public ActionResult DanhSachDonHang(int? page)
        {
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = danhMucList;
            var nguoidung = Session["TaiKhoan"] as NguoiDung;
            int makh = nguoidung.MaNguoiDung;
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            var danhSachDonHang = db.DonHangs.Where(u=>u.MaNguoiDung==makh).ToPagedList(pageNumber,pageSize);

            return View(danhSachDonHang);
        }
        public ActionResult ThongTin()
        {
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = danhMucList;
            var nguoidung = Session["TaiKhoan"] as NguoiDung;
            int makh = nguoidung.MaNguoiDung;
            var infor = db.NguoiDungs.SingleOrDefault(u=>u.MaNguoiDung==makh);
            return View(infor);
        }
        public ActionResult ChiTietDonHang(long maDonHang, int? page)
        {
            List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = danhMucList;
            int pageSize = 6;
            int pageNumber = (page ?? 1);

            var chiTietDonHang = (from ctdh in db.ChiTietDonHangs
                                  join sp in db.SanPhams on ctdh.MaSanPham equals sp.MaSanPham
                                  where ctdh.MaDonHang == maDonHang
                                  select new ChiTietDonHangViewModel
                                  {
                                      TenSanPham = sp.TenSanPham,
                                      SoLuong = ctdh.SoLuong,
                                      DonGia = ctdh.Gia,
                                      MaDonHang = ctdh.MaDonHang,
                                      Size = ctdh.Size,
                                      Mau = ctdh.Mau
                                  }).ToPagedList(pageNumber, pageSize);

            if (chiTietDonHang == null)
            {
                return HttpNotFound();
            }

            return View(chiTietDonHang);
        }

        public ActionResult ThanhToanQuaThe()
        {

            var nguoidung = Session["TaiKhoan"] as NguoiDung;
            int makh = nguoidung.MaNguoiDung;
            List<GioHang> gioHangItems = GetGioHangItems();
            foreach (var gioHang in gioHangItems)
            {
                var sp = db.SanPhams.SingleOrDefault(u => u.MaSanPham == gioHang.MaSanPham);
                if (sp.SoLuongTon < gioHang.SoLuong)
                {
                    return RedirectToAction("Cart", new { err = "Không đủ hàng trong kho" });
                }
            }

            decimal tongTien = gioHangItems.Sum(item => item.ThanhTien);

            DonHang dh = new DonHang
            {
                MaDonHang = DateTime.Now.Ticks,
                NgayDatHang = DateTime.Now,
                MaNguoiDung = makh,
                TongTien = tongTien,
                DiaChiGiaoHang = nguoidung.DiaChi,
                TrangThai = "Thanh toán online"
            };
            db.DonHangs.InsertOnSubmit(dh);
            db.SubmitChanges();
            var urlPayment = "";
            Session["MaDH"] = dh.MaDonHang;

            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve 
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY 
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma định danh merchant kết nối (Terminal Id)
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret Key


            VnPayLibrary vnpay = new VnPayLibrary();
            long amount = (long)dh.TongTien+50000;
            Debug.WriteLine(amount);
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            long madonhang = dh.MaDonHang;
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());

            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + madonhang);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", madonhang.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            urlPayment = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Redirect(urlPayment);
        }
        public ActionResult Return()
        {
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        long code = (long)Session["MaDH"];

                        List<DanhMucSanPham> danhMucList = db.DanhMucSanPhams.ToList();
                        ViewBag.DanhMucList = danhMucList;
        
                        List<GioHang> gioHangItems = GetGioHangItems();

                        decimal tongTien = gioHangItems.Sum(item => item.ThanhTien);

                        ViewBag.TongTien = tongTien + 50000;
                        ViewData["success"] = "Thanh toán thành công đơn hàng sẽ giao tới cho quý khách trong vòng 3-5 ngày.";
                        foreach (var gioHang in gioHangItems)
                        {
                            ChiTietDonHang chiTietDonHang = new ChiTietDonHang
                            {
                                MaDonHang = code,
                                MaSanPham = gioHang.MaSanPham,
                                SoLuong = gioHang.SoLuong,
                                Gia = gioHang.Gia,
                                Size = gioHang.KichThuoc,
                                Mau = gioHang.Mau

                            };

                            var sp = db.SanPhams.SingleOrDefault(u => u.MaSanPham == gioHang.MaSanPham);
                            sp.SoLuongTon -= gioHang.SoLuong;
                            db.ChiTietDonHangs.InsertOnSubmit(chiTietDonHang);
                            db.SubmitChanges();
                        }
                        db.GioHangs.DeleteAllOnSubmit(gioHangItems);
                        db.SubmitChanges();
                        return View(gioHangItems);
                    }
                    else
                    {

                        ViewBag.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                    }
                }
            }
            List<DanhMucSanPham> list = db.DanhMucSanPhams.ToList();
            ViewBag.DanhMucList = list;

            List<GioHang> gioHangItem = GetGioHangItems();

            decimal tong = gioHangItem.Sum(item => item.ThanhTien);


            ViewBag.TongTien = tong + 50000;
            ViewData["success"] = "Thanh toán Không thành công ";
            return View();
        }
        private List<GioHang> GetGioHangItems()
        {
            NguoiDung nguoiDung = Session["TaiKhoan"] as NguoiDung;
            int userId = nguoiDung.MaNguoiDung;

            var gioHangItems = db.GioHangs.Where(u => u.MaNguoiDung == userId).ToList();
            return gioHangItems;
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult GiamGH(int id)
        {
            var sp = db.GioHangs.SingleOrDefault(u => u.MaGioHang == id);
            sp.SoLuong--;
            sp.ThanhTien = sp.SoLuong * sp.Gia;
            db.SubmitChanges();
            return RedirectToAction("Cart");
        }

    }
}