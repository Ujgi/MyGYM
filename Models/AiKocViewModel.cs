namespace MyGYM.Models
{
    public class AiKocViewModel
    {
        // Kullanıcıdan alacaklarımız
        public int Yas { get; set; }
        public int Boy { get; set; } // cm
        public int Kilo { get; set; } // kg
        public string Cinsiyet { get; set; } = "Belirtilmedi";
        public string Hedef { get; set; } // Kilo Alma, Verme, Kas vb.
        public string Aktivite { get; set; } // Hareketsiz, Orta, Aktif

        // Yapay Zekadan gelecek cevap
        public string? YapayZekaCevabi { get; set; }
    }
}