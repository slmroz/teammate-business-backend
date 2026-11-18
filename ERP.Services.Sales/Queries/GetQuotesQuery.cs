using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.DTO;
using ERP.Services.Shared.Search;

namespace ERP.Services.Sales.Queries;
public record GetQuotesQuery(
    int Page = 1,
    int PageSize = 20,
    int? CustomerId = null,
    string? QuoteNumber = null,
    int? QuoteStatusId = null,
    DateTime? ValidUntilFrom = null,
    DateTime? ValidUntilTo = null,
    decimal? MinTotalAmount = null,
    decimal? MaxTotalAmount = null,
    string? SortBy = null,      // "QuoteNumber", "TotalAmount", "ValidUntil", "CreatedAt"
    string? SortDirection = "desc"  // "asc", "desc"
) : IQuery<PagedResult<QuoteDto>>;