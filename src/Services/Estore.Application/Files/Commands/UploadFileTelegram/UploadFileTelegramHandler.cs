﻿using EStore.Application.Extensions;
using EStore.Application.Services.Telegram;
using EStore.Domain.Models.Base;

namespace EStore.Application.Files.Commands.UploadFileTelegram;

public class UploadFileTelegramHandler(
    IEStoreDbContext context,
    UserManager<User> userManager,
    ITelegramService telegramService
    ) : ICommandHandler<UploadFileTelegramCommand, AppResponse<FileEntityResponse>>
{
    public async Task<AppResponse<FileEntityResponse>> Handle(UploadFileTelegramCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(command.UserName);
        if (user is null)
        {
            return AppResponse<FileEntityResponse>.NotFound("User", command.UserName);
        }

        var result = await telegramService.UploadFileToStrorageAsync(command, user.Id);

        if(result.Succeed){
            await context.TeleFileEntities.AddAsync(result.Data, cancellationToken);
            await context.CommitAsync(cancellationToken);   
            return AppResponse<FileEntityResponse>.Success(result.Data.ToFileEntityResponse());
        }

        return AppResponse<FileEntityResponse>.Error(result.Message);
    }
}
