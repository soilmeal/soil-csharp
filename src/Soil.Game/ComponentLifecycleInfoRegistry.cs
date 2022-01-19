using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Soil.Types;

namespace Soil.Game;

public class ComponentLifecycleInfoRegistry
{
    private readonly Dictionary<string, ComponentLifecycleInfo> _infos;

    private ComponentLifecycleInfoRegistry(Dictionary<string, ComponentLifecycleInfo> infos)
    {
        _infos = infos;
    }

    public bool TryGetValue(Type type, out ComponentLifecycleInfo? info)
    {
        return _infos.TryGetValue(type.FullName, out info);
    }

    public class Builder
    {
        private Dictionary<string, ComponentLifecycleInfo> _infos = new();

        private static MethodInfo? FindLifecycleMethod(
            Type type,
            ComponentLifecycleType lifecycleType)
        {
            MethodInfo? methodInfo = type.GetMethods(BindingFlags.Instance)
                .Where(method =>
                {
                    ComponentLifecycleAttribute? attr = method.GetCustomAttribute<ComponentLifecycleAttribute>();
                    return attr != null && attr.Value == lifecycleType;
                })
                .Where(method => method.ReturnType == typeof(void))
                .Where(method => method.GetParameters().Length <= 0)
                .FirstOrDefault();
            if (methodInfo is not null)
            {
                return methodInfo;
            }

            methodInfo = type.GetMethod(lifecycleType.FastToString(), BindingFlags.Instance);
            if (methodInfo.ReturnType != typeof(void))
            {
                return null;
            }

            if (methodInfo.GetParameters().Length > 0)
            {
                return null;
            }

            return methodInfo;
        }

        public Builder AddAll(Assembly assembly)
        {
            Type[] componentTypes = assembly.GetTypes()
                .Where(t => t.BaseType == typeof(Component))
                .ToArray();
            foreach (var componentType in componentTypes)
            {
                if (componentType == null)
                {
                    continue;
                }

                var actions = new Dictionary<ComponentLifecycleType, Action<object>>();
                foreach (var type in Enum.GetValues(typeof(ComponentLifecycleType)))
                {
                    ComponentLifecycleType lifecycleType = (ComponentLifecycleType)type;
                    MethodInfo? methodInfo = FindLifecycleMethod(componentType, lifecycleType);
                    if (methodInfo == null)
                    {
                        continue;
                    }

                    actions.TryAdd(
                        lifecycleType,
                        MethodInfoHelper.CreateAction(
                            componentType,
                            methodInfo));
                }

                _infos.TryAdd(componentType.FullName, new ComponentLifecycleInfo(actions));
            }

            return this;
        }

        public ComponentLifecycleInfoRegistry Build()
        {
            return new ComponentLifecycleInfoRegistry(_infos);
        }
    }
}
