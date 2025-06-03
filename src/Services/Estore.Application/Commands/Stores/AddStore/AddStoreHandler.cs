using EStore.Application.Queries.Stores.GetAllStores;
using EStore.Application.Services.Telegram;
using Mapster;
using MediatR;

namespace EStore.Application.Commands.Stores.AddStore;

public class AddStoreHandler(ITelegramService telegramService, IEStoreDbContext dbContext) 
: IRequestHandler<AddStoreCommand, AppResponse<StoreDto>>
{
    public async Task<AppResponse<StoreDto>> Handle(AddStoreCommand command, CancellationToken cancellationToken)
    {
        var result = await telegramService.CreateNewChannelAsync(command.ChannelName, command.Description, cancellationToken);
        if(!result.Succeed){
            return AppResponse<StoreDto>.Error(result.Message);
        }

        var store = new Store
        {
            Description = command.Description,
            ChannelName = command.ChannelName,
            ChannelId = result.Data
        };

        dbContext.Stores.Add(store);
        
        await dbContext.CommitAsync(cancellationToken); 

        return AppResponse<StoreDto>.Success(store.Adapt<StoreDto>(), "Store created successfully");
    }
} 