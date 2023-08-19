using Microsoft.Extensions.Logging;
using Moq;
using PatientProviderSchedulingApi.Models;
using PatientProviderSchedulingApi.Repositories.Interfaces;
using PatientProviderSchedulingApi.Services;


namespace UnitTests
{
    public class ProviderScheduleServiceTests
    {

        private readonly Mock<IProviderScheduleRepository> _providerScheduleRepository = new Mock<IProviderScheduleRepository>();
        private readonly Mock<ILogger<ProviderScheduleService>> _logger = new Mock<ILogger<ProviderScheduleService>>();


        [Fact]
        public void AddProviderSchedule_Multiple_Schedules_For_One_Day() // for single providerids
        {

            //Arrange
            var target = new ProviderScheduleService(_providerScheduleRepository.Object, _logger.Object);


            List<ProviderSchedule> schedule = new List<ProviderSchedule>()
            {
                new ProviderSchedule ()
                {
                    ProviderId = 1,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(5)
                },
                new ProviderSchedule ()
                {
                    ProviderId = 1,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(3)
                }
            };
            var exceptionType = typeof(Exception);
            var expectedMessage = "providerschedules for a provider cannot contain multiple schedules for single day";

            // Act and Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>  await target.AddProviderSchedule(schedule)).Result;
            // Assert
            Assert.Equal(expectedMessage, ex.Message);

        }

        [Fact]
        public void AddProviderSchedule_Multiple_Schedules_For_One_Day_Multiple_Providers() // for multiple providerids
        {

            //Arrange
            var target = new ProviderScheduleService(_providerScheduleRepository.Object, _logger.Object);


            List<ProviderSchedule> schedule = new List<ProviderSchedule>()
            {
                new ProviderSchedule ()
                {
                    ProviderId = 1,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(5)
                },
                new ProviderSchedule ()
                {
                    ProviderId = 1,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(2)
                },
                new ProviderSchedule ()
                {
                    ProviderId = 2,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(3)
                }
            };
            var exceptionType = typeof(Exception);
            var expectedMessage = "providerschedules for a provider cannot contain multiple schedules for single day";

            // Act and Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await target.AddProviderSchedule(schedule)).Result;
            // Assert
            Assert.Equal(expectedMessage, ex.Message);

        }

        [Fact]
        public async void AddProviderSchedule_Multiple_Schedules_For_One_Day_Multiple_Providers_Success() // for multiple providerids
        {

            //Arrange
            var target = new ProviderScheduleService(_providerScheduleRepository.Object, _logger.Object);
            List<ProviderSchedule> schedule = new List<ProviderSchedule>()
            {
                new ProviderSchedule ()
                {
                    ProviderId = 1,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(5)
                },
                new ProviderSchedule ()
                {
                    ProviderId = 2,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(3)
                }
            };

            _providerScheduleRepository.Setup(x => x.AddProviderSchedule(It.IsAny<IEnumerable<ProviderSchedule>>())).ReturnsAsync(schedule);

           
            // Act and Assert
            var result = await target.AddProviderSchedule(schedule);
            // Assert
            Assert.Equal(2, result.Count());

        }
    }
}
