using ERP.Services.Shared.Exceptions;

namespace ERP.Services.User.Exceptions;
public sealed class UserAlreadyExistsException : CustomException
{
    public UserAlreadyExistsException(string email)
        : base($"User {email} already exists")
    {
    }
}
