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
        return _infos.TryGetValue(type.FullName!, out info);
    }

    public class Builder
    {
        private readonly Dictionary<string, ComponentLifecycleInfo> _infos = new();

        private static MethodInfo? FindLifecycleMethod(Type type, ComponentLifecycleStep step)
        {
            MethodInfo? methodInfo = type.GetMethods(BindingFlags.Instance)
                .Where(method =>
                {
                    ComponentLifecycleAttribute? attr = method.GetCustomAttribute<ComponentLifecycleAttribute>();
                    return attr != null && attr.Step == step;
                })
                .Where(method => method.ReturnType == typeof(void))
                .Where(method => method.GetParameters().Length <= 0)
                .FirstOrDefault();
            if (methodInfo is not null)
            {
                return methodInfo;
            }

            methodInfo = type.GetMethod(step.FastToString(), BindingFlags.Instance);
            if (methodInfo == null)
            {
                return null;
            }

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

        public Builder AddRange(Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length <= 0)
            {
                return this;
            }

            foreach (var assembly in assemblies)
            {
                AddAll(assembly);
            }
            return this;
        }

        public Builder AddAll(Assembly assembly)
        {
            Type[] types = assembly.GetTypes()
                .Where(t => t.BaseType == typeof(Component))
                .ToArray();
            foreach (var type in types)
            {
                if (type == null)
                {
                    continue;
                }

                var actions = new Dictionary<ComponentLifecycleStep, Action<object>>();
                foreach (var step in ComponentLifecycleStepExtensions.FastGetValues())
                {
                    MethodInfo? methodInfo = FindLifecycleMethod(type, step);
                    if (methodInfo == null)
                    {
                        continue;
                    }

                    actions.TryAdd(step, MethodInfoHelper.CreateAction(type, methodInfo));
                }

                _infos.TryAdd(type.FullName!, new ComponentLifecycleInfo(actions));
            }

            return this;
        }

        public ComponentLifecycleInfoRegistry Build()
        {
            return new ComponentLifecycleInfoRegistry(_infos);
        }
    }
}
