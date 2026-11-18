using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.Search;

namespace ERP.Services.Products.Commands;
public record UpdatePriceListItemCommand(
    int Id,
    int PriceListId,
    int ProductId,
    decimal Price,
    int LastUpdatedBy) : ICommand;

