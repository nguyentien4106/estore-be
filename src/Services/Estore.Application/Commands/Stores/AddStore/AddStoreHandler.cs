using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EStore.Services.Estore.Application.Commands.Stores.AddStore;

public class AddStoreCommandHandler : IRequestHandler<AddStoreCommand, AddStoreResponse>
{
    // Assuming you have a repository or service to save store data
    // private readonly IStoreRepository _storeRepository;

    // public AddStoreCommandHandler(IStoreRepository storeRepository)
    // {
    //     _storeRepository = storeRepository;
    // }

    public async Task<AddStoreResponse> Handle(AddStoreCommand request, CancellationToken cancellationToken)
    {
        // Implement logic to add the new store
        // For example, create a new Store entity and save it
        // var newStore = new Store { ChannelName = request.ChannelName, /* other properties */ };
        // var createdStore = await _storeRepository.AddStoreAsync(newStore);

        // For now, returning a dummy response
        // Replace with actual data saving and response generation
        await Task.Delay(100); // Simulate async work

        return new AddStoreResponse
        {
            StoreId = new System.Random().Next(1, 1000), // Dummy Store ID
            Status = "Store created successfully.",
            ChannelName = request.ChannelName
        };
    }
} 