using ERP.Model.Abstractions;
using ERP.Model.Model;
using ERP.Services.Sales.Queries;
using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.DTO;
using ERP.Services.Shared.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Services.Sales.Commands.Handlers;
// ERP.Services.Sales/Handlers/AddQuoteHandler.cs
internal sealed class AddQuoteHandler : ICommandHandler<AddQuoteCommand>
{
    private readonly ErpContext _db;
    private readonly IClock _clock;

    public AddQuoteHandler(ErpContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task HandleAsync(AddQuoteCommand command)
    {
        // Walidacja powiązań
        if (!await _db.Customers.AnyAsync(c => c.Id == command.CustomerId))
            throw new Exception($"Customer {command.CustomerId} not found");

        if (!await _db.PriceLists.AnyAsync(p => p.Id == command.PriceListId))
            throw new Exception($"PriceList {command.PriceListId} not found");

        if (command.ContactId.HasValue &&
            !await _db.Contacts.AnyAsync(c => c.Id == command.ContactId))
            throw new Exception($"Contact {command.ContactId} not found");

        var quote = new Quote
        {
            Id = command.Id ?? 0,  // Nadpisane Id dla testów
            CustomerId = command.CustomerId,
            PriceListId = command.PriceListId,
            ContactId = command.ContactId,
            QuoteNumber = command.QuoteNumber,
            Subject = command.Subject,
            Description = command.Description,
            QuoteStatusId = command.QuoteStatusId,
            ValidUntil = DateOnly.FromDateTime(command.ValidUntil),
            Subtotal = command.Subtotal,
            TaxAmount = command.TaxAmount,
            TotalAmount = command.TotalAmount,
            DiscountPercentage = command.DiscountPercentage,
            IssueDate = command.IssueDate != null ? DateOnly.FromDateTime(command.IssueDate.Value) : DateOnly.FromDateTime(_clock.Current().Date),
            DueDate = command.DueDate != null ? DateOnly.FromDateTime(command.DueDate.Value) : null,
            CreatedAt = _clock.Current(),
            CreatedBy = command.CreatedBy,  // CurrentUser.Id
        };

        await _db.Quotes.AddAsync(quote);
        await _db.SaveChangesAsync();

        // Zapisanie wygenerowanego Id do komendy (dla testów)
        command.Id = quote.Id;
    }
}

