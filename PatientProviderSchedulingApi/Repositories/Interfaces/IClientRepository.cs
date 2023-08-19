using PatientProviderSchedulingApi.Models;

namespace PatientProviderSchedulingApi.Repositories.Interfaces
{
    public interface IClientRepository
    {
        public Task<Client> AddClient(Client client);
    }
}
