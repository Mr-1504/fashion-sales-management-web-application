using System;
using System.Collections.Generic;

namespace BTL_LTWeb.Models;

public partial class TAnhSp
{
    public int MaSp { get; set; }

    public string TenFileAnh { get; set; } = null!;

    public string? ViTri { get; set; }

    public virtual TDanhMucSp DangMucSp { get; set; } = null!;
}
