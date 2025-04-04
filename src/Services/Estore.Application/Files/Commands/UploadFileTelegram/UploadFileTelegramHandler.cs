using Estore.Application.Services.Telegram;

namespace Estore.Application.Files.Commands.UploadFileTelegram;

public class UploadFileTelegramHandler(
    IEStoreDbContext context,
    UserManager<User> userManager,
    ITelegramService telegramService
    ) : ICommandHandler<UploadFileTelegramCommand, AppResponse<TeleFileLocation>>
{
    public async Task<AppResponse<TeleFileLocation>> Handle(UploadFileTelegramCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(command.UserName);
        if (user is null)
        {
            return AppResponse<TeleFileLocation>.NotFound("User", command.UserName);
        }

        var result = await telegramService.UploadFileToStrorageAsync(command, user.Id);

        if(result.Succeed){
            await context.TeleFilesLocations.AddAsync(result.Data, cancellationToken);
            await context.CommitAsync(cancellationToken);   
            return AppResponse<TeleFileLocation>.Success(result.Data);
        }

        return AppResponse<TeleFileLocation>.Error(result.Message);
    }
}
