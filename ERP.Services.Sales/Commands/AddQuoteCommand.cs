using ERP.Services.Shared.CQRS;

namespace ERP.Services.Sales.Commands;
public class AddQuoteCommand : ICommand
{
    public int? Id { get; set; }  // Nadpisywalne dla testów
    public int CustomerId { get; set; }
    public int PriceListId { get; set; }
    public int? ContactId { get; set; }
    public string QuoteNumber { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string? Description { get; set; }
    public int QuoteStatusId { get; set; } = 1;
    public DateTime ValidUntil { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int CreatedBy { get; set; }
}