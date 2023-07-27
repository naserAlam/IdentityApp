using API.DTOs.Account;
using MediatR;

namespace API.Commands
{
    public record RegisterUserCommand(RegistrationRequestDto RegistrationRequest) : IRequest<bool>;
}
