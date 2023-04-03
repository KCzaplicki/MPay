namespace MPay.Abstractions.Common;

public interface IClock
{
    DateTime Now { get; }
}