using API.DTOs.Account;
using API.Models;
using API.Queries;
using API.Repositories;
using API.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace API.Handlers
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly JWTService _jWTService;

        public LoginQueryHandler(IUserRepository userRepository,
                                 SignInManager<User> signInManager,
                                 JWTService jWTService)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _jWTService = jWTService;
        }
        public async Task<UserDto> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.LoginRequest.UserName);
            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.LoginRequest.Password, false);
            if (result == null) return null;

            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = _jWTService.CreateJWT(user)
            };
        }
    }
}
