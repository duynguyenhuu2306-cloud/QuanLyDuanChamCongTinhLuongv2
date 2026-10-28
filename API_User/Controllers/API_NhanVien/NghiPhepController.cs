using System.Security.Claims;
using System.Text;
using API_User.Models;
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
        public NghiPhepController(INghiPhepBusiness business) => _business = business;

        [HttpGet]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetAll() => Ok(_business.GetAll());

        [HttpGet("all")]
        [Authorize(Roles = "Admin,PM,Employee")]
        public IActionResult GetAllForWorkspace() => Ok(User.IsInRole("Employee")
            ? _business.GetByNguoiDung(CurrentUserId())
            : _business.GetAll());

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);
            return data == null ? NotFound() : Ok(data);
        }

        [HttpGet("nguoidung/{maNguoiDung}")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetByNguoiDung(string maNguoiDung) => Ok(_business.GetByNguoiDung(maNguoiDung));

        [HttpGet("me")]
        [Authorize(Roles = "Employee")]
        public IActionResult GetMine() => Ok(_business.GetByNguoiDung(CurrentUserId()));

        [HttpGet("pending")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult GetPending() => Ok(_business.GetPending());

        [HttpGet("search")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult Search([FromQuery] string keyword = "") => Ok(_business.Search(keyword));

        [HttpGet("export/csv")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult ExportCsv([FromQuery] DateTime? tuNgay, [FromQuery] DateTime? denNgay,
            [FromQuery] string? loai = null, [FromQuery] string? trangThai = null)
        {
            var records = _business.GetAll().AsEnumerable();
            if (tuNgay.HasValue) records = records.Where(x => x.Ngaybatdau.Date >= tuNgay.Value.Date);
            if (denNgay.HasValue) records = records.Where(x => x.Ngaybatdau.Date <= denNgay.Value.Date);
            if (!string.IsNullOrWhiteSpace(loai)) records = records.Where(x => x.Loainghiphep == loai);
            if (!string.IsNullOrWhiteSpace(trangThai)) records = records.Where(x => x.Trangthai == trangThai);

            var csv = new StringBuilder();
            csv.AppendLine("MaDon,MaNguoiDung,Loai,NgayBatDau,NgayKetThuc,SoGio,LyDo,TrangThai,NguoiDuyet,LyDoTuChoi");
            foreach (var item in records)
            {
                csv.AppendLine(string.Join(",", new[]
                {
                    Csv(item.Manghiphep), Csv(item.Manguoidung), Csv(item.Loainghiphep),
                    Csv(item.Ngaybatdau.ToString("yyyy-MM-dd")), Csv(item.Ngayketthuc.ToString("yyyy-MM-dd")),
                    Csv(item.Sogiolam?.ToString("0.##") ?? ""), Csv(item.Lydo), Csv(item.Trangthai),
                    Csv(item.Nguoiduyet), Csv(item.LydoTuChoi)
                }));
            }

            var content = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
            return File(content, "text/csv; charset=utf-8", $"bao-cao-nghi-ot-{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public IActionResult Create([FromBody] NghiPhepRequest request)
        {
            if (request == null) return BadRequest();
            var entry = ToEntity(request, CurrentUserId(), $"NP-{Guid.NewGuid():N}");
            var result = _business.Create(entry);
            return result <= 0
                ? BadRequest("Đơn không hợp lệ: nghỉ không được trùng ngày; OT chỉ một ngày và tổng OT tối đa 4 giờ/ngày.")
                : CreatedAtAction(nameof(GetMine), entry);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Employee")]
        public IActionResult Update(string id, [FromBody] NghiPhepRequest request)
        {
            if (request == null) return BadRequest();
            var current = _business.GetById(id);
            if (current == null) return NotFound();
            if (current.Manguoidung != CurrentUserId()) return Forbid();

            var entry = ToEntity(request, current.Manguoidung, id);
            var result = _business.Update(entry);
            return result <= 0
                ? BadRequest("Chỉ được sửa đơn Pending/Rejected hợp lệ; khi gửi lại, đơn sẽ ở trạng thái Pending.")
                : Ok(entry);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")]
        public IActionResult Delete(string id)
        {
            var current = _business.GetById(id);
            if (current == null) return NotFound();
            if (current.Manguoidung != CurrentUserId()) return Forbid();
            return _business.Delete(id) <= 0
                ? BadRequest("Chỉ được xóa đơn Pending hoặc Rejected.")
                : NoContent();
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult Approve(string id)
        {
            var reviewer = CurrentUserId();
            if (!User.IsInRole("Admin") && !_business.CanReviewerReview(id, reviewer)) return Forbid();
            return _business.Approve(id, reviewer) <= 0 ? NotFound() : Ok();
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin,PM")]
        public IActionResult Reject(string id, [FromBody] TuChoiNghiPhepRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.LydoTuChoi))
                return BadRequest("Cần nhập lý do từ chối.");
            var reviewer = CurrentUserId();
            if (!User.IsInRole("Admin") && !_business.CanReviewerReview(id, reviewer)) return Forbid();
            return _business.Reject(id, reviewer, request.LydoTuChoi.Trim()) <= 0 ? NotFound() : Ok();
        }

        private static NghiPhep ToEntity(NghiPhepRequest request, string userId, string id) => new()
        {
            Manghiphep = id,
            Manguoidung = userId,
            Loainghiphep = request.Loainghiphep?.Trim() == "Nghi" ? "Leave" : request.Loainghiphep?.Trim(),
            Ngaybatdau = request.Ngaybatdau,
            Ngayketthuc = request.Ngayketthuc,
            Sogiolam = request.Sogiolam,
            Lydo = request.Lydo?.Trim()
        };

        private string CurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) throw new UnauthorizedAccessException("Token không có mã người dùng.");
            return userId;
        }

        private static string Csv(string value) => $"\"{(value ?? string.Empty).Replace("\"", "\"\"")}\"";
    }
}
