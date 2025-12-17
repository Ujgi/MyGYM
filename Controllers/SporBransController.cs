using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyGYM.Data;
using MyGYM.Models;

namespace MyGYM.Controllers
{
    [Authorize(Roles = "Admin")] // Sadece Admin yönetebilir
    public class SporBransController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SporBransController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. LİSTELEME (Index)
        public async Task<IActionResult> Index()
        {
            return View(await _context.SporBranslar.ToListAsync());
        }

        // 2. EKLEME SAYFASI (Create - GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. EKLEME İŞLEMİ (Create - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SporBrans service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        // 4. SİLME SAYFASI (Delete - GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var service = await _context.SporBranslar.FirstOrDefaultAsync(m => m.Id == id);
            if (service == null) return NotFound();

            return View(service);
        }

        // 5. SİLME İŞLEMİ (Delete - POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.SporBranslar.FindAsync(id);
            if (service != null)
            {
                _context.SporBranslar.Remove(service);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}