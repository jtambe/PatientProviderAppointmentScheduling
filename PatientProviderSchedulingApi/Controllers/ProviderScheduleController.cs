using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientProviderSchedulingApi.Models;
using PatientProviderSchedulingApi.Services.Interfaces;
using System.Net;
using System.Text.Json;

namespace PatientProviderSchedulingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderScheduleController : ControllerBase
    {
        private readonly IProviderScheduleService _providerScheduleService;
        private readonly ILogger<ProviderScheduleController> _logger;

        public ProviderScheduleController(IProviderScheduleService providerScheduleService, ILogger<ProviderScheduleController> logger)
        {
            _providerScheduleService = providerScheduleService;
            _logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] IEnumerable<ProviderSchedule> providerSchedules)
        {
            try
            {
                return  Ok(await _providerScheduleService.AddProviderSchedule(providerSchedules));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProviderScheduleController.AddProviderSchedule failed for providerSchedules: {providerSchedules}", JsonSerializer.Serialize(providerSchedules));
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }

    }
}
