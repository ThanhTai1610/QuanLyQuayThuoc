using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class HinhAnhThuoc
{
    public int MaHinh { get; set; }

    public int? MaThuoc { get; set; }

    public string? DuongDan { get; set; }

    public virtual Thuoc? MaThuocNavigation { get; set; }
}
