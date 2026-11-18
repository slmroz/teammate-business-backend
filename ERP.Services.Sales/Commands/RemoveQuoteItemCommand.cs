using ERP.Services.Shared.CQRS;

namespace ERP.Services.Sales.Commands;
public record RemoveQuoteItemCommand(int Id, int RemovedBy) : ICommand;