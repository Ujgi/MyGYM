using Microsoft.AspNetCore.Identity; // IdentityUser için şart
using System.ComponentModel.DataAnnotations;

namespace MyGYM.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        // Hangi Hoca?
        public int TrainerId { get; set; }
        public Antrenor? Trainer { get; set; }

        // Hangi Üye? (Başlangıçta boş olabilir, o yüzden ?)
        public string? MemberId { get; set; }
        public IdentityUser? Member { get; set; }

        // Ne zaman?
        [Display(Name = "Randevu Tarihi")]
        public DateTime Date { get; set; }

        public bool IsApproved { get; set; } = false; // Varsayılan: Onaysız

        // Durumu (Dolu mu boş mu?)
        // Eğer MemberId null ise "Müsait", doluysa "Randevu Alındı" demektir.
        public bool IsAvailable => String.IsNullOrEmpty(MemberId);
    }
}