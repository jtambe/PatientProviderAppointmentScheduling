using Microsoft.Extensions.Logging;
using Moq;
using PatientProviderSchedulingApi.Models;
using PatientProviderSchedulingApi.Repositories.Interfaces;
using PatientProviderSchedulingApi.Services;
using PatientProviderSchedulingApi.Services.Interfaces;

namespace UnitTests
{
    public class ReservationServiceTests
    {
        
        private readonly Mock<IReservationRepository> _reservationRepository = new Mock<IReservationRepository>();
        private readonly Mock<IProviderScheduleService> _providerScheduleService = new Mock<IProviderScheduleService>();
        private readonly Mock<ILogger<ReservationService>> _logger = new Mock<ILogger<ReservationService>>();


        private DateTime stringToDateTime(string dateString)
        {
            var dateTime = DateTime.ParseExact(dateString, "yyyy-MM-ddTHH:mm:ss.fffZ", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal);
            return dateTime;
        }

        [Fact]
        public async void AddReservation_Test1() 
        {
            //Arrange
            var target = new ReservationService(_reservationRepository.Object, _providerScheduleService.Object, _logger.Object);

            // same start and End
            var reservation = new Reservation()
            {
                ClientId = 1,
                ProviderId = 1,
                StartTime = this.stringToDateTime("2023-08-19T09:30:42.408Z"),
                EndTime = this.stringToDateTime("2023-08-19T09:30:42.408Z"),
            };


            // Act
            var result = await target.AddReservation(reservation);
            // Assert
            Assert.Null(result);

        }
        
        [Fact]
        public async void AddReservation_Test2()
        {
            //Arrange
            var target = new ReservationService(_reservationRepository.Object, _providerScheduleService.Object, _logger.Object);

            // more than 15 minutes of timeslot
            var reservation = new Reservation()
            {
                ClientId = 1,
                ProviderId = 1,
                StartTime = this.stringToDateTime("2023-08-19T09:30:42.408Z"),
                EndTime = this.stringToDateTime("2023-08-19T09:50:42.408Z"),
            };


            // Act
            var result = await target.AddReservation(reservation);
            // Assert
            Assert.Null(result);

        }

        [Fact]
        public async void AddReservation_Test3()
        {
            //Arrange
            var target = new ReservationService(_reservationRepository.Object, _providerScheduleService.Object, _logger.Object);

            // 24 hours of gap needed between reservationTime and time slot of appointment
            var reservation = new Reservation()
            {
                ClientId = 1,
                ProviderId = 1,
                StartTime = this.stringToDateTime("2023-08-19T09:30:42.408Z"),
                EndTime = this.stringToDateTime("2023-08-19T09:45:42.408Z"),
                ReservationTime = this.stringToDateTime("2023-08-18T09:31:42.408Z"),
            };


            // Act
            var result = await target.AddReservation(reservation);
            // Assert
            Assert.Null(result);

        }


        [Fact]
        public async void AddReservation_Test4()
        {
            //Arrange
            var target = new ReservationService(_reservationRepository.Object, _providerScheduleService.Object, _logger.Object);

            // Provider Schedule does not fit the appointment time requested
            var reservation = new Reservation()
            {
                ClientId = 1,
                ProviderId = 1,
                StartTime = DateTime.Now.AddDays(2),
                EndTime = DateTime.Now.AddDays(2).AddMinutes(15),
                ReservationTime = DateTime.Now,
            };

            var pScheduleForDay = new ProviderSchedule()
            {
                ProviderId = 1,
                StartTime = DateTime.Now.AddDays(2).AddMinutes(-30),
                EndTime = DateTime.Now.AddDays(2).AddMinutes(10),
            };

            _providerScheduleService.Setup(x => x.GetProviderScheduleForTheDay(It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync(pScheduleForDay);

            // Act
            var result = await target.AddReservation(reservation);
            // Assert
            Assert.Null(result);

        }

        [Fact]
        public async void AddReservation_Test5()
        {
            //Arrange
            var target = new ReservationService(_reservationRepository.Object, _providerScheduleService.Object, _logger.Object);

            // someone else has confirmed an appointment in the requested timeslot
            var reservation = new Reservation()
            {
                ClientId = 1,
                ProviderId = 1,
                StartTime = DateTime.Now.AddDays(2),
                EndTime = DateTime.Now.AddDays(2).AddMinutes(15),
                ReservationTime = DateTime.Now,
            };

            //Provider's schedule meets the requested timeslot 
            var pScheduleForDay = new ProviderSchedule()
            {
                ProviderId = 1,
                StartTime = DateTime.Now.AddDays(2).AddMinutes(-30),
                EndTime = DateTime.Now.AddDays(2).AddHours(5),
            };

            // appoint is confirmed during requested time slot. So even if provider has schedule in the time, provider cannot see 2 patients
            var pReservations = new List<Reservation>()
            {
                new Reservation()
                {
                    ClientId = 15,
                    ProviderId = 1,
                    StartTime = DateTime.Now.AddDays(2).AddMinutes(-5),
                    EndTime = DateTime.Now.AddDays(2).AddMinutes(10),
                    ReservationTime = DateTime.Now.AddMinutes(-20),
                    IsConfirmed = true,
                    ConfirmationTime = DateTime.Now,
                }
            };

            _providerScheduleService.Setup(x => x.GetProviderScheduleForTheDay(It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync(pScheduleForDay);
            _reservationRepository.Setup(x => x.GetProviderReservationsForDay(It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync(pReservations);

            // Act
            var result = await target.AddReservation(reservation);
            // Assert
            Assert.Null(result);

        }

        [Fact]
        public async void AddReservation_Test6()
        {
            //Arrange
            var target = new ReservationService(_reservationRepository.Object, _providerScheduleService.Object, _logger.Object);

            // someone else has time left to confirm an appointment in the requested timeslot
            var reservation = new Reservation()
            {
                ClientId = 1,
                ProviderId = 1,
                StartTime = DateTime.Now.AddDays(2),
                EndTime = DateTime.Now.AddDays(2).AddMinutes(15),
                ReservationTime = DateTime.Now,
            };

            //Provider's schedule meets the requested timeslot 
            var pScheduleForDay = new ProviderSchedule()
            {
                ProviderId = 1,
                StartTime = DateTime.Now.AddDays(2).AddMinutes(-30),
                EndTime = DateTime.Now.AddDays(2).AddHours(5),
            };

            // someone else has time to confirm their appointment. As such we will not honor new requested appointment that conflicts pending appointment
            var pReservations = new List<Reservation>()
            {
                new Reservation()
                {
                    ClientId = 15,
                    ProviderId = 1,
                    StartTime = DateTime.Now.AddDays(2).AddMinutes(-5),
                    EndTime = DateTime.Now.AddDays(2).AddMinutes(10),
                    ReservationTime = DateTime.Now.AddMinutes(-20),
                    IsConfirmed = false,
                }
            };

            _providerScheduleService.Setup(x => x.GetProviderScheduleForTheDay(It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync(pScheduleForDay);
            _reservationRepository.Setup(x => x.GetProviderReservationsForDay(It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync(pReservations);

            // Act
            var result = await target.AddReservation(reservation);
            // Assert
            Assert.Null(result);

        }

        [Fact]
        public async void AddReservation_Test7()
        {

            //testing success

            //Arrange
            var target = new ReservationService(_reservationRepository.Object, _providerScheduleService.Object, _logger.Object);

            var reservation = new Reservation()
            {
                ClientId = 1,
                ProviderId = 1,
                StartTime = DateTime.Now.AddDays(2),
                EndTime = DateTime.Now.AddDays(2).AddMinutes(15),
                ReservationTime = DateTime.Now,
            };

            //Provider's schedule meets the requested timeslot 
            var pScheduleForDay = new ProviderSchedule()
            {
                ProviderId = 1,
                StartTime = DateTime.Now.AddDays(2).AddMinutes(-30),
                EndTime = DateTime.Now.AddDays(2).AddHours(5),
            };

            // Someone else made reservation but did not confirm within 30 minutes
            var pReservations = new List<Reservation>()
            {
                new Reservation()
                {
                    ClientId = 15,
                    ProviderId = 1,
                    StartTime = DateTime.Now.AddDays(2).AddMinutes(-5),
                    EndTime = DateTime.Now.AddDays(2).AddMinutes(10),
                    ReservationTime = DateTime.Now.AddMinutes(-31),
                    IsConfirmed = false,
                }
            };

            _providerScheduleService.Setup(x => x.GetProviderScheduleForTheDay(It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync(pScheduleForDay);
            _reservationRepository.Setup(x => x.GetProviderReservationsForDay(It.IsAny<int>(), It.IsAny<DateTime>())).ReturnsAsync(pReservations);
            _reservationRepository.Setup(x => x.AddReservation(It.IsAny<Reservation>())).ReturnsAsync(reservation);

            // Act
            var result = await target.AddReservation(reservation);
            // Assert
            Assert.NotNull(result);

        }


    }
}
