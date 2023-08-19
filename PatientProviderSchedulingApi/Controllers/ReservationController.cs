using Microsoft.AspNetCore.Mvc;
using PatientProviderSchedulingApi.Models;
using PatientProviderSchedulingApi.Services.Interfaces;
using System.Net;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PatientProviderSchedulingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {

        private readonly IReservationService _reservationService;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(IReservationService reservationService, ILogger<ReservationController> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Reservation reservation)
        {
            try
            {
                var result = await _reservationService.AddReservation(reservation);
                if(result is null)
                {
                    return StatusCode((int)HttpStatusCode.NotAcceptable);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReservationController.AddReservation failed for reservation: {reservation}", JsonSerializer.Serialize(reservation));
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }

        [HttpPost]
        [Route("confirm")]
        public async Task<IActionResult> ConfirmAsync([FromBody] Reservation reservation)
        {
            try
            {
                return Ok(await _reservationService.ConfirmReservation(reservation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReservationController.ConfirmReservation failed for reservation: {reservation}", JsonSerializer.Serialize(reservation));
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }
    }
}
