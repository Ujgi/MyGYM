using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyGYM.Data;
using MyGYM.Models;

namespace MyGYM.Controllers
{
    [Authorize] // Herkes girebilir (Admin yönetir, Üye alır)
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AppointmentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. MÜSAİT RANDEVULARI LİSTELE
        public async Task<IActionResult> Index()
        {
            // Sadece boş olanları (Müsait) veya benim aldıklarımı getir
            // Admin hepsini görsün, Üye sadece boşları görsün mantığı da kurulabilir.
            // Şimdilik: Tüm randevuları hoca bilgisiyle getiriyoruz.
            var appointments = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Member)
                .OrderBy(a => a.Date)
                .ToListAsync();

            return View(appointments);
        }

        // 2. ADMİN: YENİ MÜSAİTLİK EKLE (Create)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Açılır kutu (Dropdown) için hoca listesini View'a gönder
            ViewData["TrainerId"] = new SelectList(_context.Antrenors, "Id", "FullName");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            // Admin sadece tarih ve hoca seçer, MemberId boştur.
            _context.Add(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 3. ÜYE: RANDEVU AL (Book)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null || !appointment.IsAvailable)
            {
                return NotFound(); // Zaten alınmışsa hata ver
            }

            // Şu anki giriş yapmış kullanıcının ID'sini al
            var user = await _userManager.GetUserAsync(User);

            appointment.MemberId = user.Id; // Randevuyu kullanıcıya kitle

            _context.Update(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // 4. İPTAL ET (Opsiyonel - Hem Admin Hem Üye için)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            // Sadece Admin veya Randevunun sahibi iptal edebilir
            if (appointment != null)
            {
                // Eğer Adminse veya Randevu benimse
                if (User.IsInRole("Admin") || appointment.MemberId == user.Id)
                {
                    appointment.MemberId = null; // Tekrar boşa çıkar
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}