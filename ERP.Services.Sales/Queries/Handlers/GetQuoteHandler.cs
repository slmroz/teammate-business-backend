using ERP.Model.Model;
using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.DTO;
using ERP.Services.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Sales.Queries.Handlers;
internal sealed class GetQuoteHandler : IQueryHandler<GetQuoteQuery, QuoteDto>
{
    private readonly ErpContext _db;

    public GetQuoteHandler(ErpContext db) => _db = db;

    public async Task<QuoteDto> HandleAsync(GetQuoteQuery query)
    {
        var quote = await _db.Quotes
            .Include(q => q.Customer)
            .Include(q => q.Contact)
            .Include(q => q.QuoteStatus)
            .Include(q => q.QuoteItems)
            .FirstOrDefaultAsync(q => q.Id == query.Id && q.RemovedAt == null)
            ?? throw new NotFoundException($"Quote {query.Id} not found");

        return new QuoteDto(
            quote.Id,
            quote.QuoteNumber,
            quote.Subject,
            quote.CustomerId,
            quote.Customer.Name,
            quote.PriceListId,
            quote.Contact != null ? $"{quote.Contact.FirstName} {quote.Contact.LastName}" : null,
            quote.QuoteStatus.Label,
            quote.ValidUntil.ToDateTime(TimeOnly.MaxValue),
            quote.Subtotal ?? 0,
            quote.TaxAmount ?? 0,
            quote.TotalAmount,
            quote.CreatedAt.Value,
            quote.QuoteItems.Count(qi => qi.RemovedAt == null)
        );
    }
}
