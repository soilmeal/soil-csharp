using System.Diagnostics.CodeAnalysis;

namespace Soil.Net.Channel.Configuration;

public interface IChannelConfigurationSection
{
    void AddSection(IReadOnlyChannelConfigurationSection section);

    bool TryAddSection([NotNullWhen(true)] IReadOnlyChannelConfigurationSection section);

    TSection GetSection<TSection>()
        where TSection : IReadOnlyChannelConfigurationSection;

    bool TryGetSection<TSection>([NotNullWhen(true)] out TSection section)
        where TSection : IReadOnlyChannelConfigurationSection;
}

public interface IChannelConfigurationSection<T> : IChannelConfigurationSection
    where T : IChannelConfigurationSection<T>
{
}

