using Microsoft.AspNetCore.Http;

namespace e_commerce.sharedlibrary.Middleware
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await next(context);
                return;
            }

            var signedHeader = context.Request.Headers["Api-Gateway"];
            if(signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry, service is unavailable");
                return;
            }
                await next(context);

        }
    }
}
