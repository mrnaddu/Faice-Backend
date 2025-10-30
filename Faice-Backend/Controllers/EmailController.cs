using Faice_Backend.Dtos;
using Faice_Backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Faice_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailController(IEmailAppService emailAppService) : ControllerBase
{
    private readonly IEmailAppService _emailAppService = emailAppService;

    [HttpPost()]
    [Route("send-test-email")]
    public Task SendTestEmailAsync(SendTestEmailDto input)
    {
        return _emailAppService.SendEmailAsync(input.Email, input.Subject, input.Body);
    }
}
