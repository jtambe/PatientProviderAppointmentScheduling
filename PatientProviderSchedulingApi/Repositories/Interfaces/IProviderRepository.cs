using PatientProviderSchedulingApi.Models;

namespace PatientProviderSchedulingApi.Repositories.Interfaces
{
    public interface IProviderRepository
    {
        public Task<Provider> AddProvider(Provider provider);
    }
}
