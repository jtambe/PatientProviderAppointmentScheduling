using PatientProviderSchedulingApi.Models;

namespace PatientProviderSchedulingApi.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        public Task<Reservation> AddReservation(Reservation reservation);
        public Task<bool> ConfirmReservation(Reservation reservation);
        public Task<IEnumerable<Reservation>> GetProviderReservationsForDay(int providerId, DateTime day);
    }
}
