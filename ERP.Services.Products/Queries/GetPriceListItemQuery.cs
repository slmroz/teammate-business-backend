using ERP.Services.Shared.CQRS;
using ERP.Services.Products.DTO;

namespace ERP.Services.Products.Queries;
public record GetPriceListItemQuery(int Id) : IQuery<PriceListItemDto>;


