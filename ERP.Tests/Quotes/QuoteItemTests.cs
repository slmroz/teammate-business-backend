using ERP.Infrastructure.Time;
using ERP.Model.Model;
using ERP.Services.Sales.Commands;
using ERP.Services.Sales.Commands.Handlers;
using ERP.Services.Sales.Queries;
using ERP.Services.Sales.Queries.Handlers;
using ERP.Tests;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Tests.Quotes;

public class QuoteItemTests
{
    [Fact]
    public async Task AddQuoteItem_ShouldCreateItem_WithCreatedBy_RecalculateTotals()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var quote = new Quote { QuoteNumber = "Q-ITEM-001", CreatedBy = 1, Subject = "Test subject" };
        var product = new Product { Id = 101, Name = "Test Product", PartNumber = "1" };
        await context.Quotes.AddAsync(quote);
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var command = new AddQuoteItemCommand(0,
            quote.Id, 101, 2, 1, 2.5m, 100m, 0m, 250m, "Test brakes", 1);
        var handler = new AddQuoteItemHandler(context, new Clock());

        // Act
        await handler.HandleAsync(command);

        // Assert
        var item = await context.QuoteItems.FirstAsync(qi => qi.QuoteId == quote.Id);
        item.Should().NotBeNull();
        item.CreatedBy.Should().Be(1);
        item.LineTotal.Should().Be(250m);

        var updatedQuote = await context.Quotes.FindAsync(quote.Id);
        updatedQuote?.TotalAmount.Should().BeApproximately(307.5m, 0.1m);  // 250 * 1.23 VAT
    }

    [Fact]
    public async Task UpdateQuoteItem_ShouldUpdateFields_WithUpdatedBy_Recalculate()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var quote = new Quote { CreatedBy = 1 };
        await context.Quotes.AddAsync(quote);
        await context.SaveChangesAsync();

        var item = new QuoteItem
        {
            QuoteId = quote.Id,
            ProductId = 101,
            LineNumber = 1,
            Quantity = 1m,
            UnitPrice = 100m,
            LineTotal = 100m,
            CreatedBy = 1
        };
        await context.QuoteItems.AddAsync(item);
        await context.SaveChangesAsync();

        var command = new UpdateQuoteItemCommand(
            item.Id, quote.Id, 101, 3m, 120m, 0m, 360m, 1);
        var handler = new UpdateQuoteItemHandler(context, new Clock());

        // Act
        await handler.HandleAsync(command);

        // Assert
        var updatedItem = await context.QuoteItems.FindAsync(item.Id);
        updatedItem?.Quantity.Should().Be(3m);
        updatedItem?.LastUpdatedBy.Should().Be(1);

        var quoteTotal = await context.Quotes.FindAsync(quote.Id);
        quoteTotal?.TotalAmount.Should().BeApproximately(442.8m, 0.1m);  // 360 * 1.23
    }

    [Fact]
    public async Task RemoveQuoteItem_ShouldSoftDelete_WithRemovedBy_Recalculate()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var quote = new Quote { CreatedBy = 1 };
        await context.Quotes.AddAsync(quote);
        await context.SaveChangesAsync();

        var item = new QuoteItem
        {
            QuoteId = quote.Id,
            LineTotal = 200m,
            CreatedBy = 1
        };
        await context.QuoteItems.AddAsync(item);
        await context.SaveChangesAsync();

        var command = new RemoveQuoteItemCommand(item.Id, 1);
        var handler = new RemoveQuoteItemHandler(context, new Clock());

        // Act
        await handler.HandleAsync(command);

        // Assert
        var deletedItem = await context.QuoteItems.FindAsync(item.Id);
        deletedItem?.RemovedAt.Should().NotBeNull();
        deletedItem?.RemovedBy.Should().Be(1);

        var quoteAfterDelete = await context.Quotes.FindAsync(quote.Id);
        quoteAfterDelete?.TotalAmount.Should().Be(0m);  // Recalculated
    }

    [Fact]
    public async Task GetQuoteItems_ShouldReturnOnlyActiveItems()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var quote = new Quote { CreatedBy = 1 };
        await context.Quotes.AddAsync(quote);
        await context.SaveChangesAsync();

        var activeItem = new QuoteItem { QuoteId = quote.Id, LineNumber = 1, CreatedBy = 1 };
        var deletedItem = new QuoteItem
        {
            QuoteId = quote.Id,
            LineNumber = 2,
            RemovedAt = DateTime.UtcNow,
            RemovedBy = 1,
            CreatedBy = 1
        };
        await context.QuoteItems.AddRangeAsync(activeItem, deletedItem);
        await context.SaveChangesAsync();

        var query = new GetQuoteItemsQuery(quote.Id);
        var handler = new GetQuoteItemsHandler(context);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().HaveCount(1);
        result[0].LineNumber.Should().Be(1);
    }
}
