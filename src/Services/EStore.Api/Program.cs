using EStore.Api;
using EStore.Api.Extensions;
using EStore.Application;
using EStore.Infrastructure;
using EStore.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = null;  //disable the request body limit.
});

builder.Services
    .AddEStoreServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddCustomWebhookServices();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.UseEStoreApiServices();

app.MapGet("/", () => "EStore Microservice.");

app.Run();