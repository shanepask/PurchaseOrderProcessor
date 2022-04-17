using System.Collections.Generic;

namespace PurchaseOrderProcessor.Domain.Models
{
    public record PurchaseOrder
    {
        /// <summary>
        /// TEST
        /// </summary>
        /// <remarks>TEST2</remarks>
        public int Id { get; set; }
        public IEnumerable<string> Items { get; set; }
    }
}
