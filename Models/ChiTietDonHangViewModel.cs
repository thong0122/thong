using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanQuanAo.Models
{
    public class ChiTietDonHangViewModel
    {
        public long MaDonHang { get; set; }
        public string TenSanPham { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public string Size { get; set; }
        public string Mau { get; set; }
    }
}