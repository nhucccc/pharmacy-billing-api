namespace PharmacyBilling.Application.DTOs.Medicine;
public record StockAdjustRequest(int NewQuantity, string Reason);
