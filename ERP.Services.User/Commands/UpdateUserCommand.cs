using ERP.Services.Shared.CQRS;

namespace ERP.Services.User.Commands;
public record UpdateUserCommand(int Id, int Role, string Email, string FirstName, string LastName) : ICommand;