using ERP.Services.Shared.CQRS;

namespace ERP.Services.User.Commands;
public record SignInCommand(string Email, string Password) : ICommand;