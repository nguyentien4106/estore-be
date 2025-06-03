using EStore.Application.Queries.Stores.GetAllStores;

namespace EStore.Application.Commands.Stores.AddStore;

public class AddStoreCommand : ICommand<AppResponse<StoreDto>>
{
    public string? Description { get; set; }
    public string ChannelName { get; set; }
} 