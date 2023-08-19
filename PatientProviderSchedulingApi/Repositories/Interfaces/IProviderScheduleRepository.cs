using PatientProviderSchedulingApi.Models;

namespace PatientProviderSchedulingApi.Repositories.Interfaces
{
    public interface IProviderScheduleRepository
    {
        public Task<IEnumerable<ProviderSchedule>> AddProviderSchedule(IEnumerable<ProviderSchedule> providerSchedules);

        public Task<ProviderSchedule?> GetProviderScheduleForTheDay(int providerid, DateTime day);
    }
}
