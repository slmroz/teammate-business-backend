using ERP.Model.Model;
using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.Sales.Queries.Handlers;
internal sealed class GetQuoteItemsHandler : IQueryHandler<GetQuoteItemsQuery, List<QuoteItemDto>>
{
    private readonly ErpContext _db;

    public GetQuoteItemsHandler(ErpContext db) => _db = db;

    public async Task<List<QuoteItemDto>> HandleAsync(GetQuoteItemsQuery query)
    {
        return await _db.QuoteItems
            .Include(qi => qi.Product)
            .ThenInclude(p => p.ProductGroup)
            .Where(qi => qi.QuoteId == query.QuoteId && qi.RemovedAt == null)
            .OrderBy(qi => qi.LineNumber)
            .Select(qi => new QuoteItemDto(
                qi.Id,
                qi.LineNumber,
                qi.Product.Name,
                qi.ProductGroup != null ? qi.ProductGroup.Name : null,
                qi.Quantity,
                qi.UnitPrice,
                qi.DiscountPercentage ?? 0,
                qi.LineTotal,
                qi.Description
            ))
            .ToListAsync();
    }
}

