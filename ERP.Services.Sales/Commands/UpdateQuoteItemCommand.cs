using ERP.Services.Shared.CQRS;

namespace ERP.Services.Sales.Commands;
public record UpdateQuoteItemCommand(
    int Id,
    int QuoteId,
    int ProductId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountPercentage,
    decimal LineTotal,
    int LastUpdatedBy
) : ICommand;