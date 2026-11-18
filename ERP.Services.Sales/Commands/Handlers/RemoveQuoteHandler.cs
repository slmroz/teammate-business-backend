using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.Sales.Commands.Handlers;
internal sealed class RemoveQuoteHandler : ICommandHandler<RemoveQuoteCommand>
{
    private readonly ErpContext _db;
    private readonly IClock _clock;

    public RemoveQuoteHandler(ErpContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task HandleAsync(RemoveQuoteCommand command)
    {
        var quote = await _db.Quotes.FindAsync(command.Id)
            ?? throw new NotFoundException($"Quote {command.Id} not found");

        // Soft delete - tylko jeśli brak aktywnych pozycji
        if (await _db.QuoteItems.AnyAsync(qi => qi.QuoteId == command.Id && qi.RemovedAt == null))
            throw new InvalidOperationException("Cannot remove quote with active items");

        quote.RemovedAt = _clock.Current();
        quote.RemovedBy = command.RemovedBy;  // CurrentUser.Id

        await _db.SaveChangesAsync();
    }
}
