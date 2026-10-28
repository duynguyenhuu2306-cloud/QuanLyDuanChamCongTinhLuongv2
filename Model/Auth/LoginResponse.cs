using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Auth
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string MaNguoiDung { get; set; }
        public string Tendangnhap { get; set; }
        public string VaiTro { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
