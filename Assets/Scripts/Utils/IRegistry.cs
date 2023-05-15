using System;
using System.Collections.Generic;

public interface IRegistry
{
    Dictionary<Type, List<object>> _registry { get; set; }
}

public static class IRegistryHelper
{    
    /// <summary>
     /// Register an object of type T.
     /// </summary>
     /// <typeparam name="T">Type of object to register.</typeparam>
     /// <param name="instance">The value of the object to register.</param>
    public static void Register<T>(this IRegistry registry, T instance) where T : class
    {
        if (!registry._registry.ContainsKey(typeof(T))) registry._registry.Add(typeof(T), new List<object>() { instance });
        if (registry._registry[typeof(T)].Contains(instance)) return;
        registry._registry[typeof(T)].Add(instance);
    }

    /// <summary>
    /// Deregisters the given object from the registry.
    /// </summary>
    /// <typeparam name="T">Type of object to deregister.</typeparam>
    /// <param name="instance">The value of the object to deregister.</param>
    public static void Deregister<T>(this IRegistry registry, T instance) where T : class
    {
        if (!registry._registry.ContainsKey(typeof(T))) return;
        if (!registry._registry[typeof(T)].Contains(instance)) return;
        registry._registry[typeof(T)].Remove(instance);
    }

    /// <summary>
    /// Deregister any object of type T
    /// </summary>
    /// <typeparam name="T">Type to deregister</typeparam>
    public static void Deregister<T>(this IRegistry registry) where T : class
    {
        registry._registry.Remove(typeof(T));
    }

    /// <summary>
    /// Loop through all the objects in the registry of type T
    /// </summary>
    /// <typeparam name="T">Type of register through</typeparam>
    /// <param name="callback">The callback action of type T that runs for every single object in the register</param>
    public static void Loop<T>(this IRegistry registry, Action<T> callback) where T : class
    {
        Type t = typeof(T);
        if (!registry._registry.ContainsKey(t)) return;
        for (int i = 0; i < registry._registry[t].Count; i++)
            callback?.Invoke(registry._registry[t][i] as T);
    }
}
