using ERP.Services.Shared.CQRS;
using ERP.Services.Products.DTO;

namespace ERP.Services.Products.Queries;
public record GetPriceListQuery(int Id) : IQuery<PriceListDto>;