namespace API_User.Models
{
    /// <summary>
    /// Dữ liệu nhân viên được phép nhập hoặc chỉnh sửa cho một timesheet.
    /// Thông tin chủ sở hữu, trạng thái và người duyệt luôn lấy từ hệ thống.
    /// </summary>
    public class ChamCongRequest
    {
        public string Macongviec { get; set; } = string.Empty;
        public DateTime Ngaylamviec { get; set; }
        public decimal Sogiolam { get; set; }
    }

    public class TuChoiChamCongRequest
    {
        public string? LydoTuChoi { get; set; }
    }
}
