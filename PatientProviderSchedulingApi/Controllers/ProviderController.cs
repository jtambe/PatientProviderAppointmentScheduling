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
    public class ProviderController : ControllerBase
    {

        private readonly IProviderService _providerService;
        private readonly ILogger<ProviderController> _logger;

        public ProviderController(IProviderService providerService, ILogger<ProviderController> logger)
        {
            _providerService = providerService;
            _logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Provider provider)
        {
            try
            {
                return Ok(await _providerService.AddProvider(provider));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProviderController.AddProvider failed for provider: {provider}", JsonSerializer.Serialize(provider));
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }
    }
}
