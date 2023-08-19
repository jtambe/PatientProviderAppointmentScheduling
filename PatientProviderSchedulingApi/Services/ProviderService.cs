using PatientProviderSchedulingApi.Models;
using PatientProviderSchedulingApi.Repositories.Interfaces;
using PatientProviderSchedulingApi.Services.Interfaces;
using System.Text.Json;

namespace PatientProviderSchedulingApi.Services
{
    public class ProviderService : IProviderService
    {
       
        private readonly IProviderRepository _providerRepository;
        private readonly ILogger<ProviderService> _logger;

        public ProviderService(IProviderRepository providerRepository, ILogger<ProviderService> logger)
        {
            _providerRepository = providerRepository;
            _logger = logger;
        }

        public async Task<Provider> AddProvider(Provider provider)
        {
            try
            {
                return await _providerRepository.AddProvider(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProviderService.AddProvider failed for Provider: {provider}", JsonSerializer.Serialize(provider));
                throw;
            }
        }
    }
}
