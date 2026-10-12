using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API_User.Controllers.API_PM
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,PM")]
    public class CongViecController : ControllerBase
    {
        private readonly ICongViecBusiness _business;

        public CongViecController(ICongViecBusiness business)
        {
            _business = business;
        }

        // GET: api/CongViec
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_business.GetAll());
        }

        // GET: api/CongViec/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // GET: api/CongViec/duan/{maDuAn}
        [HttpGet("duan/{maDuAn}")]
        public IActionResult GetByDuAn(string maDuAn)
        {
            return Ok(_business.GetByDuAn(maDuAn));
        }

        // GET: api/CongViec/paging?pageNumber=1&pageSize=10
        [HttpGet("paging")]
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
        public IActionResult Create([FromBody] CongViec congViec)
        {
            var result = _business.Create(congViec);

            if (result <= 0)
                return BadRequest();

            return Ok(result);
        }

        // PUT: api/CongViec/{id}
        [HttpPut("{id}")]
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
        public IActionResult Delete(string id)
        {
            var result = _business.Delete(id);

            if (result <= 0)
                return NotFound();

            return Ok(result);
        }
    }
}
