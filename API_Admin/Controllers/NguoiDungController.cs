using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API_Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class NguoiDungController : ControllerBase
    {
        private readonly INguoiDungBusiness _business;

        public NguoiDungController(INguoiDungBusiness business)
        {
            _business = business;
        }

        // GET: api/NguoiDung
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_business.GetAll());
        }

        // GET: api/NguoiDung/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // GET: api/NguoiDung/username/{username}
        [HttpGet("username/{username}")]
        public IActionResult GetByUsername(string username)
        {
            var data = _business.GetByUsername(username);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // GET: api/NguoiDung/search?keyword=abc
        [HttpGet("search")]
        public IActionResult Search(string keyword)
        {
            return Ok(_business.Search(keyword));
        }

        // POST: api/NguoiDung
        [HttpPost]
        public IActionResult Create([FromBody] NguoiDung nguoiDung)
        {
            var result = _business.Create(nguoiDung);

            if (result <= 0)
                return BadRequest();

            return Ok(result);
        }

        // PUT: api/NguoiDung/{id}
        [HttpPut("{id}")]
        public IActionResult Update(
            string id,
            [FromBody] NguoiDung nguoiDung)
        {
            nguoiDung.Manguoidung = id;

            var result = _business.Update(nguoiDung);

            if (result <= 0)
                return NotFound();

            return Ok(result);
        }

        // DELETE: api/NguoiDung/{id}
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