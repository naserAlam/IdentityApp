using API.Commands;
using API.Models;
using API.Repositories;
using API.Services;
using MediatR;

namespace API.Handlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly JWTService _jWTService;

        public RegisterUserCommandHandler(IUserRepository userRepository, JWTService jWTService)
        {
            _userRepository = userRepository;
            _jWTService = jWTService;
        }
        public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.CheckEmailExistsAsync(request.RegistrationRequest.Email))
                return false;

            var lastUserCode = await _userRepository.GetLastUserCodeAsync();
            int newCode = lastUserCode + 1;

            var userToAdd = new User(newCode,
                                 request.RegistrationRequest.FirstName.ToLower(),
                                 request.RegistrationRequest.LastName.ToLower(),
                                 request.RegistrationRequest.Email.ToLower(),
                                 request.RegistrationRequest.Email.ToLower(),
                                 true);
            var result = await _userRepository.CreateUserAsync(userToAdd, request.RegistrationRequest.Password);
            return result.Succeeded;
        }
    }
}
