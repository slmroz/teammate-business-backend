using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.Search;
using ERP.Services.Products.DTO;

public record GetPriceListsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string SortBy = "Name",      
    string SortOrder = "asc")    
    : IQuery<PagedResult<PriceListDto>>;
