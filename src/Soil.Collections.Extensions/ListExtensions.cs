using System.Collections.Generic;

namespace Soil.Collections.Extensions;

public static class ListExtensions
{
    public static T? GetSafe<T>(this List<T> list, int index, T? defaultValue = default)
    {
        if (list == null)
        {
            return defaultValue;
        }

        if (index < 0 || index >= list.Count)
        {
            return defaultValue;
        }

        return list[index];
    }
}
