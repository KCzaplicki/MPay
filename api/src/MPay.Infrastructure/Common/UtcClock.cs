using MPay.Abstractions.Common;

namespace MPay.Infrastructure.Common;

internal class UtcClock : IClock
{
    public DateTime Now => DateTime.UtcNow;

    public DateTime Today => DateTime.UtcNow.Date;
}