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
internal sealed class UpdateQuoteHandler : ICommandHandler<UpdateQuoteCommand>
{
    private readonly ErpContext _db;
    private readonly IClock _clock;

    public UpdateQuoteHandler(ErpContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task HandleAsync(UpdateQuoteCommand command)
    {
        var quote = await _db.Quotes
            .Include(q => q.Customer)
            .Include(q => q.Contact)
            .FirstOrDefaultAsync(q => q.Id == command.Id)
            ?? throw new NotFoundException($"Quote {command.Id} not found");

        // Walidacja powiązań
        if (!await _db.Customers.AnyAsync(c => c.Id == command.CustomerId))
            throw new NotFoundException($"Customer {command.CustomerId} not found");

        if (!await _db.PriceLists.AnyAsync(p => p.Id == command.PriceListId))
            throw new NotFoundException($"PriceList {command.PriceListId} not found");

        if (command.ContactId.HasValue &&
            !await _db.Contacts.AnyAsync(c => c.Id == command.ContactId))
            throw new NotFoundException($"Contact {command.ContactId} not found");

        // Aktualizacja
        quote.CustomerId = command.CustomerId;
        quote.PriceListId = command.PriceListId;
        quote.ContactId = command.ContactId;
        quote.Subject = command.Subject;
        quote.Description = command.Description;
        quote.QuoteStatusId = command.QuoteStatusId;
        quote.ValidUntil = DateOnly.FromDateTime( command.ValidUntil);
        quote.Subtotal = command.Subtotal;
        quote.TaxAmount = command.TaxAmount;
        quote.TotalAmount = command.TotalAmount;
        quote.LastUpdatedAt = _clock.Current();
        quote.LastUpdatedBy = command.LastUpdatedBy;  // CurrentUser.Id

        await _db.SaveChangesAsync();
    }
}
