using PatientProviderSchedulingApi.Models;
using PatientProviderSchedulingApi.Repositories.Interfaces;
using PatientProviderSchedulingApi.Services.Interfaces;
using System.Text.Json;

namespace PatientProviderSchedulingApi.Services
{
    public class ProviderScheduleService : IProviderScheduleService
    {

        private readonly IProviderScheduleRepository _providerScheduleRepository;
        private readonly ILogger<ProviderScheduleService> _logger;

        public ProviderScheduleService(IProviderScheduleRepository providerScheduleRepository, ILogger<ProviderScheduleService> logger)
        {
            _providerScheduleRepository = providerScheduleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ProviderSchedule>> AddProviderSchedule(IEnumerable<ProviderSchedule> providerSchedules)
        {

            try
            {
                var providerSchedulesLookup = providerSchedules.ToLookup(s => s.ProviderId);
                var providerIds = providerSchedules.Select(s => s.ProviderId).Distinct();

                foreach (var providerId in providerIds)
                {
                    var pSchedules = providerSchedulesLookup[providerId];
                    // Group ProviderSchedules by date
                    var groupedByDate = pSchedules.GroupBy(x => x.StartTime.Date);
                    // Iterate through groups and find dates with multiple items
                    foreach (var group in groupedByDate)
                    {
                        if (group.Count() > 1)
                        {
                            throw new Exception("providerschedules for a provider cannot contain multiple schedules for single day");
                        }
                    }
                }
                

                return await _providerScheduleRepository.AddProviderSchedule(providerSchedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProviderScheduleService.AddProviderSchedule failed for providerSchedules: {providerSchedules}", JsonSerializer.Serialize(providerSchedules));
                throw;
            }
        }

        public async Task<ProviderSchedule?> GetProviderScheduleForTheDay(int providerid, DateTime day)
        {

            try
            {
                return await _providerScheduleRepository.GetProviderScheduleForTheDay(providerid, day);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProviderScheduleService.GetProviderScheduleForTheDay failed for providerid: {providerid} day: {day}", providerid, day);
                throw;
            }
        }
    }
}
