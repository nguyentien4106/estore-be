using EStore.Application.Data;
using Carter;

namespace EStore.Api.Endpoints;

public class GetProfile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/profile/{userName}", async (IEStoreDbContext dbContext, string userName) =>
        {
            var user = dbContext.Users.FirstOrDefault(item => item.UserName == userName);
            return user?.UserName ?? "Profile";
        })
        .WithName("GetProfile")
        .Produces(StatusCodes.Status202Accepted)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
        
    }
}