using ERP.Services.Shared.CQRS;

namespace ERP.Services.User.Commands;
public record ForgotPasswordCommand(string Email) : ICommand;