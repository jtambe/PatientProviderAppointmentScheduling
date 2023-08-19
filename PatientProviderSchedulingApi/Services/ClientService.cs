using PatientProviderSchedulingApi.Models;
using PatientProviderSchedulingApi.Repositories.Interfaces;
using PatientProviderSchedulingApi.Services.Interfaces;
using System.Text.Json;

namespace PatientProviderSchedulingApi.Services
{
    public class ClientService : IClientService
    {

        private readonly IClientRepository _clientRepository;
        private readonly ILogger<ClientService> _logger;

        public ClientService(IClientRepository clientRepository, ILogger<ClientService> logger)
        {
            _clientRepository = clientRepository;
            _logger = logger;
        }

        public async Task<Client> AddClient(Client client)
        {
            try
            {
                return await _clientRepository.AddClient(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ClientService.AddClient failed for client: {client}", JsonSerializer.Serialize(client));
                throw;
            }
        }
    }
}
