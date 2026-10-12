    using BLL.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Model.Auth;
    using System.Security.Claims;

    namespace API_Core.Controllers
    {
        [ApiController]
        [Route("api/core/doi-mat-khau")]
        [Authorize]
        public class DoiMatKhauController : ControllerBase
        {
            private readonly IAuthBusiness _business;

            public DoiMatKhauController(IAuthBusiness business)
            {
                _business = business;
            }

            [HttpPut]
            public IActionResult DoiMatKhau(
                [FromBody] DoiMatKhauRequest request)
            {
                var maNguoiDung = User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

                if (string.IsNullOrEmpty(maNguoiDung))
                    return Unauthorized();

                int result = _business.DoiMatKhau(
                    maNguoiDung,
                    request
                );

                if (result == 0)
                {
                    return BadRequest(new
                    {
                        message = "Mật khẩu cũ không đúng hoặc đổi mật khẩu thất bại."
                    });
                }

                return Ok(new
                {
                    message = "Đổi mật khẩu thành công."
                });
            }
        }
    }