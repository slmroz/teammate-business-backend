using ERP.Services.Shared.CQRS;

namespace ERP.Services.User.Commands;
public record SignUpCommand(string Email, string Password, string? FirstName, string? LastName, int? Role) : ICommand;