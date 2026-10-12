using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Auth
{
    public class TokenResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ? Token { get; set; }
        public string ? RefreshToken { get; set; }
    }
}
