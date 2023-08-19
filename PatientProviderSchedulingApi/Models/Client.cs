using Dapper.Contrib.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace PatientProviderSchedulingApi.Models
{
    [Table("Client")]
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [SwaggerSchema(Nullable = false)]
        public string FirstName { get; set; } = string.Empty;
        [SwaggerSchema(Nullable = false)]
        public string LastName { get; set; } = string.Empty;
        [SwaggerSchema(Nullable = false)]
        public string ClientEmail { get; set; } = string.Empty;
        [SwaggerSchema(Nullable = false)]
        public string ClientPhone { get; set; } = string.Empty;
    }
}
