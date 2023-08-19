using PatientProviderSchedulingApi.Models;

namespace PatientProviderSchedulingApi.Services.Interfaces
{
    public interface IReservationService
    {
        public Task<Reservation?> AddReservation(Reservation reservation);
        public Task<bool> ConfirmReservation(Reservation reservation);
        public Task<IEnumerable<Reservation>> GetProviderReservationsForDay(int providerId, DateTime day);
    }
}
