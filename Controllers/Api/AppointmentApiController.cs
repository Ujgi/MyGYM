using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGYM.Data;
using MyGYM.Models;

namespace MyGYM.Controllers.Api
{
    // Erişim Adresi: https://localhost:xxxx/api/appointment
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TÜM RANDEVULARI GETİR
        // GET: api/appointment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {


            return await _context.Appointments
                .Include(a => a.Trainer) // Randevunun hocasını da getir
                .ToListAsync();
        }

        // 2. ID'YE GÖRE RANDEVU GETİR
        // GET: api/appointment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Trainer) // Detayda hoca bilgisini de getir
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound("Randevu bulunamadı.");
            }

            return appointment;
        }

        // 3. FİLTRELEME: Belirli bir tarihteki randevuları getir
// Kullanımı: api/appointment/tarih?tarih=2023-12-20
        [HttpGet("tarih")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByDate(DateTime tarih)
        {
            // LINQ SORGUSU: Sadece gönderilen günün kayıtlarını getirir
            // (Saat farkını yok saymak için .Date kullanıyoruz)
            var randevular = await _context.Appointments
                .Include(a => a.Trainer)
                .Where(x => x.Date.Date == tarih.Date)
                .ToListAsync();

            if (!randevular.Any())
            {
                return NotFound("Bu tarihte kayıtlı randevu bulunamadı.");
            }

            return randevular;
        }
    }
}