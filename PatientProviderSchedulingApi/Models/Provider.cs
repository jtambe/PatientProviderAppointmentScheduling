using Dapper.Contrib.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace PatientProviderSchedulingApi.Models
{
    [Table("Provider")]
    public class Provider
    {
        [Key]
        public int Id { get; set; }
        [SwaggerSchema(Nullable = false)]
        public string FirstName { get; set; } = string.Empty;
        [SwaggerSchema(Nullable = false)]
        public string LastName { get; set; } = string.Empty;
        [SwaggerSchema(Nullable = false)]
        public string ProviderEmail { get; set; } = string.Empty;
        [SwaggerSchema(Nullable = false)]
        public string ProviderPhone { get; set; } = string.Empty;
    }
}
