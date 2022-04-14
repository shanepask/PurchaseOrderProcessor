using System.Collections;
using System.Collections.Generic;

namespace PurchaseOrderProcessor.Domain.Models
{
    public record PurchaseOrder
    {
        public int Id { get; set; }
        public IEnumerable<string> Items { get; set; }
    }
}
