using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGYM.Data;
using MyGYM.Models;

namespace MyGYM.Controllers.Api
{
    // Erişim Adresi: https://localhost:xxxx/api/antrenor
    [Route("api/antrenor")]
    [ApiController]
    public class AntrenorApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AntrenorApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TÜM ANTRENÖRLERİ GETİR
        // GET: api/antrenor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Antrenor>>> GetAntrenors()
        {

            return await _context.Antrenors.ToListAsync();
        }

        // 2. ID'YE GÖRE TEK BİR ANTRENÖR GETİR
        // GET: api/antrenor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Antrenor>> GetAntrenor(int id)
        {
            var antrenor = await _context.Antrenors.FindAsync(id);

            if (antrenor == null)
            {
                return NotFound("Aradığınız antrenör bulunamadı.");
            }

            return antrenor;
        }
        // 3. FİLTRELEME: İsim veya Uzmanlık alanına göre arama
// Kullanımı: api/antrenor/ara?kelime=Fitness
            [HttpGet("ara")]
        public async Task<ActionResult<IEnumerable<Antrenor>>> SearchAntrenor(string kelime)
        {
            if (string.IsNullOrEmpty(kelime))
            {
                return await _context.Antrenors.ToListAsync();
            }

            // Hem isme bakıyor hem de uzmanlık alanına.
            var result = await _context.Antrenors
                .Where(x => x.FullName.ToLower().Contains(kelime.ToLower()) ||
                            x.Expertise.ToLower().Contains(kelime.ToLower()))
                .ToListAsync();

            return result;
        }
    }
}