using EStore.Application.Models.Dashboard;

namespace EStore.Application.Queries.Dashboard;

public record GetUserStorageQuery(string UserName) : IQuery<AppResponse<UserStorageDto>>; 