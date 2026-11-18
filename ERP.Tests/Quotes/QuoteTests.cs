using ERP.Infrastructure.Time;
using ERP.Model.Model;
using ERP.Services.Sales.Commands;
using ERP.Services.Sales.Commands.Handlers;
using ERP.Services.Sales.Queries;
using ERP.Services.Sales.Queries.Handlers;
using ERP.Services.Shared.Exceptions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ERP.Tests.Quotes;

public class QuoteTests
{
    [Fact]
    public async Task AddQuote_ShouldCreateQuote_WithCreatedBy()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var command = new AddQuoteCommand
        {
            CustomerId = 1,
            PriceListId = 1,
            QuoteNumber = "Q-TEST-001",
            Subject = "Test Offer",
            ValidUntil = DateTime.UtcNow.AddDays(30),
            TotalAmount = 1000m,
            CreatedBy = 1  // Nowe pole!
        };
        var handler = new AddQuoteHandler(context, new Infrastructure.Time.Clock());

        // Act
        await handler.HandleAsync(command);

        // Assert
        var quote = await context.Quotes.FirstAsync(q => q.QuoteNumber == "Q-TEST-001");
        quote.Should().NotBeNull();
        quote.CreatedBy.Should().Be(1);
        quote.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        command.Id.Should().Be(quote.Id);  // Zapisane do komendy
    }

    [Fact]
    public async Task UpdateQuote_ShouldUpdateFields_WithUpdatedBy()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var quote = new Quote
        {
            CustomerId = 1,
            QuoteNumber = "Q-UPDATE-001",
            TotalAmount = 500m,
            CreatedBy = 1
        };
        await context.Quotes.AddAsync(quote);
        await context.SaveChangesAsync();

        var command = new UpdateQuoteCommand(
            quote.Id, 1, 1, null, "Updated Subject", null, 1,
            DateTime.UtcNow.AddDays(15), 450m, 50m, 500m, 1);
        var handler = new UpdateQuoteHandler(context, new Infrastructure.Time.Clock());

        // Act
        await handler.HandleAsync(command);

        // Assert
        var updatedQuote = await context.Quotes.FindAsync(quote.Id);
        updatedQuote.Should().NotBeNull();
        updatedQuote.Subject.Should().Be("Updated Subject");
        updatedQuote.LastUpdatedBy.Should().Be(1);
        updatedQuote.LastUpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task RemoveQuote_ShouldSoftDelete_WithRemovedBy()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        var quote = new Quote { QuoteNumber = "Q-DELETE-001", CreatedBy = 1 };
        await context.Quotes.AddAsync(quote);
        await context.SaveChangesAsync();

        var command = new RemoveQuoteCommand(quote.Id, 1);
        var handler = new RemoveQuoteHandler(context, new Infrastructure.Time.Clock());

        // Act
        await handler.HandleAsync(command);

        // Assert
        var deletedQuote = await context.Quotes.FindAsync(quote.Id);
        deletedQuote.Should().NotBeNull();
        deletedQuote.RemovedAt.Should().NotBeNull();
        deletedQuote.RemovedBy.Should().Be(1);
    }

    [Fact]
    public async Task GetQuotes_ShouldReturnPagedResult_WithTotalCountItems()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange - 3 quotes
        for (int i = 1; i <= 3; i++)
        {
            await context.Quotes.AddAsync(new Quote
            {
                QuoteNumber = $"Q-{i}",
                TotalAmount = i * 100m,
                CreatedBy = 1
            });
        }
        await context.SaveChangesAsync();

        var query = new GetQuotesQuery(Page: 1, PageSize: 2);
        var handler = new GetQuotesHandler(context);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert - Nowa struktura PagedResult!
        result.TotalCount.Should().Be(3);
        result.Items.Should().HaveCount(2);
        result.Items[0].QuoteNumber.Should().Be("Q-3");  // Sort desc CreatedAt
    }

    [Fact]
    public async Task GetQuotes_ShouldFilterByCustomerId()
    {
        using var context = TestDbContextFactory.Create();

        // Arrange
        await context.Quotes.AddRangeAsync(
            new Quote { CustomerId = 1, QuoteNumber = "Q-C1-1" },
            new Quote { CustomerId = 2, QuoteNumber = "Q-C2-1" },
            new Quote { CustomerId = 1, QuoteNumber = "Q-C1-2" }
        );
        await context.SaveChangesAsync();

        var query = new GetQuotesQuery(CustomerId: 1);
        var handler = new GetQuotesHandler(context);

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        result.TotalCount.Should().Be(2);
        result.Items.Should().OnlyContain(q => q.CustomerId == 1);
    }

    [Fact]
    public async Task GetQuote_ShouldThrowNotFoundException()
    {
        using var context = TestDbContextFactory.Create();

        var query = new GetQuoteQuery(Id: 999);
        var handler = new GetQuoteHandler(context);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => handler.HandleAsync(query));
    }
}
