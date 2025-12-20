namespace MyGYM.Models;
using System.ComponentModel.DataAnnotations;
public class Antrenor
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Ad Soyad")]
    [Required(ErrorMessage = "Ad Soyad zorunludur.")]
    [StringLength(100)]
    public string FullName { get; set; }

    [Display(Name = "Uzmanlık Alanı")]
    [Required(ErrorMessage = "Uzmanlık alanı belirtilmelidir.")]
    public string Expertise { get; set; } // Örn: Vücut Geliştirme



    public ICollection<SporBrans>? SporBranslar { get; set; }

    // Randevular eklendiğinde burayı açacağız:
    public ICollection<Appointment> Appointments { get; set; }
}
