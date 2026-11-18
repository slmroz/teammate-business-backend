namespace ERP.Services.Shared.DTO;
public record QuoteDto(
    int Id,
    string QuoteNumber,
    string Subject,
    int CustomerId,
    string CustomerName,
    int PriceListId,
    string? ContactName,
    string QuoteStatusName,
    DateTime ValidUntil,
    decimal Subtotal,
    decimal TaxAmount,
    decimal TotalAmount,
    DateTime CreatedAt,
    int ItemsCount
);