using PatientProviderSchedulingApi.Models;
using System.Data.SqlClient;
using System.Data;
using Dapper.Contrib.Extensions;
using System.Text.Json;
using PatientProviderSchedulingApi.Repositories.Interfaces;

namespace PatientProviderSchedulingApi.Repositories
{
    public class ProviderRepository : IProviderRepository
    {

        private readonly string _connectionString;
        private readonly ILogger<ProviderRepository> _logger;

        public ProviderRepository(IConfiguration configuration, ILogger<ProviderRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<Provider> AddProvider(Provider provider)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();
                    provider.Id = await connection.InsertAsync<Provider>(provider);
                }
                return provider;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProviderService.AddProvider failed for provider: {provider}", JsonSerializer.Serialize(provider));
                throw;
            }
        }
    }
}
