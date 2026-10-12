using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API_Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Accountant")]
    public class ChinhSachLuongController : ControllerBase
    {
        private readonly IChinhSachLuongBusiness _business;

        public ChinhSachLuongController(
            IChinhSachLuongBusiness business)
        {
            _business = business;
        }

        // GET: api/ChinhSachLuong
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_business.GetAll());
        }

        // GET: api/ChinhSachLuong/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // GET: api/ChinhSachLuong/current
        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            var data = _business.GetCurrent();

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // GET: api/ChinhSachLuong/search?keyword=abc
        [HttpGet("search")]
        public IActionResult Search(string keyword)
        {
            return Ok(_business.Search(keyword));
        }

        // POST
        [HttpPost]
        public IActionResult Create([FromBody] ChinhSachLuong chinhSach)
        {
            var result = _business.Create(chinhSach);

            if (result <= 0)
                return BadRequest();

            return Ok(result);
        }

        // PUT
        [HttpPut("{id}")]
        public IActionResult Update(
            string id,
            [FromBody] ChinhSachLuong chinhSach)
        {
            chinhSach.Machinhsach = id;

            var result = _business.Update(chinhSach);

            if (result <= 0)
                return NotFound();

            return Ok(result);
        }

        // DELETE
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
