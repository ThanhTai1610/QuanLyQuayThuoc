using System;
using System.Collections.Generic;

namespace QuanLyQuayThuoc.Models;

public partial class LichSuChatbot
{
    public int MaChat { get; set; }

    public int? MaKhachHang { get; set; }

    public string? CauHoi { get; set; }

    public string? CauTraLoi { get; set; }

    public DateTime? ThoiGian { get; set; }

    public virtual NguoiDung? MaKhachHangNavigation { get; set; }
}
