using System.Threading.Tasks;

namespace EStore.Application.Services.RabbitMQ
{
    public interface IRabbitMQService
    {
        Task<bool> ProducerAsync(string message);

        Task<AppResponse<string>> ConsumerAsync(string message);
    }
}
