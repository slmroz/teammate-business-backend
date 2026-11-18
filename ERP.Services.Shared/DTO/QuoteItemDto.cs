namespace ERP.Services.Shared.DTO;
public record QuoteItemDto(
    int Id,
    int LineNumber,
    string ProductName,
    string? ProductGroupName,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage,
    decimal LineTotal,
    string? Description
);