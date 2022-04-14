namespace PurchaseOrderProcessor.Domain.Models;

public record ShippingSlipItem
{
    public string Description { get; init; }
    public int Quantity { get; init; }
}