using ERP.Services.Shared.CQRS;

namespace ERP.Services.User.Commands;

public record UpdateDarkModeCommand(int UserId, bool Enabled) : ICommand;
