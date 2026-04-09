using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyQuayThuoc.Models;

public partial class DanhMuc
{
    public int MaDanhMuc { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public string? Icon { get; set; }

    public string? MoTa { get; set; }
    public int? MaDanhMucCha { get; set; }
    public string? Slug { get; set; }
    public string? TrangThai { get; set; }
    public int? ThuTu { get; set; }
    
    public virtual ICollection<DanhMuc> DanhMucCon { get; set; } = new List<DanhMuc>();
    [ForeignKey("MaDanhMucCha")]
    public virtual DanhMuc? DanhMucCha { get; set; }
    public virtual ICollection<Thuoc> Thuocs { get; set; } = new List<Thuoc>();
}
