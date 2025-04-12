using BuildingBlocks.Pagination;
using EStore.Application.Models.Files;

namespace EStore.Application.Queries.Files.GetFilesByUserName;

public record GetFilesByUserNameQuery(string UserName, PaginationRequest PaginationRequest) : IQuery<AppResponse<PaginatedResult<FileEntityResult>>>;
