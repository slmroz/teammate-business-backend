using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.Search;
using ERP.Services.Products.DTO;

namespace ERP.Services.Products.Queries;

public record GetProductsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    int? GroupId = null,
    string SortBy = "PartNumber",  
    string SortOrder = "asc")      
    : IQuery<PagedResult<ProductDto>>;