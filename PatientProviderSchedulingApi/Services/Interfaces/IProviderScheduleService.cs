using PatientProviderSchedulingApi.Models;

namespace PatientProviderSchedulingApi.Services.Interfaces
{
    public interface IProviderScheduleService
    {

        public Task<IEnumerable<ProviderSchedule>> AddProviderSchedule(IEnumerable<ProviderSchedule> providerSchedules);

        public Task<ProviderSchedule?> GetProviderScheduleForTheDay(int providerid, DateTime day);

    }
}
