using PatientProviderSchedulingApi.Models;

namespace PatientProviderSchedulingApi.Services.Interfaces
{
    public interface IClientService
    {
        public Task<Client> AddClient(Client client);
    }
}
