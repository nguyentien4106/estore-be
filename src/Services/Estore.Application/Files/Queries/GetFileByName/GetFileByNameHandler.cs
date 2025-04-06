using EStore.Application.Dtos.Files;
using EStore.Application.Data;
using Microsoft.EntityFrameworkCore;
using Mapster;
namespace EStore.Application.Files.Queries.GetFileByName;

public class GetFileByNameHandler(IEStoreDbContext context) : IQueryHandler<GetFileByNameQuery, AppResponse<FileInformationDto>>
{
    public async Task<AppResponse<FileInformationDto>> Handle(GetFileByNameQuery query, CancellationToken cancellationToken)
    {
        // var file = await context.R2Files.FirstOrDefaultAsync(item => item.FileName == query.FileName, cancellationToken);
        //
        // if(file is null)
        // {
        //     return AppResponse<FileInformationDto>.NotFound("File", query.FileName);
        // }
        //
        // var dto = file.Adapt<FileInformationDto>();

        return AppResponse<FileInformationDto>.Success(null);
    }
}
