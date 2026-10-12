using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Auth
{
    public class RefreshTokenRecord
    {
        public long MaRefreshToken { get; set; }
        public string Manguoidung { get; set; } = string.Empty;
        public string TokenHash { get; set; } = string.Empty;
        public DateTime Ngaytao { get; set; }
        public DateTime Ngayhethan { get; set; }
        public DateTime? Ngaythuhoi { get; set; }
    }
}