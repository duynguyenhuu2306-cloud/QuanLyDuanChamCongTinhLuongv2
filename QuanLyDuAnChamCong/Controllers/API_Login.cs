using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;

namespace QuanLyDuAnChamCong.Controllers
{
    [ApiController]
    [Route("api/core/login")]
    public class API_Login : ControllerBase
    {
        private readonly IAuthBusiness _business;

        public API_Login(IAuthBusiness business)
        {
            _business = business;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var result = _business.Login(request);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}
