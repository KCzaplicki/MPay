using System.Threading.Channels;
using MPay.Abstractions.Events;

namespace MPay.Infrastructure.Events;

internal class EventChannel : IEventChannel
{
    private readonly Channel<IEvent> _channel = Channel.CreateUnbounded<IEvent>();

    public ChannelReader<IEvent> Reader => _channel.Reader;
    public ChannelWriter<IEvent> Writer => _channel.Writer;
}