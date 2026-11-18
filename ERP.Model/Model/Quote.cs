using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class Quote
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int PriceListId { get; set; }

    public int? ContactId { get; set; }

    public string? QuoteNumber { get; set; }

    public string Subject { get; set; } = null!;

    public string? Description { get; set; }

    public int? QuoteStatusId { get; set; }

    public DateOnly ValidUntil { get; set; }

    public decimal? Subtotal { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? DueDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? RemovedAt { get; set; }

    public int? RemovedBy { get; set; }

    public virtual Contact? Contact { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual User? LastUpdatedByNavigation { get; set; }

    public virtual PriceList PriceList { get; set; } = null!;

    public virtual ICollection<QuoteItem> QuoteItems { get; set; } = new List<QuoteItem>();

    public virtual QuoteStatus? QuoteStatus { get; set; }

    public virtual User? RemovedByNavigation { get; set; }
}
