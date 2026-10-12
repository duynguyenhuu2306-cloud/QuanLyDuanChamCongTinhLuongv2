using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using Model.Model;
using Microsoft.AspNetCore.Authorization;
namespace API_User.Controllers.API_PM
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,PM")]
    public class DuAnController : ControllerBase
    {
        private readonly IDuAnBusiness _business;

        public DuAnController(IDuAnBusiness business)
        {
            _business = business;
        }

        // GET: api/DuAn
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_business.GetAll());
        }

        // GET: api/DuAn/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // GET: api/DuAn/paging?pageNumber=1&pageSize=10
        [HttpGet("paging")]
        public IActionResult GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
                pageNumber = 1;

            if (pageSize < 1)
                pageSize = 10;

            var data = _business.GetPaged(pageNumber, pageSize);

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

        // GET: api/DuAn/search?keyword=test&pageNumber=1&pageSize=10
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

        // POST: api/DuAn
        [HttpPost]
        public IActionResult Create([FromBody] DuAn duAn)
        {
            var result = _business.Create(duAn);

            if (result <= 0)
                return BadRequest();

            return Ok(result);
        }

        // PUT: api/DuAn/{id}
        [HttpPut("{id}")]
        public IActionResult Update(
            string id,
            [FromBody] DuAn duAn)
        {
            duAn.Maduan = id;

            var result = _business.Update(duAn);

            if (result <= 0)
                return NotFound();

            return Ok(result);
        }

        // DELETE: api/DuAn/{id}
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
