using Faice_Backend.Dtos;
using Faice_Backend.Entities;
using Faice_Backend.Enums;
using Faice_Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Faice_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountAppService accountAppService)
        : ControllerBase
{
    private readonly IAccountAppService _accountAppService = accountAppService;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        // only admins can access other user records
        var currentUser = (User)HttpContext.Items["User"];
        if (id != currentUser.Id && currentUser.Role != Role.Admin)
            return Unauthorized(new { message = "Unauthorized" });

        var user = await _accountAppService.GetByIdAsync(id);
        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequestDto input)
    {
        var response = await _accountAppService.LoginAsync(input);
        return Ok(response);
    }
}
