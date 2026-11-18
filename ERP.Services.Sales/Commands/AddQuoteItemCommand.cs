using ERP.Services.Shared.CQRS;

namespace ERP.Services.Sales.Commands;
public class AddQuoteItemCommand : ICommand
{
    public AddQuoteItemCommand(int? id, int quoteId, int productId, int? productGroupId, int lineNumber, decimal quantity, decimal unitPrice, decimal discountPercentage, decimal lineTotal, string? description, int createdBy)
    {
        Id = id;
        QuoteId = quoteId;
        ProductId = productId;
        ProductGroupId = productGroupId;
        LineNumber = lineNumber;
        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountPercentage = discountPercentage;
        LineTotal = lineTotal;
        Description = description;
        CreatedBy = createdBy;
    }

    public int? Id { get; set; }  // Nadpisywalne dla testów
    public int QuoteId { get; set; }
    public int ProductId { get; set; }
    public int? ProductGroupId { get; set; }
    public int LineNumber { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal LineTotal { get; set; }
    public string? Description { get; set; } = null;
    public int CreatedBy { get; set; }
}