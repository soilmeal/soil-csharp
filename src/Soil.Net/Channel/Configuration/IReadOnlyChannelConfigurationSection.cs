using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Soil.Net.Channel.Configuration;

public interface IReadOnlyChannelConfigurationSection
{
    TSection GetSection<TSection>()
        where TSection : IReadOnlyChannelConfigurationSection;

    bool TryGetSection<TSection>([NotNullWhen(true)] out TSection section)
        where TSection : IReadOnlyChannelConfigurationSection;
}

public interface IReadOnlyChannelConfigurationSection<T> : IReadOnlyChannelConfigurationSection
    where T : IReadOnlyChannelConfigurationSection<T>
{
}
