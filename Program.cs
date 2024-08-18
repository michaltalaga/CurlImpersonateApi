using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/curl", ([FromBody] CurlRequest request, ILogger<Program> logger) =>
{
    logger.LogInformation("Received a request: Method={Method}, Url={Url}, Headers={Headers}, Body={Body}",
            request.Method, request.Url, string.Join("; ", request.Headers?.Select(h => $"{h.Key}: {h.Value}") ?? Array.Empty<string>()), request.Body);
    var url = new Uri(request.Url).AbsoluteUri;
    var processInfo = new ProcessStartInfo
    {
        FileName = "curl_chrome116",
        Arguments = $"-X {request.Method} \"{url}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    foreach (var header in request.Headers ?? new())
    {
        processInfo.Arguments += $" -H \"{header.Key}: {header.Value}\"";
    }
    if (request.Body?.Length > 0)
    {
        processInfo.Arguments += $" -d \"{request.Body}\"";
    }

    var process = new Process { StartInfo = processInfo };
    process.Start();

    string result = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();

    process.WaitForExit();

    if (process.ExitCode != 0)
    {
        throw new Exception($"Error executing command: {error}");
    }

    return Results.Ok(result);
})
.WithName("curl")
.WithOpenApi();

app.Run();

public class CurlRequest
{
    public string Url { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
    public string? Method { get; set; } = "GET";
    public string? Body { get; set; }
}