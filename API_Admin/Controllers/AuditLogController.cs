using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Model;

namespace API_Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogBusiness _business;

        public AuditLogController(
            IAuditLogBusiness business)
        {
            _business = business;
        }

        // GET: api/AuditLog
        [HttpGet]
        public IActionResult GetAll() 
        {
            return Ok(_business.GetAll());
        }

        // GET: api/AuditLog/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            var data = _business.GetById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        // GET: api/AuditLog/nguoidung/{maNguoiDung}
        [HttpGet("nguoidung/{maNguoiDung}")]
        public IActionResult GetByNguoiDung(string maNguoiDung)
        {
            return Ok(_business.GetByNguoiDung(maNguoiDung));
        }

        // GET: api/AuditLog/search?keyword=abc
        [HttpGet("search")]
        public IActionResult Search(string keyword)
        {
            return Ok(_business.Search(keyword));
        }

        // POST: api/AuditLog
        [HttpPost]
        public IActionResult Create([FromBody] Audit_Log auditLog)
        {
            var result = _business.Create(auditLog);

            if (result <= 0)
                return BadRequest();

            return Ok(result);
        }
    }
}
