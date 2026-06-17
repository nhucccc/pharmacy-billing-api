using PharmacyBilling.Domain.Common;

namespace PharmacyBilling.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    public Guid InvoiceId { get; private set; }
    public string ItemName { get; private set; } = null!;
    public string ItemType { get; private set; } = null!;   // "MEDICINE", "EXAMINATION", "SERVICE"
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal SubTotal => Quantity * UnitPrice;
    public string? Note { get; private set; }

    // Navigation
    public Invoice Invoice { get; private set; } = null!;

    private InvoiceItem() { }

    public static InvoiceItem Create(Guid invoiceId, string itemName, string itemType,
        int quantity, decimal unitPrice, string? note = null)
        => new InvoiceItem
        {
            InvoiceId = invoiceId,
            ItemName = itemName,
            ItemType = itemType,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Note = note
        };
}
