using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Model;
using System.Security.Claims;

namespace API_User.Controllers.API_PM
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,PM,Employee")]
    public class PhanCongController : ControllerBase
    {
        private readonly IPhanCongBusiness _business;

        public PhanCongController(IPhanCongBusiness business)
        {
            _business = business;
        }

        // GET: api/PhanCong
        [HttpGet]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetAll()
        {
            return Ok(_business.GetAll());
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,PM,Employee")]
        public IActionResult GetAllForWorkspace() => Ok(User.IsInRole("Employee")
            ? _business.GetByNguoiDung(CurrentUserId())
            : _business.GetAll());

        // GET: api/PhanCong/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // GET: api/PhanCong/congviec/{maCongViec}
        [HttpGet("congviec/{maCongViec}")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetByCongViec(string maCongViec)
        {
            return Ok(_business.GetByCongViec(maCongViec));
        }

        // GET: api/PhanCong/nguoidung/{maNguoiDung}
        [HttpGet("nguoidung/{maNguoiDung}")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetByNguoiDung(string maNguoiDung)
        {
            return Ok(_business.GetByNguoiDung(maNguoiDung));
        }

        // GET: api/PhanCong/paging?pageNumber=1&pageSize=10
        [HttpGet("paging")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 10;

            var data = _business.GetPaged(
                pageNumber,
                pageSize
            );

            int totalRecords = _business.Count(null);

            int totalPages = (int)Math.Ceiling(
                (double)totalRecords / pageSize
            );

            return Ok(new
            {
                data = data,
                pageNumber = pageNumber,
                pageSize = pageSize,
                totalRecords = totalRecords,
                totalPages = totalPages
            });
        }

        // GET: api/PhanCong/search?keyword=PC&pageNumber=1&pageSize=10
        [HttpGet("search")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult Search(
            [FromQuery] string keyword = "",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 10;

            var data = _business.SearchPaged(
                keyword,
                pageNumber,
                pageSize
            );

            int totalRecords = _business.Count(keyword);

            int totalPages = (int)Math.Ceiling(
                (double)totalRecords / pageSize
            );

            return Ok(new
            {
                data = data,
                keyword = keyword,
                pageNumber = pageNumber,
                pageSize = pageSize,
                totalRecords = totalRecords,
                totalPages = totalPages
            });
        }

        // POST: api/PhanCong
        [HttpPost]
        public IActionResult Create([FromBody] PhanCong phanCong)
        {
            if (phanCong == null)
                return BadRequest();

            if (!User.IsInRole("Admin") && !_business.IsProjectManagerForTask(phanCong.Macongviec, CurrentUserId()))
                return Forbid();

            var result = _business.Create(phanCong);

            if (result <= 0)
                return BadRequest();

            return Ok(result);
        }

        // PUT: api/PhanCong/{id}
        [HttpPut("{id}")]
        public IActionResult Update(
            string id,
            [FromBody] PhanCong phanCong)
        {
            if (phanCong == null)
                return BadRequest();

            var current = _business.GetById(id);
            if (current == null)
                return NotFound();

            if (!User.IsInRole("Admin") &&
                (!_business.IsProjectManagerForTask(current.Macongviec, CurrentUserId()) ||
                 !_business.IsProjectManagerForTask(phanCong.Macongviec, CurrentUserId())))
                return Forbid();

            phanCong.Ma_phancong = id;

            var result = _business.Update(phanCong);

            if (result <= 0)
                return NotFound();

            return Ok(result);
        }

        // DELETE: api/PhanCong/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var current = _business.GetById(id);
            if (current == null)
                return NotFound();

            if (!User.IsInRole("Admin") && !_business.IsProjectManagerForTask(current.Macongviec, CurrentUserId()))
                return Forbid();

            var result = _business.Delete(id);

            if (result <= 0)
                return NotFound();

            return Ok(result);
        }

        private string CurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("Token không có mã người dùng.");

            return userId;
        }
    }
}
