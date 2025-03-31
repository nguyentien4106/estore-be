using EStore.Api;
using EStore.Application;
using EStore.Infrastructure;
using EStore.Infrastructure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddEStoreServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.UseEStoreApiServices();

app.MapGet("/", () => "EStore Microservice.");

app.Run();