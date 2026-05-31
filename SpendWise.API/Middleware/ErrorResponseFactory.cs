namespace SpendWise.API.Middleware
{
    public class ErrorResponseFactory
    {
        public object CreateErrorResponse(int statusCode, string message)
        {
            return new
            {
                StatusCode = statusCode,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}