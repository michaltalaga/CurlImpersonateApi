using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/curl", ([FromBody] CurlRequest request) =>
{
    return JsonSerializer.Serialize(request);
})
.WithName("curl")
.WithOpenApi();

app.Run();

public class CurlRequest
{
    public string Url { get; set; }
    public Dictionary<string, string> Headers { get; set; }
}