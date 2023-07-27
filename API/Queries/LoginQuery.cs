using API.DTOs.Account;
using MediatR;

namespace API.Queries
{
    public record LoginQuery(LoginRequestDto LoginRequest) : IRequest<UserDto>;
}
