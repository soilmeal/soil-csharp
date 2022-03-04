using System;
using System.Collections.Generic;
using System.Linq;

namespace Soil.Collections.Extensions;

public static class ListExtensions
{
    public static T? GetSafe<T>(this IList<T> list, int index, T? defaultValue = default)
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

    public static T? GetSafe<T>(this T[] arr, int index, T? defaultValue = default)
    {
        if (arr == null)
        {
            return defaultValue;
        }

        if (index < 0 || index >= arr.Length)
        {
            return defaultValue;
        }

        return arr[index];
    }
}
