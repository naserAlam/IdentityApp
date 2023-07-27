using API.Commands;
using API.DTOs.Account;
using API.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            var query = new GetAllUsersQuery(pageNumber, pageSize);
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("users/{code}")]
        public async Task<IActionResult> GetUserByCode(int code)
        {
            var response = await _mediator.Send(new GetUserByCodeQuery(code));
            if (response == null) return NotFound();
            return Ok(response);
        }

        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefreshUserToken()
        {
            // Get the logged-in user's email from the claim
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var query = new RefreshUserTokenQuery(email);
            var userDto = await _mediator.Send(query);

            return userDto;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginRequestDto loginRequest)
        {
            var userDto = await _mediator.Send(new LoginQuery(loginRequest));
            if (userDto == null)
                return Unauthorized("Invalid username or password");
            return Ok(userDto);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequest)
        {
            bool isRegistered = await _mediator.Send(new RegisterUserCommand(registrationRequest));
            if (!isRegistered) return BadRequest($"{registrationRequest.Email} already exists");

            return Ok("Account created successfully");
        }
    }
}
