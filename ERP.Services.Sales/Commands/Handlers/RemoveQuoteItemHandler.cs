using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Sales.Commands.Handlers;
internal sealed class RemoveQuoteItemHandler : ICommandHandler<RemoveQuoteItemCommand>
{
    private readonly ErpContext _db;
    private readonly IClock _clock;

    public RemoveQuoteItemHandler(ErpContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task HandleAsync(RemoveQuoteItemCommand command)
    {
        var item = await _db.QuoteItems
            .Include(qi => qi.Quote)
            .FirstOrDefaultAsync(qi => qi.Id == command.Id)
            ?? throw new NotFoundException($"QuoteItem {command.Id} not found");

        item.RemovedAt = _clock.Current();
        item.RemovedBy = command.RemovedBy;

        await _db.SaveChangesAsync();
        await RecalculateQuoteTotals(item.QuoteId);
    }

    private async Task RecalculateQuoteTotals(int quoteId)
    {
        var total = await _db.QuoteItems
            .Where(qi => qi.QuoteId == quoteId && qi.RemovedAt == null)
            .SumAsync(qi => qi.LineTotal);

        var quote = await _db.Quotes.FindAsync(quoteId)!;
        quote.Subtotal = total;
        quote.TotalAmount = total * 1.23m;
        quote.TaxAmount = total * 0.23m;
        quote.LastUpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
