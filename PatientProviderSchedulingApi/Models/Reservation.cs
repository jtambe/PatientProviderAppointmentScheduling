using Dapper.Contrib.Extensions;

namespace PatientProviderSchedulingApi.Models
{
    [Table("Reservation")]
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public int ClientId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime ReservationTime { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime? ConfirmationTime { get; set; }

    }
}
