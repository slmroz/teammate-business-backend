using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Shared.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Sales.Commands.Handlers;
internal sealed class AddQuoteItemHandler : ICommandHandler<AddQuoteItemCommand>
{
    private readonly ErpContext _db;
    private readonly IClock _clock;

    public AddQuoteItemHandler(ErpContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task HandleAsync(AddQuoteItemCommand command)
    {
        var quote = await _db.Quotes.FindAsync(command.QuoteId)
            ?? throw new Exception($"Quote {command.QuoteId} not found");

        if (!await _db.Products.AnyAsync(p => p.Id == command.ProductId))
            throw new Exception($"Product {command.ProductId} not found");

        var item = new QuoteItem
        {
            QuoteId = command.QuoteId,
            ProductId = command.ProductId,
            ProductGroupId = command.ProductGroupId,
            LineNumber = command.LineNumber,
            Quantity = command.Quantity,
            UnitPrice = command.UnitPrice,
            DiscountPercentage = command.DiscountPercentage,
            LineTotal = command.LineTotal,
            Description = command.Description,
            CreatedAt = _clock.Current(),
            CreatedBy = command.CreatedBy
        };

        await _db.QuoteItems.AddAsync(item);
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

