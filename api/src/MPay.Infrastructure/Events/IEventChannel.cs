using System.Threading.Channels;
using MPay.Abstractions.Events;

namespace MPay.Infrastructure.Events;

public interface IEventChannel
{
    ChannelReader<IEvent> Reader { get; }
    ChannelWriter<IEvent> Writer { get; }
}