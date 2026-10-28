namespace API_User.Models
{
    /// <summary>Thông tin nhân viên được phép gửi cho đơn nghỉ hoặc tăng ca.</summary>
    public class NghiPhepRequest
    {
        /// <summary>Chỉ nhận "Leave" hoặc "OT", khớp CHECK constraint trong SQL.</summary>
        public string Loainghiphep { get; set; } = string.Empty;
        public DateTime Ngaybatdau { get; set; }
        public DateTime Ngayketthuc { get; set; }
        /// <summary>Bắt buộc với OT. Đơn nghỉ tự tính 8 giờ cho mỗi ngày làm việc.</summary>
        public decimal? Sogiolam { get; set; }
        public string Lydo { get; set; } = string.Empty;
    }

    public class TuChoiNghiPhepRequest
    {
        public string LydoTuChoi { get; set; } = string.Empty;
    }
}
