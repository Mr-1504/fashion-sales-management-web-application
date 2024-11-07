using System;
using System.Collections.Generic;

namespace BTL_LTWeb.Models;

public partial class TAnhChiTietSp
{
    public int MaChiTietSp { get; set; }

    public string TenFileAnh { get; set; } = null!;

    public string? ViTri { get; set; }

    public virtual TChiTietSanPham ChiTietSanPham { get; set; } = null!;
}