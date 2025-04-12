using EStore.Application.Extensions;
using EStore.Application.Models.Files;
using EStore.Application.Services.Telegram;

namespace EStore.Application.Commands.Files.UploadFile;

public class UploadFileTelegramHandler(
    IEStoreDbContext context,
    UserManager<User> userManager,
    ITelegramService telegramService
    ) : ICommandHandler<UploadFileTelegramCommand, AppResponse<FileEntityResult>>
{
    public async Task<AppResponse<FileEntityResult>> Handle(UploadFileTelegramCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(command.UserName);
        if (user is null)
        {
            return AppResponse<FileEntityResult>.NotFound("User", command.UserName);
        }

        var result = await telegramService.UploadFileToStrorageAsync(command, user.Id);

        if(result.Succeed){
            await context.TeleFileEntities.AddAsync(result.Data, cancellationToken);
            await context.CommitAsync(cancellationToken);   
            return AppResponse<FileEntityResult>.Success(result.Data.ToFileEntityResponse());
        }

        return AppResponse<FileEntityResult>.Error(result.Message);
    }
}
