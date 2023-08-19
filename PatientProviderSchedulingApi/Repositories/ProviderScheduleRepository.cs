using Microsoft.Extensions.Configuration;
using PatientProviderSchedulingApi.Models;
using PatientProviderSchedulingApi.Repositories.Interfaces;
using System.Data.SqlClient;
using System.Data;
using Dapper.Contrib.Extensions;
using System.Text.Json;
using Dapper;

namespace PatientProviderSchedulingApi.Repositories
{
    public class ProviderScheduleRepository : IProviderScheduleRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ProviderScheduleRepository> _logger;

        public ProviderScheduleRepository(IConfiguration configuration, ILogger<ProviderScheduleRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<IEnumerable<ProviderSchedule>> AddProviderSchedule(IEnumerable<ProviderSchedule> providerSchedules)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();
                    foreach(var schedule in providerSchedules)
                    {
                        schedule.Id = await connection.InsertAsync<ProviderSchedule>(schedule);
                    }
                }
                return providerSchedules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProviderScheduleRepository.AddProviderSchedule failed for providerSchedules: {providerSchedules}", JsonSerializer.Serialize(providerSchedules));
                throw;
            }
        }

        public async Task<ProviderSchedule?> GetProviderScheduleForTheDay(int providerId, DateTime day)
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();
                var query = "SELECT * FROM ProviderSchedule WHERE ProviderId = @providerId AND CONVERT(date,StartTime) >= @start AND CONVERT(date,EndTime) <= @end";
                var schedule = await connection.QueryAsync<ProviderSchedule>(query, new { @providerId = providerId, @start = day.Date, @end = day.Date.AddDays(1)});
                return schedule.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProviderScheduleRepository.GetProviderScheduleForTheDay failed for providerId: {providerId} day: {day}", providerId, day);
                throw;
            }
        }
    }
}
