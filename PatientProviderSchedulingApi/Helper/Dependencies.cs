using PatientProviderSchedulingApi.Repositories;
using PatientProviderSchedulingApi.Repositories.Interfaces;
using PatientProviderSchedulingApi.Services;
using PatientProviderSchedulingApi.Services.Interfaces;

namespace PatientProviderSchedulingApi.Helper
{
    public static class Dependencies
    {
        public static void AddAppDependencies(this IServiceCollection services)
        {
            services.AddTransient<IReservationService, ReservationService>();
            services.AddTransient<IProviderScheduleService, ProviderScheduleService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IProviderService, ProviderService>();

            services.AddTransient<IReservationRepository, ReservationRepository>();
            services.AddTransient<IProviderScheduleRepository, ProviderScheduleRepository>();
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IProviderRepository, ProviderRepository>();


        }
    }
}
