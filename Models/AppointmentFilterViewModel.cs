using MyGYM.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyGYM.Models // Namespace'ini projene göre ayarla
{
    public class AppointmentFilterViewModel
    {
        // Filtrelenmiş randevuları tutacak liste
        public List<Appointment> Appointments { get; set; }

        // Dropdown (Açılır Kutu) için Hoca listesi
        public SelectList Trainers { get; set; }

        // Kullanıcının seçtiği filtre değerleri (Formda seçili kalsın diye)
        public int? SelectedTrainerId { get; set; }
        public DayOfWeek? SelectedDay { get; set; }
    }
}