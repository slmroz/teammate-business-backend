using ERP.Services.Shared.CQRS;

namespace ERP.Services.Customer.Commands;
public sealed record RemoveCustomerCommand(int Id) : ICommand;