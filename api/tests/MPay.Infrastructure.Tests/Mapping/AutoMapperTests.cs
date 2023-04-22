using AutoMapper;

namespace MPay.Infrastructure.Tests.Mapping;

public class AutoMapperTests
{
    [Fact]
    public void Configuration_IsValid()
    {
        // Arrange
        var profiles = typeof(Extensions).Assembly.GetTypes()
            .Where(t => typeof(Profile).IsAssignableFrom(t))
            .Select(Activator.CreateInstance)
            .Cast<Profile>();
        
        // Act
        var config = new MapperConfiguration(cfg => cfg.AddProfiles(profiles));
        
        // Assert
        config.AssertConfigurationIsValid();
    }
}