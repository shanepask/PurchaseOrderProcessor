using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using PurchaseOrderProcessor.Domain.Mediation;
using PurchaseOrderProcessor.Domain.Models;

namespace PurchaseOrderProcessor.Api.Controllers
{
    /// <summary>
    /// Process purchase orders for customers.
    /// </summary>
    [ApiController]
    [Route("{customerId:int}/purchase-orders")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "Customer Purchase Orders")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly ILogger<PurchaseOrderController> _logger;

        /// <summary>
        /// Create a new purchase order controller.
        /// </summary>
        /// <param name="logger"></param>
        public PurchaseOrderController(ILogger<PurchaseOrderController> logger) => _logger = logger;

        /// <summary>
        /// Submit a purchase order for processing and produce a shipping slip for physical products.
        /// </summary>
        /// <remarks>
        /// This operation will take the purchase order and process each of its line items to produce a shipping slip and update the customer records with the membership status.
        /// </remarks>
        /// <example name="Membership products" description="A purchase order that will update the customers membership but produce no shipping slip.">
        /// {
        ///     "id": 1234,
        ///     "items": [
        ///         "Book Club Membership"
        ///     ]
        /// }
        /// </example>
        /// <example name="Physical products" description="A purchase order that will produce a shipping slip.">
        /// {
        ///     "id": 1234,
        ///     "items": [
        ///         "Book \"The Girl on the train\"",
        ///         "Video \"Comprehensive First Aid Training\""
        ///     ]
        /// }
        /// </example>
        /// <example name="Mixed products" description="A purchase order that will produce a shipping slip and update a customers membership status.">
        /// {
        ///     "id": 1234,
        ///     "items": [
        ///         "Book \"The Girl on the train\"",
        ///         "Video \"Comprehensive First Aid Training\"",
        ///         "Premium Membership"
        ///     ]
        /// }
        /// </example>
        /// <param name="purchaseOrder">The purchase order to process.</param>
        /// <param name="customerId">The customer ID for the purchase order.</param>
        /// <param name="mediator"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Nothing or a shipping slip for the physical products in the purchase order.</returns>
        /// <response code="200">The purchase order has been processed successfully and a shipping slip has been returned.</response>
        /// <response code="204">The purchase order has been processed successfully and no shipping slip is required.</response>
        /// <response code="400">The request could not be processed as it was not a valid purchase order.</response>
        /// <response code="415">The content type required was not supported.</response>
        /// <response code="500">The server encountered a problem processing the purchase order, it is assumed the purchase order has not been processed.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ShippingSlip), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(415)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Post([FromBody] PurchaseOrder purchaseOrder, [FromRoute] int customerId, [FromServices] IMediator mediator, CancellationToken cancellationToken)
        {
            using var sc = _logger.BeginScope("Process purchase order request {correlationId}", HttpContext?.TraceIdentifier);
            _logger.LogInformation("Processing purchase order: {purchaseOrderId}", purchaseOrder.Id);
            try
            {
                var poResult = await mediator.ProcessAsync(customerId, purchaseOrder, cancellationToken);
                _logger.LogInformation("Purchase order processing successful: {purchaseOrderId}, {shippingItemCount} shipping slip items", purchaseOrder.Id, poResult?.Items?.Count());
                return poResult?.Items != null && poResult.Items.Any() ? new OkObjectResult(poResult) : new NoContentResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Purchase order processing failed: {purchaseOrderId}", purchaseOrder.Id);
                throw new HttpRequestException("An internal error occurred.", e, HttpStatusCode.InternalServerError);
            }
        }
    }
}
