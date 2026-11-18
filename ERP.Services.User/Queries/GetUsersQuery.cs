using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.Search;
using ERP.Services.User.DTO;

namespace ERP.Services.User.Queries;
public record GetUsersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    int? Role = null,           
    string SortBy = "LastName",    
    string SortOrder = "asc")      
    : IQuery<PagedResult<UserDto>>;