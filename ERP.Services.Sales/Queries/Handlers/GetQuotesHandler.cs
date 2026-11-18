using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Sales.Commands;
using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.DTO;
using ERP.Services.Shared.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.Sales.Queries.Handlers;
// GetQuotesHandler.cs
internal sealed class GetQuotesHandler : IQueryHandler<GetQuotesQuery, PagedResult<QuoteDto>>
{
    private readonly ErpContext _db;

    public GetQuotesHandler(ErpContext db) => _db = db;

    public async Task<PagedResult<QuoteDto>> HandleAsync(GetQuotesQuery query)
    {
        var quotesQuery = _db.Quotes
            .Include(q => q.Customer)
            .Include(q => q.Contact)
            .Include(q => q.QuoteStatus)
            .AsQueryable();

        // Filtrowanie
        if (query.CustomerId.HasValue)
            quotesQuery = quotesQuery.Where(q => q.CustomerId == query.CustomerId);
        if (!string.IsNullOrEmpty(query.QuoteNumber))
            quotesQuery = quotesQuery.Where(q => q.QuoteNumber.Contains(query.QuoteNumber));
        if (query.QuoteStatusId.HasValue)
            quotesQuery = quotesQuery.Where(q => q.QuoteStatusId == query.QuoteStatusId);
        if (query.ValidUntilFrom.HasValue)
            quotesQuery = quotesQuery.Where(q => q.ValidUntil >= DateOnly.FromDateTime(query.ValidUntilFrom.Value.Date));
        if (query.ValidUntilTo.HasValue)
            quotesQuery = quotesQuery.Where(q => q.ValidUntil <= DateOnly.FromDateTime(query.ValidUntilTo.Value.Date));
        if (query.MinTotalAmount.HasValue)
            quotesQuery = quotesQuery.Where(q => q.TotalAmount >= query.MinTotalAmount);
        if (query.MaxTotalAmount.HasValue)
            quotesQuery = quotesQuery.Where(q => q.TotalAmount <= query.MaxTotalAmount);

        // Sortowanie
        quotesQuery = ApplySorting(quotesQuery, query.SortBy, query.SortDirection);

        var total = await quotesQuery.CountAsync();
        var items = await quotesQuery
            .OrderByDescending(q => q.CreatedAt)  // Domyślne sortowanie
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(q => new QuoteDto(
                q.Id,
                q.QuoteNumber,
                q.Subject,
                q.CustomerId,
                q.Customer.Name,
                q.PriceListId,
                q.Contact != null ? $"{q.Contact.FirstName} {q.Contact.LastName}" : null,
                q.QuoteStatus.Label,
                q.ValidUntil.ToDateTime(TimeOnly.MinValue),
                q.Subtotal ?? 0,
                q.TaxAmount ?? 0,
                q.TotalAmount,
                q.CreatedAt.Value,
                q.QuoteItems.Count
            ))
            .ToListAsync();

        return new PagedResult<QuoteDto>
        {
            TotalCount = total,
            Items = items
        };
    }

    private IQueryable<Quote> ApplySorting(
        IQueryable<Quote> query,
        string? sortBy,
        string? sortDirection)
    {
        var isDescending = sortDirection?.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "quotenumber" => isDescending
                ? query.OrderByDescending(q => q.QuoteNumber)
                : query.OrderBy(q => q.QuoteNumber),
            "totalamount" => isDescending
                ? query.OrderByDescending(q => q.TotalAmount)
                : query.OrderBy(q => q.TotalAmount),
            "validuntil" => isDescending
                ? query.OrderByDescending(q => q.ValidUntil)
                : query.OrderBy(q => q.ValidUntil),
            "createdat" or _ => isDescending
                ? query.OrderByDescending(q => q.CreatedAt)
                : query.OrderBy(q => q.CreatedAt)
        };
    }
}

