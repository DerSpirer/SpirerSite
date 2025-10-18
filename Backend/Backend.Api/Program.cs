using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Backend.Api.Services.Agent;
using Backend.Api.Services.BlobStorage;
using Backend.Api.Services.Embedding;
using Backend.Api.Services.KnowledgeBase;
using Backend.Api.Services.VectorDatabase;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add OpenTelemetry with Azure Monitor
builder.Services.AddOpenTelemetry().UseAzureMonitor();

// Add CORS with public policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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
builder.Services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
builder.Services.AddScoped<IEmbeddingService, OpenAIEmbeddingService>();
builder.Services.AddScoped<IVectorDatabaseService, PineconeVectorDatabaseService>();
builder.Services.AddScoped<IKnowledgeBaseService, KnowledgeBaseService>();
builder.Services.AddScoped<IAgentService, OpenAIAgentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors();

// Default page
app.MapGet("/", () => Results.Ok(new
{
    status = "success",
    message = "Backend API is running",
    version = "1.0",
    timestamp = DateTime.UtcNow
}));

app.MapControllers();

// 404 handler - must be after all other routes
app.MapFallback(() => Results.NotFound(new
{
    status = "error",
    message = "Page not found",
    statusCode = 404
}));

await app.RunAsync();
