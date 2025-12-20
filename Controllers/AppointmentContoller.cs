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

        // 1. MÜSAİT RANDEVULARI LİSTELE VE FİLTRELE
        public async Task<IActionResult> Index(int? trainerId, DayOfWeek? day)
        {
            // 1. Temel Sorgu (Henüz veritabanına gitmedi, hazırlık)
            // AsQueryable() diyerek sorguyu dinamik hale getiriyoruz.
            var query = _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Member)
                .AsQueryable();

            // 2. Hoca Filtresi: Eğer kullanıcı bir hoca seçtiyse sorguya ekle
            if (trainerId.HasValue)
            {
                query = query.Where(a => a.TrainerId == trainerId.Value);
            }

            // 3. Gün Filtresi: Eğer kullanıcı bir gün seçtiyse
            // SQL tarafında DayOfWeek fonksiyonunu çalıştırır.
            if (day.HasValue)
            {
                query = query.Where(a => a.Date.DayOfWeek == day.Value);
            }

            // 4. Sonuçları Çek (Şimdi veritabanına gider)
            var appointments = await query.OrderBy(a => a.Date).ToListAsync();

            // 5. Filtreleme Dropdown'ı için Hoca Listesini Hazırla
            var trainerList = new SelectList(await _context.Antrenors.ToListAsync(), "Id", "FullName", trainerId);

            var model = new AppointmentFilterViewModel
            {
                Appointments = appointments,
                Trainers = trainerList,
                SelectedTrainerId = trainerId,
                SelectedDay = day
            };

            return View(model);
        }

        // 2. ADMİN: YENİ DERS EKLE (Create)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Açılır kutu (Dropdown) için hoca listesini View'a gönder
            ViewData["TrainerId"] = new SelectList(_context.Antrenors, "Id", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment, int selectedHour)
        {


            appointment.Date = appointment.Date.Date.AddHours(selectedHour);

            if (appointment.Date <= DateTime.Now)
            {
                ModelState.AddModelError("", "Geçmiş bir tarih veya saate randevu açılamaz.");
            }

            if (appointment.Date.Hour < 9 || appointment.Date.Hour >= 22)
            {
                ModelState.AddModelError("", "Lütfen geçerli mesai saatleri seçiniz.");
            }

            // 3. ÇAKIŞMA KONTROLÜ
            bool isBooked = await _context.Appointments.AnyAsync(x =>
                x.TrainerId == appointment.TrainerId &&
                x.Date == appointment.Date);

            if (isBooked)
            {
                ModelState.AddModelError("", $"Seçtiğiniz antrenör saat {selectedHour}:00'da zaten dolu.");
            }

            // 4. KAYIT İŞLEMİ
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Hata varsa listeyi tekrar doldur
            ViewData["TrainerId"] = new SelectList(_context.Antrenors, "Id", "FullName", appointment.TrainerId);

            return View(appointment);
        }

        // 3. ÜYE: RANDEVU AL (Book)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);

            if (appointment != null && user != null)
            {
                // Randevuyu kullanıcıya ata
                appointment.MemberId = user.Id;

                // ÖNEMLİ: Randevu alındı ama henüz onaylanmadı!
                appointment.IsApproved = false;

                await _context.SaveChangesAsync();
            }
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

        // Adminin randevuyu onaylaması için
        [HttpPost]
        [Authorize(Roles = "Admin")] // Sadece Admin
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.IsApproved = true; // Onaylandı
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


    }
}