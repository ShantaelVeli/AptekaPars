using Microsoft.Extensions.Options;
using mservis.Models.Settings;

namespace mservis.MiddleWare;

public class ApiKeyAuth
{
    private readonly RequestDelegate _next;

    private readonly ApiKeys _keys;

    public ApiKeyAuth(IOptions<ApiKeys> settings, RequestDelegate next)
    {
        _keys = settings.Value;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var req = context.Request;
        req.EnableBuffering();

        if (req.Method == HttpMethods.Post && req.HttpContext.GetRouteValue("controller")=="Data")
        {
            var keys = await req.ReadFromJsonAsync<ApiKeys>();

            if (!req.HasJsonContentType() || string.IsNullOrEmpty(keys?.App.AppSecret.ToString()))
            {
                await GetResponse(context, 401, "Api keys missing");
                return;
            }

            if (!ApiKeysComparator.IsEqual(keys, _keys))
            {
                await GetResponse(context, 401, "Invalid Api-key");
                return;
            }
        }

        req.Body.Position = 0;
        await _next(context);
    }

    private async Task GetResponse(HttpContext context, int code, string message)
    {
        context.Response.StatusCode = code;
        await context.Response.WriteAsync(message);
    }
}