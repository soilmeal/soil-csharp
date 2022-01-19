using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Soil.Types;

public class MethodInfoHelper
{
    public static Action<object> CreateAction<T>(MethodInfo methodInfo)
    {
        return CreateAction(typeof(T), methodInfo);
    }

    public static Action<object> CreateAction(Type type, MethodInfo methodInfo)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (methodInfo is null)
        {
            throw new ArgumentNullException(nameof(methodInfo));
        }

        var input = Expression.Parameter(typeof(object), "input");
        return Expression.Lambda<Action<object>>(
            Expression.Call(
                Expression.Convert(
                    input,
                    type),
                methodInfo),
            input).Compile();
    }
}
