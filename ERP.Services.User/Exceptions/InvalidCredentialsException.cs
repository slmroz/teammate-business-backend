using ERP.Services.Shared.Exceptions;

namespace ERP.Services.User.Exceptions;

public sealed class InvalidCredentialsException : CustomException
{
    public InvalidCredentialsException()
        : base("Invalid email or password.")
    {
    }
}
