using Estore.Application.Services.Telegram;

namespace Estore.Application.Files.Commands.DeleteFileTelegram;

public class DeleteFileTelegramHandler(ITelegramService telegramService, IEStoreDbContext context) : ICommandHandler<DeleteFileTelegramCommand, AppResponse<Guid>>
{
    public async Task<AppResponse<Guid>> Handle(DeleteFileTelegramCommand command, CancellationToken cancellationToken)
    {
        var file = await context.TeleFileEntities.FindAsync(command.Id);
        if (file is null)
        {
            return AppResponse<Guid>.NotFound("File", command.Id);
        }

        var result = await telegramService.DeleteMessageAsync(file.MessageId);
        if (result.Succeed){
            context.TeleFileEntities.Remove(file);
            await context.CommitAsync(cancellationToken);
            return AppResponse<Guid>.Success(command.Id);
        }
        
        return AppResponse<Guid>.Error(result.Message);
    }
}
