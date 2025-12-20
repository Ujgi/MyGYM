using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGYM.Data;
using MyGYM.Models;

namespace MyGYM.Controllers.Api
{
    // Erişim Adresi: https://localhost:xxxx/api/sporbrans
    [Route("api/sporbrans")]
    [ApiController]
    public class SporBransApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SporBransApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TÜM BRANŞLARI GETİR
        // GET: api/sporbrans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SporBrans>>> GetSporBranslar()
        {
            return await _context.SporBranslar.ToListAsync();
        }

        // 2. ID'YE GÖRE TEK BİR BRANŞ GETİR
        // GET: api/sporbrans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SporBrans>> GetSporBrans(int id)
        {
            var brans = await _context.SporBranslar.FindAsync(id);

            if (brans == null)
            {
                return NotFound("Aradığınız spor branşı bulunamadı.");
            }

            return brans;
        }
    }
}