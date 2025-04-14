using BuildingBlocks.Pagination;
using EStore.Application.Models.Files;

namespace EStore.Application.Queries.Files.GetFilesByUserName;

public record GetFilesByUserNameQuery(string UserName, GetFilesByUserNameRequest Request) : IQuery<AppResponse<PaginatedResult<FileEntityResult>>>;
