using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Mediator;
using PurchaseOrderProcessor.Domain.Models;

namespace PurchaseOrderProcessor.Api.Controllers
{
    [ApiController]
    [Route("{customer:int}/purchase-orders")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly ILogger<PurchaseOrderController> _logger;

        public PurchaseOrderController(ILogger<PurchaseOrderController> logger) => _logger = logger;

        [HttpPost]
        public Task Post([FromBody] PurchaseOrder purchaseOrder, [FromRoute] int customerId, [FromServices] IMediator mediator)
        {
            throw new NotImplementedException();
        }
    }
}
