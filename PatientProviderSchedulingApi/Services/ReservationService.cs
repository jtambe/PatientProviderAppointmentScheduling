using PatientProviderSchedulingApi.Models;
using PatientProviderSchedulingApi.Repositories.Interfaces;
using PatientProviderSchedulingApi.Services.Interfaces;
using System.Text.Json;

namespace PatientProviderSchedulingApi.Services
{
    public class ReservationService: IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IProviderScheduleService _providerScheduleService;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(IReservationRepository reservationRepository, IProviderScheduleService providerScheduleService, ILogger<ReservationService> logger)
        {
            _reservationRepository = reservationRepository;
            _providerScheduleService = providerScheduleService;
            _logger = logger;
        }

        public async Task<Reservation?> AddReservation(Reservation requestedReservation)
        {

            //cases to check
            // 1. check if requested slot endtime <= requested slot starttime
            // 2. Is provider scheduled for the day and time, that fits the requested slot? if not return null
            // 3. Is there another reservation that is confirmed
            // 4. Is there unconfirmed reservation but reservationTime was less than 30 minutes ago
            // 5. Reservation needs to be done 24 hours before
            // 6. Assuming that reservations are only allowed for 15 minutes

            try
            {
                var requestedReservationPossible = true;
                // case 1 & case 6
                if (requestedReservation.EndTime <= requestedReservation.StartTime || Math.Round((requestedReservation.EndTime - requestedReservation.StartTime).TotalMinutes) != 15)
                {
                    requestedReservationPossible = false;
                    return null;
                }

                // case 5
                if (Math.Round((requestedReservation.StartTime - DateTime.Now).TotalHours) <= 24)
                {
                    requestedReservationPossible = false;
                    return null;
                }

                var providerSchedule = await _providerScheduleService.GetProviderScheduleForTheDay(requestedReservation.ProviderId, requestedReservation.StartTime);
                // case 2
                if(providerSchedule is null ||
                    requestedReservation.StartTime < providerSchedule.StartTime || requestedReservation.StartTime > providerSchedule.EndTime ||
                    requestedReservation.EndTime > providerSchedule.EndTime || requestedReservation.EndTime < providerSchedule.StartTime)
                {
                    requestedReservationPossible = false;
                    return null;
                }
                else
                {
                    // case 3 & 4
                    var providerReservationsForDay = await GetProviderReservationsForDay(requestedReservation.ProviderId, requestedReservation.StartTime);
                    foreach (var providerReservation in providerReservationsForDay)
                    {
                        // is there overlap
                        if((requestedReservation.StartTime == providerReservation.StartTime && requestedReservation.EndTime == providerReservation.EndTime) ||
                           (requestedReservation.EndTime <= providerReservation.EndTime && requestedReservation.EndTime >= providerReservation.StartTime) ||
                           (requestedReservation.StartTime <= providerReservation.EndTime && requestedReservation.StartTime >= providerReservation.StartTime)
                           )
                        {
                            // case 3 where someone else has already confirmed the slot, Requested slot cannot be reserved
                            // does other person still have time to confirm slot or is it already successfully confirmed?
                            if (providerReservation.IsConfirmed) 
                            {
                                requestedReservationPossible = false;
                                break;
                            }

                            //case 4. Can someone still confirm the slot?
                            // if they still have time to confirm it, requested slot cannot be reserved
                            var tSpan = DateTime.Now - providerReservation.ReservationTime;
                            if (Math.Round(tSpan.TotalMinutes) <= 30)
                            {
                                requestedReservationPossible = false;
                                break;
                            }

                        }
                    }

                }
                return requestedReservationPossible == true ?  await _reservationRepository.AddReservation(requestedReservation) :  null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReservationService.AddReservation failed for requestedReservation: {requestedReservation}", JsonSerializer.Serialize(requestedReservation));
                throw;
            }
        }

        public async Task<bool> ConfirmReservation(Reservation reservation)
        {
            //cases to check
            // 1. Check if timespan.TotalMinutes <= 30
            try
            {
                var tSpan = DateTime.Now - reservation.ReservationTime;
                if(Math.Round(tSpan.TotalMinutes) <= 30)
                {
                    return await _reservationRepository.ConfirmReservation(reservation);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReservationService.ConfirmReservation failed for reservation: {reservation}", JsonSerializer.Serialize(reservation));
                throw;
            }
        }

        public async Task<IEnumerable<Reservation>> GetProviderReservationsForDay(int providerId, DateTime day)
        {
            try
            {
                return await _reservationRepository.GetProviderReservationsForDay(providerId, day);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReservationService.GetProviderReservationsForDay failed for providerId: {providerId} day: {day}", providerId, day);
                throw;
            }
        }
    }
}
