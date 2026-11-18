using ERP.Services.Shared.CQRS;

namespace ERP.Services.Sales.Commands;
public record RemoveQuoteCommand(int Id, int RemovedBy) : ICommand;