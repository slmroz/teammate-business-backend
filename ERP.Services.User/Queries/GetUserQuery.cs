using ERP.Services.Shared.CQRS;
using ERP.Services.User.DTO;

namespace ERP.Services.User.Queries;
public class GetUserQuery : IQuery<UserDto>
{
    public int UserId { get; set; }
}