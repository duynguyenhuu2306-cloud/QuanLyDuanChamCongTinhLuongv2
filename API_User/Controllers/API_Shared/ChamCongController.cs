using System.Security.Claims;
using API_User.Models;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace API_User.Controllers.API_Shared
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,PM,Employee,Accountant")]
    public class ChamCongController : ControllerBase
    {
        private readonly IChamCongBusiness _business;

        public ChamCongController(IChamCongBusiness business) => _business = business;

        [HttpGet]
        [Authorize(Roles = "Admin,PM,Accountant")]
        public IActionResult GetAll() => Ok(_business.GetAll());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,PM,Accountant")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);
            return data == null ? NotFound() : Ok(data);
        }

        [HttpGet("nguoidung/{maNguoiDung}")]
        [Authorize(Roles = "Admin,PM,Accountant")]
        public IActionResult GetByNguoiDung(string maNguoiDung) => Ok(_business.GetByNguoiDung(maNguoiDung));

        [HttpGet("me")]
        [Authorize(Roles = "Employee")]
        public IActionResult GetMine([FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay)
        {
            var data = _business.GetByNguoiDung(CurrentUserId());
            if (tuNgay.HasValue) data = data.Where(x => x.Ngaylamviec.Date >= tuNgay.Value.Date);
            if (denNgay.HasValue) data = data.Where(x => x.Ngaylamviec.Date <= denNgay.Value.Date);
            return Ok(data);
        }

        [HttpGet("week")]
        [Authorize(Roles = "Employee")]
        public IActionResult GetMyWeek([FromQuery] DateTime? ngayTrongTuan)
        {
            var date = (ngayTrongTuan ?? DateTime.Today).Date;
            var from = date.AddDays(-(((int)date.DayOfWeek + 6) % 7));
            var to = from.AddDays(7);
            var data = _business.GetByNguoiDung(CurrentUserId())
                .Where(x => x.Ngaylamviec.Date >= from && x.Ngaylamviec.Date < to)
                .ToList();
            return Ok(new { tuNgay = from, denNgay = to.AddDays(-1), data, tongGio = data.Sum(x => x.Sogiolam ?? 0) });
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetPending() => Ok(_business.GetPending());

        [HttpGet("search")]
        [Authorize(Roles = "Admin,PM,Accountant")]
        public IActionResult Search([FromQuery] string keyword = "") => Ok(_business.Search(keyword));

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult Create([FromBody] ChamCongRequest request)
        {
            if (request == null) return BadRequest();

            var entry = new ChamCong
            {
                Mabangchamcong = $"CC-{Guid.NewGuid():N}",
                Manguoidung = CurrentUserId(),
                Macongviec = request.Macongviec,
                Ngaylamviec = request.Ngaylamviec,
                Sogiolam = request.Sogiolam
            };

            var result = _business.Create(entry);
            return result <= 0
                ? BadRequest("Timesheet phải thuộc công việc được giao và tổng giờ trong ngày không vượt quá 8 giờ.")
                : CreatedAtAction(nameof(GetMine), new { }, entry);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Employee")]
        public IActionResult Update(string id, [FromBody] ChamCongRequest request)
        {
            if (request == null) return BadRequest();

            var current = _business.GetById(id);
            if (current == null) return NotFound();
            if (current.Manguoidung != CurrentUserId()) return Forbid();

            current.Macongviec = request.Macongviec;
            current.Ngaylamviec = request.Ngaylamviec;
            current.Sogiolam = request.Sogiolam;
            var result = _business.Update(current);
            return result <= 0
                ? BadRequest("Chỉ có thể sửa timesheet Pending/Rejected hợp lệ, không vượt quá 8 giờ/ngày.")
                : Ok(current);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")]
        public IActionResult Delete(string id)
        {
            var current = _business.GetById(id);
            if (current == null) return NotFound();
            if (current.Manguoidung != CurrentUserId()) return Forbid();

            return _business.Delete(id) <= 0
                ? BadRequest("Chỉ có thể xóa timesheet Pending hoặc Rejected.")
                : NoContent();
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "PM,Admin")]
        public IActionResult Approve(string id)
        {
            var reviewer = CurrentUserId();
            if (!User.IsInRole("Admin") && !_business.CanReviewerReview(id, reviewer)) return Forbid();
            return _business.Approve(id, reviewer) <= 0 ? NotFound() : Ok();
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "PM,Admin")]
        public IActionResult Reject(string id, [FromBody] TuChoiChamCongRequest? request)
        {
            var reviewer = CurrentUserId();
            if (!User.IsInRole("Admin") && !_business.CanReviewerReview(id, reviewer)) return Forbid();
            return _business.Reject(id, reviewer, request?.LydoTuChoi) <= 0 ? NotFound() : Ok();
        }

        private string CurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) throw new UnauthorizedAccessException("Token không có mã người dùng.");
            return userId;
        }
    }
}
