using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Soil.Net.Channel.Configuration;

public abstract class AbstractReadOnlyConfigurationSection<TDerived> : IReadOnlyChannelConfigurationSection<TDerived>
    where TDerived : IReadOnlyChannelConfigurationSection<TDerived>
{
    protected abstract IReadOnlyDictionary<string, IReadOnlyChannelConfigurationSection> Sections { get; }

    protected static string GetFullName<TSection>()
    {
        return typeof(TSection).FullName!;
    }

    public virtual TSection GetSection<TSection>()
        where TSection : IReadOnlyChannelConfigurationSection
    {
        string name = GetFullName<TSection>();
        return (TSection)Sections[name];
    }

    public virtual bool TryGetSection<TSection>([NotNullWhen(true)] out TSection section)
        where TSection : IReadOnlyChannelConfigurationSection
    {
        string name = GetFullName<TSection>();
        IReadOnlyChannelConfigurationSection? tmpSection;
        if (!Sections.TryGetValue(name, out tmpSection))
        {
            section = default!;
            return false;
        }

        section = (TSection)Sections;
        return section != null;
    }
}
