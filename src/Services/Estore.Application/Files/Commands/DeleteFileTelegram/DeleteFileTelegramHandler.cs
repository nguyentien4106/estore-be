using Estore.Application.Dtos.Files;
using Estore.Application.Services;
using EStore.Application.Data;
using Mapster;

namespace Estore.Application.Files.Commands.DeleteFileTelegram;

public class DeleteFileTelegramHandler(IEStoreDbContext context, ITelegramService telegramService) : ICommandHandler<DeleteFileTelegramCommand, AppResponse<FileInformationDto>>
{
    public async Task<AppResponse<FileInformationDto>> Handle(DeleteFileTelegramCommand command, CancellationToken cancellationToken)
    {
        var file = await context.R2Files.FindAsync(command.Id, cancellationToken);
        if(file == null ){
            return AppResponse<FileInformationDto>.NotFound("File", command.Id);
        }

        context.R2Files.Remove(file);

        var result = await telegramService.DeleteMessageAsync(long.Parse(file.Url));
        if(result.Succeed){
            await context.CommitAsync(cancellationToken);
            return AppResponse<FileInformationDto>.Success(file.Adapt<FileInformationDto>());
        }

        return AppResponse<FileInformationDto>.Error(result.Message);
    }
}
