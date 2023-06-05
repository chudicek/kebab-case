using Kebabos.Contracts.User;
using Microsoft.AspNetCore.Mvc;
using Kebabos.Services.User;
using Kebabos.Services;

namespace Kebabos.Controllers;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost()]
    public async Task<IActionResult> CreateUser(UserCreateRequest request)
    => (await _userService.CreateUser(request)).process(
        user => CreatedAtAction(nameof(CreateUser), new { id = user.Id }, user),
        Common.handle
    );

    [HttpGet()]
    public async Task<IActionResult> GetUser()
    => (await _userService.GetUsers()).process(
        users => Ok(users),
        Common.handle
    );

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    => (await _userService.GetUserById(id)).process(
        user => Ok(user),
        Common.handle
    );
}