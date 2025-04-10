using CuentaClara.API.Models;
using System.Net;
using System.Text.Json;

namespace CuentaClara.API.Middleware
{
    /// <summary>
    /// manejar excepciones globales
    /// </summary>
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error no manejado");
                await HandleExceptionAsync(context, error);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500
            var result = ApiResult.Fail($"Error interno del servidor: {exception.Message}");

            // Puedes personalizar respuestas para diferentes tipos de excepciones
            if (exception is KeyNotFoundException)
            {
                code = HttpStatusCode.NotFound; // 404
                result = ApiResult.Fail("Recurso no encontrado");
            }
            else if (exception is UnauthorizedAccessException)
            {
                code = HttpStatusCode.Unauthorized; // 401
                result = ApiResult.Fail("No autorizado");
            }
            else if (exception is ArgumentException || exception is FormatException)
            {
                code = HttpStatusCode.BadRequest; // 400
                result = ApiResult.Fail($"Solicitud inválida: {exception.Message}");
            }

            // Establece el tipo de contenido y el código de estado
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            // Serializa el resultado a JSON
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var json = JsonSerializer.Serialize(result, options);

            return context.Response.WriteAsync(json);
        }
    }
}
