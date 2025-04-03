using Estore.Application.Helpers;
using Estore.Application.Services;
using Estore.Application.Services.Telegram;

namespace Estore.Application.Files.Commands.UploadFileTelegram;

public class UploadFileTelegramHandler(
    IEStoreDbContext context,
    UserManager<User> userManager,
    ITelegramService telegramService
    ) : ICommandHandler<UploadFileTelegramCommand, AppResponse<FileInformationDto>>
{
    public async Task<AppResponse<FileInformationDto>> Handle(UploadFileTelegramCommand command, CancellationToken cancellationToken)
    {
        var file = command.File;
        var user = await userManager.FindByNameAsync(command.UserName);
        if (user is null)
        {
            return AppResponse<FileInformationDto>.NotFound("User", command.UserName);
        }

        var result = await telegramService.UploadFileToStrorageAsync(file, user.Id);
        if(result.Succeed){
            await context.TeleFilesLocations.AddAsync(result.Data);
            await context.CommitAsync(cancellationToken);   
            return AppResponse<FileInformationDto>.Success(null);
        }

        return AppResponse<FileInformationDto>.Success(null);
    }
}
