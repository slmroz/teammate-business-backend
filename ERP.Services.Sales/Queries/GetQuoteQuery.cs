using ERP.Services.Shared.CQRS;
using ERP.Services.Shared.DTO;

namespace ERP.Services.Sales.Queries;
public record GetQuoteQuery(int Id) : IQuery<QuoteDto>;