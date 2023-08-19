using PatientProviderSchedulingApi.Models;
using PatientProviderSchedulingApi.Repositories.Interfaces;
using System.Data.SqlClient;
using System.Data;
using Dapper.Contrib.Extensions;
using System.Text.Json;
using Dapper;

namespace PatientProviderSchedulingApi.Repositories
{
    public class ReservationRepository : IReservationRepository
    {

        private readonly ILogger<ReservationRepository> _logger;
        private readonly string _connectionString;

        public ReservationRepository(IConfiguration configuration, ILogger<ReservationRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<Reservation> AddReservation(Reservation reservation)
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();
                    reservation.ReservationTime = DateTime.Now;
                    reservation.Id = await connection.InsertAsync<Reservation>(reservation);
                }
                return reservation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReservationRepository.AddReservation failed for reservation: {reservation}", JsonSerializer.Serialize(reservation));
                throw;
            }
        }

        public async Task<bool> ConfirmReservation(Reservation reservation)
        {
            try
            {
                var rowsAffected = 0;
                using (var connection = CreateConnection())
                {
                    connection.Open();
                    // Since I am assuming that schedules are submitted in UTCTime from Browser/SwaggerUI, I'm also going to simply use GETDATE() as confirmationTime so Date Calculations stay correct.
                    var updateQuery = "UPDATE dbo.Reservation SET IsConfirmed = 1, COnfirmationTime = GETDATE() WHERE Id = @reservationId";
                    rowsAffected = await connection.ExecuteAsync(updateQuery, new { @reservationId = reservation.Id });
                }
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReservationRepository.ConfirmReservation failed for reservationId: {reservationId}", reservation.Id);
                throw;
            }
        }

        public async Task<IEnumerable<Reservation>> GetProviderReservationsForDay(int providerId, DateTime day)
        {
            try
            {
                using var connection = CreateConnection();
                connection.Open();
                var query = "SELECT * FROM Reservation WHERE ProviderId = @providerId AND CONVERT(date,StartTime) >= @start AND CONVERT(date,EndTime) <= @end";
                var reservationsForDay = await connection.QueryAsync<Reservation>(query, new { @providerId = providerId, @start = day.Date, @end = day.Date.AddDays(1) });
                return reservationsForDay;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReservationRepository.GetProviderReservationsForDay failed for providerId: {providerId} day: {day}", providerId, day);
                throw;

            }
        }
    }
}
