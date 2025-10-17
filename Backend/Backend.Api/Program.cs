using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Backend.Api.Services.Agent;
using Backend.Api.Services.Embedding;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add OpenTelemetry with Azure Monitor
builder.Services.AddOpenTelemetry().UseAzureMonitor();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();

// Register Secrets Service
string keyVaultUrl = builder.Configuration["AZURE_KEY_VAULT_URL"] ?? throw new InvalidOperationException("AZURE_KEY_VAULT_URL is not set");
builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), new DefaultAzureCredential());
builder.Services.AddScoped<IEmbeddingService, OpenAIEmbeddingService>();
builder.Services.AddScoped<IAgentService, OpenAIAgentService>();

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
