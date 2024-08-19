using System.Text;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerFactory _loggerFactory;
    public RequestResponseLoggingMiddleware(RequestDelegate next,  ILoggerFactory loggerFactory)
    {
        _next = next;
        _loggerFactory = loggerFactory;
    }

    public async Task Invoke(HttpContext context)
    {
        var logger = _loggerFactory.CreateLogger("RequestLogger");
        //First, get the incoming request
        var request = await FormatRequest(context.Request);
        logger.LogInformation($"Request: {request}");
        //Copy a pointer to the original response body stream
        var originalBodyStream = context.Response.Body;

        //Create a new memory stream...
        using (var responseBody = new MemoryStream())
        {
            //...and use that for the temporary response body
            context.Response.Body = responseBody;

            //Continue down the Middleware pipeline, eventually returning to this class
            await _next(context);

            //Format the response from the server
            var response = await FormatResponse(context.Response);
            logger.LogInformation($"Response: {response}");
            //TODO: Save log to chosen datastore

            //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task<string> FormatRequest(HttpRequest request)
    {
        string bodyAsText = "";
        //We convert the byte[] into a string using UTF8 encoding...
        request.EnableBuffering(); // Enable buffering, so the body can be read multiple times
        bodyAsText = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Seek(0, SeekOrigin.Begin);
        return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText}";
    }

    private async Task<string> GetRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering(); // Enable buffering, so the body can be read multiple times
        using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true))
        {
            string requestBody = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin); // Reset the stream position to enable subsequent reads
            return requestBody;
        }
    }

    private async Task<string> FormatResponse(HttpResponse response)
    {
        //We need to read the response stream from the beginning...
        response.Body.Seek(0, SeekOrigin.Begin);

        //...and copy it into a string
        string text = await new StreamReader(response.Body).ReadToEndAsync();

        //We need to reset the reader for the response so that the client can read it.
        response.Body.Seek(0, SeekOrigin.Begin);

        //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
        return $"{response.StatusCode}: {text}";
    }
}