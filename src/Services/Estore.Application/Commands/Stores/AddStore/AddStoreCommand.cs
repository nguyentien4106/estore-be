using MediatR;

namespace EStore.Services.Estore.Application.Commands.Stores.AddStore;

public class AddStoreCommand : IRequest<AddStoreResponse>
{
    public string ChannelName { get; }

    public AddStoreCommand(string channelName)
    {
        ChannelName = channelName;
    }
} 