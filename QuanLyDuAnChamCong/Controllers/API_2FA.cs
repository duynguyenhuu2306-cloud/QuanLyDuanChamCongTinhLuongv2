using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;

namespace QuanLyDuAnChamCong.Controllers
{
    [ApiController]
    [Route("api/core/2fa")]
    public class API_2FA : ControllerBase
    {
        private readonly IAuthBusiness _business;

        public API_2FA(IAuthBusiness business)
        {
            _business = business;
        }

        [HttpPost("verify")]
        public IActionResult Verify(
            [FromBody] Verify2FARequest request)
        {
            var result = _business.Verify2FA(request);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}
