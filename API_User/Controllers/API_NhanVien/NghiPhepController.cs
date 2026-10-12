using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API_User.Controllers.API_NhanVien
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,PM,Employee")]
    public class NghiPhepController : ControllerBase
    {
        private readonly INghiPhepBusiness _business;

        public NghiPhepController(INghiPhepBusiness business)
        {
            _business = business;
        }

        // GET: api/NghiPhep
        // Admin và PM được xem toàn bộ
        [HttpGet]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetAll()
        {
            return Ok(_business.GetAll());
        }

        // GET: api/NghiPhep/{id}
        // Admin và PM được xem
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // GET: api/NghiPhep/nguoidung/{maNguoiDung}
        // Admin và PM được xem
        [HttpGet("nguoidung/{maNguoiDung}")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetByNguoiDung(string maNguoiDung)
        {
            return Ok(_business.GetByNguoiDung(maNguoiDung));
        }

        // GET: api/NghiPhep/pending
        // Chỉ Admin và PM được xem đơn đang chờ duyệt
        [HttpGet("pending")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetPending()
        {
            return Ok(_business.GetPending());
        }

        // GET: api/NghiPhep/search?keyword=abc
        // Admin và PM được tìm kiếm
        [HttpGet("search")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult Search(string keyword)
        {
            return Ok(_business.Search(keyword));
        }

        // POST: api/NghiPhep
        // Employee tạo đơn nghỉ phép
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult Create([FromBody] NghiPhep nghiPhep)
        {
            if (nghiPhep == null)
                return BadRequest();

            var result = _business.Create(nghiPhep);

            if (result <= 0)
                return BadRequest();

            return Ok(result);
        }

        // PUT: api/NghiPhep/{id}
        // Employee sửa đơn nghỉ phép
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee")]
        public IActionResult Update(
            string id,
            [FromBody] NghiPhep nghiPhep)
        {
            if (nghiPhep == null)
                return BadRequest();

            nghiPhep.Manghiphep = id;

            var result = _business.Update(nghiPhep);

            if (result <= 0)
                return NotFound();

            return Ok(result);
        }

        // DELETE: api/NghiPhep/{id}
        // Employee xóa đơn nghỉ phép
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")]
        public IActionResult Delete(string id)
        {
            var result = _business.Delete(id);

            if (result <= 0)
                return NotFound();

            return Ok(result);
        }

        // PUT: api/NghiPhep/{id}/approve
        // PM và Admin duyệt
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult Approve(
            string id,
            [FromQuery] string nguoiDuyet)
        {
            var result = _business.Approve(id, nguoiDuyet);

            if (result <= 0)
                return NotFound();

            return Ok(result);
        }

        // PUT: api/NghiPhep/{id}/reject
        // PM và Admin từ chối
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult Reject(
            string id,
            [FromQuery] string nguoiDuyet)
        {
            var result = _business.Reject(id, nguoiDuyet);

            if (result <= 0)
                return NotFound();

            return Ok(result);
        }
    }
}