using EStore.Domain.Abstractions;

namespace EStore.Domain.Models;

public class Store : Entity<Guid>
{
    public long ChannelId { get; set; }

    public string? Description { get; set; }

    public string ChannelName { get; set; }

    public long MessageCount { get; set; }
}

