using Dapper;
using Dapper.Contrib.Extensions;
using PatientProviderSchedulingApi.Models;
using PatientProviderSchedulingApi.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;

namespace PatientProviderSchedulingApi.Repositories
{
    public class ClientRepository : IClientRepository
    {

        private readonly string _connectionString;
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(IConfiguration configuration, ILogger<ClientRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<Client> AddClient(Client client)
        {
			try
			{

                using (var connection = CreateConnection())
                {
                    connection.Open();
                    client.Id = await connection.InsertAsync<Client>(client);
                }
                return client;
                
			}
			catch (Exception ex)
			{
                _logger.LogError(ex, "ReservationRepository.AddClient failed for client: {client}", JsonSerializer.Serialize(client));
                throw;
			}
        }
    }
}
