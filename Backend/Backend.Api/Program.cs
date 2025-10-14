using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add OpenTelemetry with Azure Monitor
builder.Services.AddOpenTelemetry().UseAzureMonitor();

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Register Secrets Service
string keyVaultUrl = builder.Configuration["AZURE_KEY_VAULT_URL"] ?? throw new InvalidOperationException("AZURE_KEY_VAULT_URL is not set");
builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
