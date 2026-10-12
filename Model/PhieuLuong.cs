using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class PhieuLuong
    {
        public string Maphieuluong { get; set; }
        public string Machayluong { get; set; }
        public string Manguoidung { get; set; }
        public string Machinhsach { get; set; }

        public decimal? Tonggiolam { get; set; }
        public decimal? Tonggiolamthem { get; set; }

        public decimal? Tongluong { get; set; }
        public decimal? Khoantru { get; set; }
        public decimal? Luongnhanduoc { get; set; }

        public DateTime? Ngaytao { get; set; }
    }
}
