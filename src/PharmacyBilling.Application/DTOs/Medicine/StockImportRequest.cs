namespace PharmacyBilling.Application.DTOs.Medicine;
public record StockImportRequest(int Quantity, string? Note, DateTime? ExpiryDate);
