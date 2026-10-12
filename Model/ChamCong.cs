using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ChamCong
    {
        public string Mabangchamcong { get; set; }
        public string Manguoidung { get; set; }
        public string Macongviec { get; set; }
        public DateTime Ngaylamviec { get; set; }
        public decimal? Sogiolam { get; set; }
        public string Trangthai { get; set; }
        public string Nguoiduyet { get; set; }
        public string LydoTuChoi { get; set; }
        public DateTime? Ngaytao { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
