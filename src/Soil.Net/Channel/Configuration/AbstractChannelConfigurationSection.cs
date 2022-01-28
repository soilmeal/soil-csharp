using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Soil.Net.Channel.Configuration;

public abstract class AbstractChannelConfigurationSection<TDerived> : IChannelConfigurationSection<TDerived>
    where TDerived : IChannelConfigurationSection<TDerived>
{
    protected abstract IDictionary<string, IReadOnlyChannelConfigurationSection> Sections { get; }

    protected static string GetFullName(IReadOnlyChannelConfigurationSection section)
    {
        return section.GetType().FullName ?? "";
    }

    protected static string GetFullName<TSection>()
    {
        return typeof(TSection).FullName ?? "";
    }

    public virtual void AddSection(IReadOnlyChannelConfigurationSection section)
    {
        if (section == null)
        {
            throw new ArgumentNullException(nameof(section));
        }

        string name = GetFullName(section);
        Sections.Add(name, section);
    }

    public virtual bool TryAddSection([NotNullWhen(true)] IReadOnlyChannelConfigurationSection section)
    {
        if (section == null)
        {
            return false;
        }

        string name = GetFullName(section);
        return Sections.TryAdd(name, section);
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

        section = (TSection)tmpSection;
        return section != null;
    }
}
