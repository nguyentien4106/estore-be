using EStore.Application.Services.Telegram;

namespace EStore.Application.Commands.Files.DeleteFile;

public class DeleteFileTelegramHandler(ITelegramService telegramService, IEStoreDbContext context) : ICommandHandler<DeleteFileTelegramCommand, AppResponse<Guid>>
{
    public async Task<AppResponse<Guid>> Handle(DeleteFileTelegramCommand command, CancellationToken cancellationToken)
    {
        var file = await context.TeleFileEntities.FindAsync(command.Id);
        if (file is null)
        {
            return AppResponse<Guid>.NotFound("File", command.Id);
        }

        var result = await telegramService.DeleteMessageAsync(file.MessageId ?? -1);
        if (result.Succeed){
            context.TeleFileEntities.Remove(file);
            await context.CommitAsync(cancellationToken);
            return AppResponse<Guid>.Success(command.Id);
        }
        
        return AppResponse<Guid>.Error(result.Message);
    }
}
