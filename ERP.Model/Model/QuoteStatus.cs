using System;
using System.Collections.Generic;

namespace ERP.Model.Model;

public partial class QuoteStatus
{
    public int Id { get; set; }

    public string? Label { get; set; }

    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();
}
