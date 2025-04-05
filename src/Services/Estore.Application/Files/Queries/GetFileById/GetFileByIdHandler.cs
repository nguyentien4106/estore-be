using Estore.Application.Dtos.Files;
using EStore.Application.Data;
using Mapster;
using Estore.Application.Files.Queries.GetFileById;
namespace Estore.Application.Files.Queries.GetFileByName;

public class GetFileByIdHandler(IEStoreDbContext context) : IQueryHandler<GetFileByIdQuery, AppResponse<FileInformationDto>>
{
    public async Task<AppResponse<FileInformationDto>> Handle(GetFileByIdQuery query, CancellationToken cancellationToken)
    {
        // var file = await context.R2Files.FindAsync(query.Id);
        //
        // if(file is null)
        // {
        //     return AppResponse<FileInformationDto>.NotFound("File", query.Id);
        // }
        //
        // var dto = file.Adapt<FileInformationDto>();

        return AppResponse<FileInformationDto>.Success(null);
    }
}
