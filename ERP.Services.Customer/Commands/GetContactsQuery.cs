using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.Search;
using ERP.Services.Customer.DTO;

namespace ERP.Services.Customer.Commands; 

public record GetContactsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    int? CustomerId = null,
    string SortBy = "LastName",    
    string SortOrder = "asc")      
    : IQuery<PagedResult<ContactDto>>;