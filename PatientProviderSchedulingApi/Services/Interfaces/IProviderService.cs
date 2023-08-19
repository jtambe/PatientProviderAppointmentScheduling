using PatientProviderSchedulingApi.Models;

namespace PatientProviderSchedulingApi.Services.Interfaces
{
    public interface IProviderService
    {
        public Task<Provider> AddProvider(Provider provider);
    }
}
