using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Auth
{
    public class Verify2FARequest
    {
        public string Tendangnhap { get; set; }
        public string OTP { get; set; }
    }
}
