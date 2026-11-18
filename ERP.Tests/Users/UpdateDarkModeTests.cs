using ERP.Services.User.Commands;
using ERP.Services.User.Commands.Handlers;
using Microsoft.EntityFrameworkCore;

namespace ERP.Tests.Users;

public class UpdateDarkModeTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateDarkMode_ShouldPersistPreferenceForCurrentUser(bool enabled)
    {
        using var context = TestDbContextFactory.Create();
        var user = new Model.Model.User
        {
            Email = "user@test.com",
            Password = "hashed-password",
            Role = (int)Model.Enum.UserRole.User
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new UpdateDarkModeHandler(context);

        await handler.HandleAsync(new UpdateDarkModeCommand(user.Id, enabled));

        var updatedUser = await context.Users.AsNoTracking().SingleAsync();
        Assert.Equal(enabled, updatedUser.DarkModeEnabled);
    }
}
