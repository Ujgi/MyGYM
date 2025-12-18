using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGYM.Data;
using MyGYM.Models;

namespace MyGYM.Controllers
{
    [Authorize]
    public class AntrenorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AntrenorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. LİSTELEME (Index)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            // Veritabanındaki tüm antrenörleri listeye çevirip View'a gönder
            return View(await _context.Antrenors.ToListAsync());
        }

        // ==========================================
        // 2. EKLEME (Create)
        // ==========================================

        // Formu Göster (GET)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // Formu Kaydet (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Antrenor trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Listeye dön
            }
            return View(trainer); // Hata varsa formu tekrar göster
        }

        // ==========================================
        // 3. DÜZENLEME (Edit)
        // ==========================================

        // Düzenleme Formunu Göster (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Antrenors.FindAsync(id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // Değişiklikleri Kaydet (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Antrenor trainer)
        {
            if (id != trainer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Antrenors.Any(e => e.Id == trainer.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // ==========================================
        // 4. SİLME (Delete)
        // ==========================================

        // Silme Onay Ekranını Göster (GET)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Antrenors
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // Silme İşlemini Gerçekleştir (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Antrenors.FindAsync(id);
            if (trainer != null)
            {
                _context.Antrenors.Remove(trainer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}