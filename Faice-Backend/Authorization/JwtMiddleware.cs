using Faice_Backend.Interfaces;

namespace Faice_Backend.Authorization;

public class JwtMiddleware(
    RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(
        HttpContext context, 
        IAccountAppService AccountAppService,
        IJwtUtilsService jwtUtilsService)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
        var userId = jwtUtilsService.ValidateJwtToken(token);
        if (userId != null)
        {
            // attach user to context on successful jwt validation
            context.Items["User"] = AccountAppService.GetById(userId.Value);
        }

        await _next(context);
    }
}
