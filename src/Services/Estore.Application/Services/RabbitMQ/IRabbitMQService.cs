using System.Threading.Tasks;

namespace EStore.Application.Services.RabbitMQ
{
    public interface IRabbitMQService
    {
        Task<bool> ProducerAsync(string queueName, string message);

        Task<bool> ProducerAsync(string queueName, object data);

        Task MergeFileConsumerAsync(string message);

        Task PushFileConsumerAsync(string message);
    }
}
