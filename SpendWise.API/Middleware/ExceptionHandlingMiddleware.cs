using System.Net;
using System.Text.Json;

namespace SpendWise.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly ErrorResponseFactory _errorResponseFactory;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            ErrorResponseFactory errorResponseFactory)
        {
            _next = next;
            _logger = logger;
            _errorResponseFactory = errorResponseFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _errorResponseFactory.CreateErrorResponse(
                    context.Response.StatusCode,
                    "An unexpected error occurred."
                );

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}