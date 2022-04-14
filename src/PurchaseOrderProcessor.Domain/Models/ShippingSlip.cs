using System;
using System.Collections.Generic;

namespace PurchaseOrderProcessor.Domain.Models
{
    public record ShippingSlip
    {
        public int CustomerId { get; init; }
        public IEnumerable<ShippingSlipItem> Items { get; init; } = Array.Empty<ShippingSlipItem>();
    }
}
