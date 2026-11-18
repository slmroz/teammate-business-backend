using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.Sales.Commands.Handlers;
internal sealed class UpdateQuoteItemHandler : ICommandHandler<UpdateQuoteItemCommand>
{
    private readonly ErpContext _db;
    private readonly IClock _clock;

    public UpdateQuoteItemHandler(ErpContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task HandleAsync(UpdateQuoteItemCommand command)
    {
        var item = await _db.QuoteItems
            .Include(qi => qi.Quote)
            .FirstOrDefaultAsync(qi => qi.Id == command.Id)
            ?? throw new NotFoundException($"QuoteItem {command.Id} not found");

        if (!await _db.Products.AnyAsync(p => p.Id == command.ProductId))
            throw new NotFoundException($"Product {command.ProductId} not found");

        // Aktualizacja
        item.QuoteId = command.QuoteId;
        item.ProductId = command.ProductId;
        item.Quantity = command.Quantity;
        item.UnitPrice = command.UnitPrice;
        item.DiscountPercentage = command.DiscountPercentage;
        item.LineTotal = command.LineTotal;
        item.LastUpdatedAt = _clock.Current();
        item.LastUpdatedBy = command.LastUpdatedBy;

        await _db.SaveChangesAsync();
        await RecalculateQuoteTotals(command.QuoteId);
    }

    private async Task RecalculateQuoteTotals(int quoteId)
    {
        var total = await _db.QuoteItems
            .Where(qi => qi.QuoteId == quoteId && qi.RemovedAt == null)
            .SumAsync(qi => qi.LineTotal);

        var quote = await _db.Quotes.FindAsync(quoteId)!;
        quote.Subtotal = total;
        quote.TotalAmount = total * 1.23m;  // +23% VAT
        quote.TaxAmount = total * 0.23m;
        quote.LastUpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
