using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API_Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Accountant")]
    public class ChayBangLuongController : ControllerBase
    {
        private readonly IChayBangLuongBusiness _business;

        public ChayBangLuongController(
            IChayBangLuongBusiness business)
        {
            _business = business;
        }

        // GET: api/ChayBangLuong
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_business.GetAll());
        }

        // GET: api/ChayBangLuong/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // GET: api/ChayBangLuong/search?keyword=abc
        [HttpGet("search")]
        public IActionResult Search(string keyword)
        {
            return Ok(_business.Search(keyword));
        }

        // POST
        [HttpPost]
        public IActionResult Create([FromBody] ChayBangLuong chayBangLuong)
        {
            var result = _business.Create(chayBangLuong);

            if (result <= 0)
                return BadRequest();

            return Ok(result);
        }

        // PUT
        [HttpPut("{id}")]
        public IActionResult Update(
            string id,
            [FromBody] ChayBangLuong chayBangLuong)
        {
            chayBangLuong.Machayluong = id;

            var result = _business.Update(chayBangLuong);

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

        // PUT: api/ChayBangLuong/{id}/complete
        [HttpPut("{id}/complete")]
        public IActionResult Complete(string id)
        {
            var result = _business.Complete(id);

            if (result <= 0) return Conflict("Chỉ hoàn tất kỳ lương Draft sau khi đã tạo phiếu lương.");

            return Ok(result);
        }

        [HttpPost("{id}/generate")]
        public IActionResult GeneratePayslips(string id)
        {
            var run = _business.GetById(id);
            if (run == null) return NotFound("Không tìm thấy kỳ lương.");
            if (run.Trangthai != "Draft") return BadRequest("Chỉ có thể tính lương cho kỳ đang ở trạng thái Draft.");

            var inserted = _business.GeneratePayslips(id);
            return inserted <= 0
                ? BadRequest("Không tạo được phiếu lương. Hãy kiểm tra chính sách lương có hiệu lực trước ngày kết thúc kỳ.")
                : Ok(new { created = inserted });
        }
    }
}
