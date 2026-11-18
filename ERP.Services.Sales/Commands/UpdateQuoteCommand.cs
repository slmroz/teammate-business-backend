using ERP.Services.Shared.CQRS;

namespace ERP.Services.Sales.Commands;
public record UpdateQuoteCommand(
    int Id,
    int CustomerId,
    int PriceListId,
    int? ContactId,
    string Subject,
    string? Description,
    int QuoteStatusId,
    DateTime ValidUntil,
    decimal Subtotal,
    decimal TaxAmount,
    decimal TotalAmount,
    int LastUpdatedBy
) : ICommand;