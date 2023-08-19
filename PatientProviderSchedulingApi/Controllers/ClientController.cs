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
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientController> _logger;

        public ClientController(IClientService clientService, ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] Client client)
        {
            try
            {
                return Ok(await _clientService.AddClient(client));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ClientController.AddClient failed for client: {client}", JsonSerializer.Serialize(client));
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }
    }
}
