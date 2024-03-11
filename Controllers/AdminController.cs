using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebBanQuanAo.Models;

namespace WebBanQuanAo.Controllers
{
    public class AdminController : Controller
    {
        DbDataContext db = new DbDataContext();
        // GET: Admin
        public ActionResult Index(int? page)
        {
            int pageSize = 5; 
            int pageNumber = (page ?? 1);
            var sp = db.SanPhams.ToPagedList(pageNumber, pageSize);
            return View(sp);
        }
        public ActionResult ThemSP()
        {
            ViewBag.DanhMucList = new SelectList(db.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemSP(FormCollection form)
        {
            if (ModelState.IsValid)
            {
  
                SanPham sanPham = new SanPham();

                sanPham.TenSanPham = form["TenSanPham"];
                sanPham.MoTa = form["MoTa"];
                sanPham.Gia = decimal.Parse(form["Gia"]); 
                sanPham.SoLuongTon = int.Parse(form["SoLuongTon"]); 
                sanPham.MaDanhMuc = int.Parse(form["MaDanhMuc"]); 

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/img"), fileName);
                        if (!System.IO.File.Exists(path))
                        {
                            file.SaveAs(path);
                        }
                        sanPham.HinhAnh = fileName;
                    }
                }
                db.SanPhams.InsertOnSubmit(sanPham);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DanhMucList = new SelectList(db.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", form["MaDanhMuc"]);
            return View();
        }
        [HttpGet]
        public ActionResult SuaSP(int id)
        {

            var sanPham = db.SanPhams.SingleOrDefault(u=>u.MaSanPham==id);


            if (sanPham == null)
            {
                return HttpNotFound();
            }


            ViewBag.DanhMucList = new SelectList(db.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);

            return View(sanPham);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaSP(SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                var existingSanPham = db.SanPhams.SingleOrDefault(u => u.MaSanPham == sanPham.MaSanPham);

                if (existingSanPham != null)
                {
                    existingSanPham.TenSanPham = sanPham.TenSanPham;
                    existingSanPham.MoTa = sanPham.MoTa;
                    existingSanPham.Gia = sanPham.Gia;
                    existingSanPham.SoLuongTon = sanPham.SoLuongTon;
                    existingSanPham.MaDanhMuc = sanPham.MaDanhMuc;
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];

                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/img"), fileName);
                            if (!System.IO.File.Exists(path))
                            {
                                file.SaveAs(path);
                            }
                            existingSanPham.HinhAnh = fileName;
                        }
                    }


                    db.SubmitChanges();

                    return RedirectToAction("Index");
                }
            }


            ViewBag.DanhMucList = new SelectList(db.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }


        public ActionResult User(int? page)
        {
            int pageSize = 5; 
            int pageNumber = (page ?? 1); 
            var sp = db.NguoiDungs.ToPagedList(pageNumber, pageSize);
            return View(sp);
        }
        public ActionResult DM(int? page)
        {
            int pageSize = 5; 
            int pageNumber = (page ?? 1);
            var sp = db.DanhMucSanPhams.ToPagedList(pageNumber, pageSize);
            return View(sp);
        }
        public ActionResult ThemDM()
        {
   
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemDM(DanhMucSanPham danhMuc)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/img"), fileName);
                        file.SaveAs(path);
                        danhMuc.HinhAnh = fileName;
                    }
                }

                db.DanhMucSanPhams.InsertOnSubmit(danhMuc);
                db.SubmitChanges();

                return RedirectToAction("DM");
            }

            return View(danhMuc);
        }
        public ActionResult SuaDM(int id)
        {

            DanhMucSanPham danhMuc = db.DanhMucSanPhams.SingleOrDefault(dm => dm.MaDanhMuc == id);

            if (danhMuc == null)
            {

                return HttpNotFound();
            }

            return View(danhMuc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaDM(DanhMucSanPham danhMuc)
        {
            if (ModelState.IsValid)
            {

                DanhMucSanPham existingDanhMuc = db.DanhMucSanPhams.SingleOrDefault(dm => dm.MaDanhMuc == danhMuc.MaDanhMuc);

                if (existingDanhMuc == null)
                {
 
                    return HttpNotFound();
                }

                existingDanhMuc.TenDanhMuc = danhMuc.TenDanhMuc;

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
    
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/img"), fileName);
                        file.SaveAs(path);
                        danhMuc.HinhAnh = fileName;
                    }
                }

        
                db.SubmitChanges();

                return RedirectToAction("DM");
            }

          
            return View(danhMuc);
        }
        public ActionResult SuaDH(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DonHang donHang = db.DonHangs.SingleOrDefault(d => d.MaDonHang == id);

            if (donHang == null)
            {
                return HttpNotFound();
            }

 
            return View(donHang);
        }

        [HttpPost]
        public ActionResult SuaDH(DonHang donHang)
        {
            if (ModelState.IsValid)
            {
    
                DonHang existingDonHang = db.DonHangs.SingleOrDefault(d => d.MaDonHang == donHang.MaDonHang);

                if (existingDonHang != null)
                {
       
                    existingDonHang.TrangThai = donHang.TrangThai;

               
                    db.SubmitChanges();

                    return RedirectToAction("DH");
                }
            }

            return View(donHang);
        }
        public ActionResult CTDH(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DonHang donHang = db.DonHangs.SingleOrDefault(d => d.MaDonHang == id);

            if (donHang == null)
            {
                return HttpNotFound();
            }


            List<ChiTietDonHang> chiTietDonHangs = db.ChiTietDonHangs.Where(c => c.MaDonHang == id).ToList();

            List<SanPham> sanPhams = new List<SanPham>();
            foreach (var chiTiet in chiTietDonHangs)
            {
                SanPham sanPham = db.SanPhams.SingleOrDefault(s => s.MaSanPham == chiTiet.MaSanPham);
                if (sanPham != null)
                {
                    sanPhams.Add(sanPham);
                }
            }


            ViewBag.SanPhams = sanPhams;
            ViewBag.ChiTietDonHangs = chiTietDonHangs;

            return View(donHang);
        }

        public ActionResult XoaDM(int id)
        {
            DanhMucSanPham danhMuc = db.DanhMucSanPhams.SingleOrDefault(dm => dm.MaDanhMuc == id);

            if (danhMuc == null)
            {
                return HttpNotFound();
            }

  
            var sanPhams = db.SanPhams.Where(sp => sp.MaDanhMuc == id).ToList();

     
            foreach (var sanPham in sanPhams)
            {
 
                var chiTietGioHangs = db.GioHangs.Where(ct => ct.MaSanPham == sanPham.MaSanPham).ToList();
                var ctdh = db.ChiTietDonHangs.Where(ct => ct.MaSanPham == sanPham.MaSanPham).ToList();
               
                db.GioHangs.DeleteAllOnSubmit(chiTietGioHangs);
                db.ChiTietDonHangs.DeleteAllOnSubmit(ctdh);
              
                db.SanPhams.DeleteOnSubmit(sanPham);
            }

       
            db.DanhMucSanPhams.DeleteOnSubmit(danhMuc);


            db.SubmitChanges();

            return RedirectToAction("DM");
        }

        public ActionResult DeleteSP(int id)
        {


            var sanPhams = db.SanPhams.Where(sp => sp.MaSanPham == id).ToList();

            foreach (var sanPham in sanPhams)
            {
                var chiTietGioHangs = db.GioHangs.Where(ct => ct.MaSanPham == sanPham.MaSanPham).ToList();
                var ctdh = db.ChiTietDonHangs.Where(ct => ct.MaSanPham == sanPham.MaSanPham).ToList();
                db.GioHangs.DeleteAllOnSubmit(chiTietGioHangs);
                db.ChiTietDonHangs.DeleteAllOnSubmit(ctdh);
                db.SanPhams.DeleteOnSubmit(sanPham);
            }


            db.SubmitChanges();

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult SuaUser(int id)
        {
           
            var nguoiDung = db.NguoiDungs.SingleOrDefault(u => u.MaNguoiDung == id);

            if (nguoiDung == null)
            {
            
                return HttpNotFound();
            }

           
            return View(nguoiDung);
        }
        [HttpPost]
        public ActionResult SuaUser(NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
         
                var userToUpdate = db.NguoiDungs.SingleOrDefault(u => u.MaNguoiDung == nguoiDung.MaNguoiDung);

                if (userToUpdate != null)
                {
                  
                    userToUpdate.DiaChi = nguoiDung.DiaChi;
                    userToUpdate.SoDienThoai = nguoiDung.SoDienThoai;
                    userToUpdate.ChucVu = nguoiDung.ChucVu;
                    userToUpdate.HoVaTen = nguoiDung.HoVaTen;

                    db.SubmitChanges();

                    return RedirectToAction("User");
                }
                else
                {
                   
                    return HttpNotFound();
                }
            }

            return View(nguoiDung);
        }

       
        public ActionResult DH(int? page)
        {
            int pageSize = 5; 
            int pageNumber = (page ?? 1); 
            var sp = db.DonHangs.ToPagedList(pageNumber, pageSize);
            return View(sp);
        }
    }
}