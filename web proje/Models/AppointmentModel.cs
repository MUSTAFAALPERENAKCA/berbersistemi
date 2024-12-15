using System.ComponentModel.DataAnnotations;

namespace BarberShopProject.Models
{
    public class Appointment
    {
        [Key]
        public int? Id { get; set; } // Randevu için benzersiz kimlik (primary key).

        public DateTime Date { get; set; } // Randevunun tarihi ve saati.

        public Barber? Barber { get; set; } // Randevuya atanmış berberin bilgileri (navigation property).

        public int? BarberId { get; set; } // İlgili berberin kimliği (foreign key).

        public Service? Service { get; set; } // Randevuda alınacak hizmet bilgisi (navigation property).

        public int? ServiceId { get; set; } // İlgili hizmetin kimliği (foreign key).

        public Customer? Customer { get; set; } // Randevuyu alan müşterinin bilgileri (navigation property).

        public string CustomerId { get; set; } // Müşterinin kimliği (foreign key).
    }
}