using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class QuoteItem
{
    public int Id { get; set; }

    public int QuoteId { get; set; }

    public int ProductId { get; set; }

    public int? ProductGroupId { get; set; }

    public int LineNumber { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public decimal LineTotal { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public int? LastUpdatedBy { get; set; }

    public DateTime? RemovedAt { get; set; }

    public int? RemovedBy { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? LastUpdatedByNavigation { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ProductGroup? ProductGroup { get; set; }

    public virtual Quote Quote { get; set; } = null!;

    public virtual User? RemovedByNavigation { get; set; }
}
