using BuildingBlocks.Models;
using Carter;
using Estore.Application.Dtos.Store;
using Estore.Application.Store.Commands.DeleteImage;
using Estore.Application.Store.Commands.StoreImage;
using Microsoft.AspNetCore.Mvc;

namespace EStore.Api.Endpoints.Store;

public class DeleteImage : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/store/images", async (string fileName, ISender sender) =>
        {
            var command = new DeleteImageCommand(fileName);
            var result = await sender.Send(command);

            return Results.Ok(result);
        })
        .Produces<AppResponse<R2File>>(StatusCodes.Status201Created)
        .WithName("DeleteImage")
        .WithTags("DeleteImage");
    }
}
