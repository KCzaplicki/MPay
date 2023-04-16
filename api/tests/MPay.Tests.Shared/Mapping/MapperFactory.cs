using AutoMapper;
using MPay.Core;

namespace MPay.Tests.Shared.Mapping;

internal static class MapperFactory
{
    public static IMapper Create()
    {
        var profiles = typeof(Extensions).Assembly.GetTypes()
            .Where(t => typeof(Profile).IsAssignableFrom(t))
            .Select(Activator.CreateInstance)
            .Cast<Profile>();

        var config = new MapperConfiguration(cfg => { cfg.AddProfiles(profiles); });

        return config.CreateMapper();
    }
}