using Dapper.Contrib.Extensions;

namespace PatientProviderSchedulingApi.Models
{
    [Table("ProviderSchedule")]
    public class ProviderSchedule
    {
        [Key]
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
