using ERP.Model.Model;
using ERP.Services.Shared.CQRS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Services.User.Commands.Handlers;

internal sealed class UpdateDarkModeHandler : ICommandHandler<UpdateDarkModeCommand>
{
    private readonly ErpContext _dbContext;

    public UpdateDarkModeHandler(ErpContext dbContext)
        => _dbContext = dbContext;

    public async Task HandleAsync(UpdateDarkModeCommand command)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == command.UserId && user.RemovedAt == null);

        if (user is null)
            throw new KeyNotFoundException("User not found");

        user.DarkModeEnabled = command.Enabled;
        await _dbContext.SaveChangesAsync();
    }
}
