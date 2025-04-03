using Estore.Application.Dtos.Files;
using Estore.Application.Services;
using EStore.Application.Data;
using Mapster;

namespace Estore.Application.Files.Commands.DeleteFile;

public class DeleteFileHandler(ICloudflareClient client, IEStoreDbContext context) : ICommandHandler<DeleteFileCommand, AppResponse<FileInformationDto>>
{
    public async Task<AppResponse<FileInformationDto>> Handle(DeleteFileCommand command, CancellationToken cancellationToken)
    {
        var file = await context.R2Files.FindAsync(command.Id, cancellationToken);
        if(file == null ){
            return AppResponse<FileInformationDto>.NotFound("File", command.Id);
        }

        context.R2Files.Remove(file);

        var result = await client.DeleteFileAsync(file.StorageFileName);
        if(result.Succeed){
            await context.CommitAsync(cancellationToken);
            return AppResponse<FileInformationDto>.Success(file.Adapt<FileInformationDto>());
        }

        return AppResponse<FileInformationDto>.Error(result.Message);
    }
}
