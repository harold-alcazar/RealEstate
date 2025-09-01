using System.Net;
using System.Text.Json;
using RealEstate.Application.Exceptions;

namespace RealEstate.Api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
            catch(Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger logger)
        {
            context.Response.ContentType = "application/json";

            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = "Ha ocurrido un error en el servidor.";

            switch(ex)
            {
                case BadRequestException:
                statusCode = HttpStatusCode.BadRequest;
                message = ex.Message;
                break;

                case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = ex.Message;
                break;

                case UnauthorizedException:
                statusCode = HttpStatusCode.Unauthorized;
                message = ex.Message;
                break;

                default:
                logger.LogError(ex, "Error inesperado");
                break;
            }

            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                success = false,
                message,
                detail = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
