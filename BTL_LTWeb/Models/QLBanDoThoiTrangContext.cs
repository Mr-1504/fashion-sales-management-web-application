using Microsoft.EntityFrameworkCore;

namespace BTL_LTWeb.Models;

public partial class QLBanDoThoiTrangContext : DbContext
{
    public QLBanDoThoiTrangContext() {}
    public QLBanDoThoiTrangContext(DbContextOptions<QLBanDoThoiTrangContext> options): base(options){}

    public virtual DbSet<TTag> Tags { get; set; }
    public virtual DbSet<TAnhChiTietSp> TAnhChiTietSps { get; set; }
    public virtual DbSet<TAnhSp> TAnhSps { get; set; }
    public virtual DbSet<TChiTietSanPham> TChiTietSanPhams { get; set; }
    public virtual DbSet<TDanhMucSp> TDanhMucSps { get; set; }
    public virtual DbSet<TChiTietHoaDonBan> TChiTietHoaDonBans { get; set; }
    public virtual DbSet<THoaDonBan> THoaDonBans { get; set; }
    public virtual DbSet<TKhachHang> TKhachHangs { get; set; }
    public virtual DbSet<TNhanVien> TNhanViens { get; set; }
    public virtual DbSet<TUser> TUsers { get; set; }
    public virtual DbSet<TDanhSachCuaHang> TDanhSachCuaHangs { get; set; }
    public virtual DbSet<TGioHang> TGioHangs { get; set; }

    public virtual DbSet<TGiaoHang> TGiaoHangs { get; set; }
    public virtual DbSet<TempUserOtp> TempUserOtps { get; set; }
    public virtual DbSet<TDanhGia> TDanhGias { get; set; }
    public virtual DbSet<TPhanHoi> TPhanHois { get; set; }
    public virtual DbSet<TMaGiamGia> TMaGiamGias { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TempUserOtp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TempUser__3214EC0793EDB907");

            entity.ToTable("TempUserOtp");

            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.OtpCode).HasMaxLength(6);
            entity.Property(e => e.OtpExpiration).HasColumnType("datetime");
        });
        modelBuilder.Entity<TAnhChiTietSp>(entity =>
        {
            entity.HasKey(e => new { e.MaChiTietSp, e.TenFileAnh }).HasName("PK__tAnhChiT__6DFA63355C4D9532");

            entity.ToTable("tAnhChiTietSP");

            entity.Property(e => e.MaChiTietSp).HasColumnName("MaChiTietSP");
            entity.Property(e => e.TenFileAnh).HasMaxLength(255);
            entity.Property(e => e.ViTri).HasMaxLength(255);

            entity.HasOne(d => d.ChiTietSanPham).WithMany(p => p.TAnhChiTietSps)
                .HasForeignKey(d => d.MaChiTietSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnhChiTietSP_ChiTietSP");
        });

        modelBuilder.Entity<TAnhSp>(entity =>
        {
            entity.HasKey(e => new { e.MaSp, e.TenFileAnh }).HasName("PK__tAnhSP__2FC2FB7E27DAAE4B");

            entity.ToTable("tAnhSP");

            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.TenFileAnh).HasMaxLength(255);
            entity.Property(e => e.ViTri).HasMaxLength(255);

            // Sửa chỗ này cho đúng với liên kết tới bảng tChiTietSanPham
            entity.HasOne(d => d.DangMucSp).WithMany(p => p.TAnhSps)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnhSP_ChiTietSanPham");
        });

        modelBuilder.Entity<TChiTietSanPham>(entity =>
        {
            entity.HasKey(e => e.MaChiTietSp).HasName("PK__tChiTiet__651D9057021AE26D");

            entity.ToTable("tChiTietSanPham");

            entity.Property(e => e.MaChiTietSp).HasColumnName("MaChiTietSP");
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.KichThuoc).HasMaxLength(50);
            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.MauSac).HasMaxLength(50);
            entity.Property(e => e.Slton).HasColumnName("SLTon");

            entity.HasOne(d => d.DanhMucSp).WithMany(p => p.TChiTietSanPhams)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietSP_DanhMucSP");
        });

        modelBuilder.Entity<TTag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__tTag__657CFD1C3A3D3A3D");

            entity.ToTable("TTag");

            entity.Property(e => e.TagId).HasColumnName("TagID");
            entity.Property(e => e.TagName).HasMaxLength(50);
            entity.Property(e => e.TagImage).HasColumnName("TagImage");
        });

        modelBuilder.Entity<TDanhMucSp>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("PK__tDanhMuc__2725081C34ED1B0E");

            entity.ToTable("tDanhMucSP");

            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.ChatLieu).HasMaxLength(100);
            entity.Property(e => e.Gia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GioiThieuSp).HasColumnName("GioiThieuSP");
            entity.Property(e => e.HangSx)
                .HasMaxLength(100)
                .HasColumnName("HangSX");
            entity.Property(e => e.LoaiDt)
                .HasMaxLength(100)
                .HasColumnName("LoaiDT");
            entity.Property(e => e.TenSp)
                .HasMaxLength(100)
                .HasColumnName("TenSP");
        });

        modelBuilder.Entity<TChiTietHoaDonBan>(entity =>
        {
            entity.HasKey(e => new { e.MaHoaDonBan, e.MaChiTietSP }).HasName("PK__tChiTiet__6A50CA8AF98C3478");
            entity.ToTable("tChiTietHoaDonBan");

            entity.Property(e => e.MaHoaDonBan).HasColumnName("MaHoaDonBan");
            entity.Property(e => e.MaChiTietSP).HasColumnName("MaChiTietSP");
            entity.Property(e => e.SoLuongBan).HasColumnName("SoLuongBan");
            entity.Property(e => e.DonGiaBan).HasColumnType("decimal(18, 2)").HasColumnName("DonGiaBan");

            entity.HasOne(d => d.DanhMucSP).WithMany()
                .HasForeignKey(d => d.MaChiTietSP)
                .HasConstraintName("FK_tChiTietHoaDonBan_tDanhMucSP");

            entity.HasOne(d => d.HoaDonBan).WithMany()
                .HasForeignKey(d => d.MaHoaDonBan)
                .HasConstraintName("FK_tChiTietHoaDonBan_tHoaDonBan");
        });

        modelBuilder.Entity<THoaDonBan>(entity =>
        {
            entity.HasKey(e => e.MaHoaDonBan).HasName("PK__tHoaDonBan");

            entity.ToTable("tHoaDonBan");

            entity.Property(e => e.MaHoaDonBan).HasColumnName("MaHoaDonBan");
            entity.Property(e => e.NgayHoaDon).HasColumnType("datetime");
            entity.Property(e => e.MaKhachHang).HasColumnName("MaKhachHang");
            entity.Property(e => e.TongTienHd).HasColumnType("decimal(18, 2)").HasColumnName("TongTienHD");
            entity.Property(e => e.MaGiamGia).HasColumnName("MaGiamGia");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(100);
            entity.Property(e => e.GhiChu).HasColumnType("nvarchar(max)");

            entity.HasOne(d => d.KhachHang).WithMany()
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK_HoaDonBan_KhachHang");

            entity.HasMany(d => d.TChiTietHoaDonBans).WithOne(p => p.HoaDonBan)
               .HasForeignKey(p => p.MaHoaDonBan)
               .OnDelete(DeleteBehavior.ClientSetNull)
               .HasConstraintName("FK_HoaDonBan_ChiTietHoaDonBan");

            entity.HasOne(d => d.GiamGia).WithMany()
                .HasForeignKey(d => d.MaGiamGia)
                .HasConstraintName("FK_HoaDonBan_MaGiamGia");

            entity.HasOne(h => h.GiaoHang)
            .WithOne(g => g.HoaDonBan)
            .HasForeignKey<TGiaoHang>(g => g.MaHoaDonBan)
            .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TMaGiamGia>(entity =>
        {
            entity.HasKey(e => e.MaGiamGia).HasName("PK_tMaGiamGia");

            entity.ToTable("tMaGiamGia");

            entity.Property(e => e.Code).HasColumnName("Code");
            entity.Property(e => e.TiLeGiam).HasColumnName("TiLeGiam");
            entity.Property(e => e.NgayBatDau).HasColumnName("NgayBatDau");
            entity.Property(e => e.NgayKetThuc).HasColumnName("NgayKetThuc");
            entity.Property(e => e.Mota).HasColumnName("Mota");
            entity.Property(e => e.TrangThai).HasColumnName("TrangThai");

        });

        modelBuilder.Entity<TKhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__tKhachHa__88D2F0E5693BB92D");

            entity.ToTable("tKhachHang");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TenKhachHang).HasMaxLength(100);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("Email");

            entity.HasOne(d => d.User).WithMany(p => p.TKhachHangs)
                .HasForeignKey(d => d.Email)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhachHang_User");
        });

        modelBuilder.Entity<TGioHang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__tGioHang__F5001DA30BA24EA8");

            entity.ToTable("tGioHang");

            entity.Property(e => e.MaKhachHang)
                .HasMaxLength(50)
                .HasColumnName("MaKhachHang");
            entity.Property(e => e.MaChiTietSP).HasColumnName("MaChiTietSP");
            entity.Property(e => e.SoLuong).HasColumnName("SoLuong");
        });

        modelBuilder.Entity<TNhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__tNhanVie__77B2CA474377FBA4");

            entity.ToTable("tNhanVien");

            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.TenNhanVien).HasMaxLength(100);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("Email");

            entity.HasOne(d => d.UsernameNavigation).WithMany(p => p.TNhanViens)
                .HasForeignKey(d => d.Email)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tNhanVien_tUser");
        });

        modelBuilder.Entity<TUser>(entity =>
        {
            entity.HasKey(e => e.Email).HasName("PK__tUser__F3DBC5736648EC50");

            entity.ToTable("tUser");

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("Email");
            entity.Property(e => e.LoaiUser).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50).HasColumnName("password");
            entity.Property(e => e.Salt).HasMaxLength(50);
        });
        modelBuilder.Entity<TDanhSachCuaHang>(entity =>
        {
            entity.HasKey(e => e.SDTCuaHang).HasName("PK_TDanhSachCuaHang");

            entity.ToTable("tDanhSachCuaHang");

            entity.Property(e => e.SDTCuaHang)
                .HasColumnName("SDTCuaHang")
                .HasMaxLength(10)
                .ValueGeneratedNever()
                .IsRequired();

            entity.Property(e => e.DiaChi)
                .HasColumnName("DiaChi")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.KhuVuc)
                .HasColumnName("KhuVuc")
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.KinhDo)
                .HasColumnName("KinhDo")
                .HasColumnType("float")
                .IsRequired();

            entity.Property(e => e.ViDo)
                .HasColumnName("ViDo")
                .HasColumnType("float")
                .IsRequired();
        });
        modelBuilder.Entity<TGiaoHang>(entity =>
        {
            entity.HasKey(e => e.MaGiaoHang).HasName("PK_TGiaoHang");
            entity.ToTable("tGiaoHang");

            entity.Property(e => e.MaGiaoHang)
                .HasColumnName("MaGiaoHang")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.MaHoaDonBan)
                .HasColumnName("MaHoaDonBan")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.ThanhPho)
                .HasMaxLength(100)
                .HasColumnName("ThanhPho")
                .IsRequired();

            entity.Property(e => e.QuanHuyen)
                .HasMaxLength(100)
                .HasColumnName("QuanHuyen")
                .IsRequired();

            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .HasColumnName("DiaChi")
                .IsRequired();

            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(20)
                .HasColumnName("SoDienThoai")
                .IsRequired();

            entity.Property(e => e.HoTenNguoiNhan)
                .HasMaxLength(100)
                .HasColumnName("HoTenNguoiNhan")
                .IsRequired();
            entity.HasOne(g => g.HoaDonBan)
            .WithOne(h => h.GiaoHang)
            .HasForeignKey<TGiaoHang>(g => g.MaHoaDonBan)
            .OnDelete(DeleteBehavior.ClientSetNull);
        });

        //sao các a làm dài thế, build giúp e với
        modelBuilder.Entity<TDanhGia>(entity =>
        {
            entity.HasKey(d => d.MaDanhGia);
            entity.ToTable("tDanhGia");
        });
        modelBuilder.Entity<TPhanHoi>(entity =>
        {
            entity.HasKey(e => new { e.MaKhachHang, e.MaDanhGia });
            entity.ToTable("tPhanHoi");
        });

       

    }
}
