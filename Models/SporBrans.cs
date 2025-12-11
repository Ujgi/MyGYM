using System.ComponentModel.DataAnnotations;

namespace MyGYM.Models;

public class SporBrans
{
    [Key]
    public int Id { get; set; }

    [Display(Name = "Spor Branşı")]
    [Required(ErrorMessage = "Spor branş adı zorunludur.")]
    [StringLength(50, ErrorMessage = "Spor branş adı en fazla 50 karakter olabilir.")]
    public string Name { get; set; } // Örn: Pilates, Crossfit

    [Display(Name = "Süre (Dakika)")]
    [Required(ErrorMessage = "Süre bilgisi zorunludur.")]
    [Range(15, 180, ErrorMessage = "Süre 15 ile 180 dakika arasında olmalıdır.")]
    public int Duration { get; set; }

    [Display(Name = "Ücret (TL)")]
    [Required(ErrorMessage = "Ücret bilgisi zorunludur.")]
    [Range(0, 10000, ErrorMessage = "Geçerli bir ücret giriniz.")]
    public decimal Price { get; set; }

    [Display(Name = "Açıklama")]
    public string? Description { get; set; }

    public ICollection<Antrenor>? Antrenors { get; set; }
}