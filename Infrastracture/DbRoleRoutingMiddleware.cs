public class DbRoleRoutingMiddleware
{
    private readonly RequestDelegate _next;

    public DbRoleRoutingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Если запрос на чтение (GET) — направляем на слейвы
        if (HttpMethods.IsGet(context.Request.Method))
        {
            DbConnectionContext.CurrentRole = DbNodeRole.Slave;
        }
        else
        {
            DbConnectionContext.CurrentRole = DbNodeRole.Master;
        }

        await _next(context);
    }
}

// Экстеншн для удобного подключения
public static class DbRoleRoutingMiddlewareExtensions
{
    public static IApplicationBuilder UseDbRoleRouting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<DbRoleRoutingMiddleware>();
    }
}