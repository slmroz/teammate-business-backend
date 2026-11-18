using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.Search;
using ERP.Services.Customer.DTO;

namespace ERP.Services.Customer.Queries;
public sealed record GetCustomersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string SortBy = "Name",       
    string SortOrder = "asc"
) : IQuery<PagedResult<CustomerDto>>;