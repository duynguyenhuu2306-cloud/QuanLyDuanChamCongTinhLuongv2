using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using System.Security.Claims;

namespace API_User.Controllers.API_PM
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,PM,Employee")]
    public class CongViecController : ControllerBase
    {
        private readonly ICongViecBusiness _business;

        public CongViecController(ICongViecBusiness business)
        {
            _business = business;
        }

        // GET: api/CongViec
        [HttpGet]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetAll()
        {
            return Ok(_business.GetAll());
        }

        [HttpGet("summary")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetSummary()
        {
            var rows = _business.GetAll().ToList();
            return Ok(new
            {
                total = rows.Count,
                todo = rows.Count(x => x.Trangthai == "Todo"),
                inProgress = rows.Count(x => x.Trangthai == "InProgress"),
                done = rows.Count(x => x.Trangthai == "Done")
            });
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,PM,Employee")]
        public IActionResult GetAllForWorkspace() => Ok(User.IsInRole("Employee")
            ? _business.GetAssignedToUser(CurrentUserId())
            : _business.GetAll());

        private string CurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Token không có mã người dùng.");

        // GET: api/CongViec/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);

            if (data == null)
                return NotFound();

            if (User.IsInRole("Employee") && !_business.GetAssignedToUser(CurrentUserId()).Any(x => x.Macongviec == id))
                return Forbid();
            return Ok(data);
        }

        // GET: api/CongViec/duan/{maDuAn}
        [HttpGet("duan/{maDuAn}")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetByDuAn(string maDuAn)
        {
            return Ok(_business.GetByDuAn(maDuAn));
        }

        // GET: api/CongViec/paging?pageNumber=1&pageSize=10
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

        // GET: api/CongViec/search?keyword=test&pageNumber=1&pageSize=10
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

        // POST: api/CongViec
        [HttpPost]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult Create([FromBody] CongViec congViec)
        {
            var result = _business.Create(congViec);

            if (result <= 0)
                return BadRequest();

            return Ok(result);
        }

        // PUT: api/CongViec/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult Update(
            string id,
            [FromBody] CongViec congViec)
        {
            congViec.Macongviec = id;

            var result = _business.Update(congViec);

            if (result <= 0)
                return NotFound();

            return Ok(result);
        }

        // DELETE: api/CongViec/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult Delete(string id)
        {
            var result = _business.Delete(id);

            if (result <= 0)
                return NotFound();

            return Ok(result);
        }
    }
}
