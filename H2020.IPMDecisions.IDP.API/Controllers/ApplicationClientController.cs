using System.Threading.Tasks;
using H2020.IPMDecisions.IDP.Data.Core;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("/api/applicationclient")]
    public class ApplicationClientController : ControllerBase
    {
        private readonly IDataService dataService;

        public ApplicationClientController(IDataService dataService)
        {
            this.dataService = dataService 
                ?? throw new System.ArgumentNullException(nameof(dataService));
        }

        [HttpGet("", Name = "GetApplicationClients")]
        [HttpHead]
        // GET: api/applicationclient
        public async Task<IActionResult> GetApplicationClients()
        {
            var applicationClients = await this.dataService.ApplicationClients.GetAll();

            return Ok(applicationClients);
        }
    }
}