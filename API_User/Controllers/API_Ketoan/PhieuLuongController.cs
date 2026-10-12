using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API_User.Controllers.API_Ketoan
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Accountant,Employee")]
    public class PhieuLuongController : ControllerBase
    {
        private readonly IPhieuLuongBusiness _business;

        public PhieuLuongController(IPhieuLuongBusiness business)
        {
            _business = business;
        }

        // GET: api/PhieuLuong
        // Admin + Kế toán xem toàn bộ phiếu lương
        [HttpGet]
        [Authorize(Roles = "Admin,Accountant")]
        public IActionResult GetAll()
        {
            return Ok(_business.GetAll());
        }

        // GET: api/PhieuLuong/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Accountant")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // GET: api/PhieuLuong/chayluong/{maChayLuong}
        [HttpGet("chayluong/{maChayLuong}")]
        [Authorize(Roles = "Admin,Accountant")]
        public IActionResult GetByChayLuong(string maChayLuong)
        {
            return Ok(_business.GetByChayLuong(maChayLuong));
        }

        // GET: api/PhieuLuong/nguoidung/{maNguoiDung}
        [HttpGet("nguoidung/{maNguoiDung}")]
        [Authorize(Roles = "Admin,Accountant")]
        public IActionResult GetByNguoiDung(string maNguoiDung)
        {
            return Ok(_business.GetByNguoiDung(maNguoiDung));
        }

        // POST: api/PhieuLuong
        [HttpPost]
        [Authorize(Roles = "Admin,Accountant")]
        public IActionResult Create([FromBody] PhieuLuong model)
        {
            if (model == null)
                return BadRequest();

            var result = _business.Create(model);

            if (result <= 0)
                return BadRequest();

            return Ok(result);
        }

        // PUT: api/PhieuLuong
        [HttpPut]
        [Authorize(Roles = "Admin,Accountant")]
        public IActionResult Update([FromBody] PhieuLuong model)
        {
            if (model == null)
                return BadRequest();

            var result = _business.Update(model);

            if (result <= 0)
                return BadRequest();

            return Ok(result);
        }

        // DELETE: api/PhieuLuong/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Accountant")]
        public IActionResult Delete(string id)
        {
            var result = _business.Delete(id);

            if (result <= 0)
                return BadRequest();

            return Ok(result);
        }
    }
}
