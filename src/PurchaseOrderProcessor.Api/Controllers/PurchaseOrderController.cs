using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Mediation;
using PurchaseOrderProcessor.Domain.Models;

namespace PurchaseOrderProcessor.Api.Controllers
{
    [ApiController]
    [Route("{customerId:int}/purchase-orders")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly ILogger<PurchaseOrderController> _logger;

        public PurchaseOrderController(ILogger<PurchaseOrderController> logger) => _logger = logger;

        [HttpPost]
        public Task<IActionResult> Post([FromBody] PurchaseOrder purchaseOrder, [FromRoute] int customerId, [FromServices] IMediator mediator, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
