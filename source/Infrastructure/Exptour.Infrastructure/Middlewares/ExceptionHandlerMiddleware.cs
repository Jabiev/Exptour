using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace Exptour.Infrastructure.Middlewares;

public static class ExceptionHandlerMiddleware
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = MediaTypeNames.Application.Json;

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contextFeature is not null)
                {
                    var errorId = Guid.NewGuid();

                    var errorResponse = new
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Unexpected error occurred. Please contact the support team.",
                        Detailed = contextFeature.Error.Message,
                        ErrorId = errorId,
                        Title = "Internal Server Error"
                    };

                    var errorJson = JsonSerializer.Serialize(errorResponse);

                    await context.Response.WriteAsync(errorJson);
                }
            });
        });
    }
}
