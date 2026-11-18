using ERP.Services.Shared.CQRS;

namespace ERP.Services.User.Commands;
public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : ICommand;
