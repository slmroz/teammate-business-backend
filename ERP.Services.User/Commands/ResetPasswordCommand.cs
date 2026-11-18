using ERP.Services.Shared.CQRS;

namespace ERP.Services.User.Commands;
public record ResetPasswordCommand(string Token, string NewPassword) : ICommand;