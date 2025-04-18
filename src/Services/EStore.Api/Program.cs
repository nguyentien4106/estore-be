using EStore.Api;
using EStore.Api.Extensions;
using EStore.Application;
using EStore.Application.Constants;
using EStore.Infrastructure;
using EStore.Infrastructure.Data.Extensions;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = null;  //disable the request body limit.
});

builder.Services
    .AddEStoreServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration);

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.UseEStoreApiServices();

app.MapGet("/", () => "EStore Microservice.");

app.Run();