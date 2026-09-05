namespace ClienteService.Infrastructure.Middlewares;

using System.Net;
using System.Text.Json;
using ClienteService.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "Excepción no controlada: {Message}", ex.Message);

        HttpResponse response = context.Response;
        response.ContentType = "application/json";

        object error;
        switch (ex)
        {
            case NotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                error = new { mensaje = ex.Message };
                break;
            case SaldoNoDisponibleException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                error = new { mensaje = ex.Message };
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                error = new { mensaje = "Ocurrió un error interno.", detalle = ex.Message };
                break;
        }

        string json = JsonSerializer.Serialize(error);
        await response.WriteAsync(json);
    }
}
