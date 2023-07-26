using API.DTOs.Account;
using API.Models;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserRepository _userRepository;

        public AccountController(JWTService jwtService,
            SignInManager<User> signInManager,
            IUserRepository userRepository)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            int itemsToSkip = (pageNumber - 1) * pageSize;
            int totalUsersCount = await _userRepository.GetTotalUsersCountAsync();

            var users = await _userRepository.GetPagedUsersAsync(pageNumber, pageSize);

            var response = new
            {
                TotalCount = totalUsersCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = users
            };

            return Ok(response);
        }

        [Authorize]
        [HttpGet("users/{code}")]
        public async Task<IActionResult> GetUserByCode(int code)
        {
            var user = await _userRepository.GetUserByCodeAsync(code);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefreshUserToken()
        {
            var user = await _userRepository.GetUserByEmailAsync(User.FindFirst(ClaimTypes.Email)?.Value);
            return CreateApplicationUserDto(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginRequest loginRequest)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginRequest.UserName);
            if (user == null)
                return Unauthorized("Invalid username or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid username or password");

            return CreateApplicationUserDto(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationRequest registrationRequest)
        {
            if (await _userRepository.CheckEmailExistsAsync(registrationRequest.Email))
                return BadRequest($"{registrationRequest.Email} already Exists!");

            var lastUserCode = await _userRepository.GetLastUserCodeAsync();
            int newCode = lastUserCode + 1;

            var userToAdd = new User
            {
                FirstName = registrationRequest.FirstName.ToLower(),
                LastName = registrationRequest.LastName.ToLower(),
                UserName = registrationRequest.Email.ToLower(),
                Email = registrationRequest.Email.ToLower(),
                Code = newCode,
                EmailConfirmed = true,
                LockoutEnabled = false
            };

            var result = await _userRepository.CreateUserAsync(userToAdd, registrationRequest.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Account created successfully");
        }

        #region Helper Methods
        private UserDto CreateApplicationUserDto(User user)
        {
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = _jwtService.CreateJWT(user),
            };
        }
        #endregion
    }
}
