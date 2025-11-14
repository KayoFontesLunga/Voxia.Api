namespace VoxiasApp.Utils.Middlewares;

public class AuthLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthLoggingMiddleware> _logger;

    public AuthLoggingMiddleware(RequestDelegate next, ILogger<AuthLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // se o usuário estiver autenticado tenta extrair o id
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.GetUserId();
            var email = context.User.GetEmail();
            if (userId != System.Guid.Empty)
            {
                _logger.LogInformation("Request authenticated. UserId: {UserId}, Email: {Email}, Path: {Path}",
                    userId, email ?? "[sem-email-claim]", context.Request.Path);
            }
        }

        await _next(context);
    }
}