using MPay.Abstractions.Common;

namespace MPay.Tests.Shared.Common;

internal static class MockClockFactory
{
    public static Mock<IClock> Create(DateTime now = default, DateTime today = default)
    {
        var mock = new Mock<IClock>();
        mock.Setup(x => x.Now).Returns(now);
        mock.Setup(x => x.Today).Returns(today);
        
        return mock;
    }
}