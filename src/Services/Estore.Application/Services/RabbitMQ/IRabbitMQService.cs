using System.Threading.Tasks;

namespace EStore.Application.Services.RabbitMQ
{
    public interface IRabbitMQService
    {
        Task<bool> ProducerAsync(string queueName, string message);

        Task MergeFileConsumerAsync(string message);

        Task PushFileConsumerAsync(string message);
    }
}
