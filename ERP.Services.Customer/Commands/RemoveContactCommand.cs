using ERP.Services.Shared.CQRS;

namespace ERP.Services.Customer.Commands;
public record RemoveContactCommand(int Id) : ICommand;
