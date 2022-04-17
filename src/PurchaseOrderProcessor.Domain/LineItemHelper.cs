using System.Collections.Generic;
using System.Linq;

namespace PurchaseOrderProcessor.Domain
{
    internal class LineItemHelper
    {
        private static readonly IEnumerable<string> _membershipProducts = new[] { "Book Club Membership", "Video Club Membership", "Premium Membership" };

        public static bool IsPhysicalProduct(string lineItem) => !IsMembershipProduct(lineItem) && lineItem != null && (lineItem.StartsWith("Book") || lineItem.StartsWith("Video"));

        public static bool IsMembershipProduct(string lineItem) => lineItem != null && _membershipProducts.Contains(lineItem);

    }
}
