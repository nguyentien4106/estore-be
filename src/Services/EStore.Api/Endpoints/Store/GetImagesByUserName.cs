using BuildingBlocks.Models;
using Carter;
using Estore.Application.Dtos.Store;
using Estore.Application.Store.Commands;
using Estore.Application.Store.Queries;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Store;

public class GetImageByFileName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/store/images/{fileName}", async (string fileName, ISender sender) =>
        {
            var command = new GetImageByFileNameQuery(fileName);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Produces<AppResponse<string>>(StatusCodes.Status200OK)
        .WithName("GetImageByFileName")
        .WithTags("GetImageByFileName");
    }
}
