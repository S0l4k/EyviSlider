using System;
using System.Collections.Generic;
using UnityEngine;


public class Dependencies : MonoBehaviour
{
    public static Dependencies Instance;

    private void Awake()
    {
        Instance = this;
    }

    private Dictionary<Type, object> dependencies = new();


    // aby u¿yæ, w innym skrypcie w awake; Dependencies.Instance.RegisterDependency<>();
    public void RegisterDependency<T>(T instance)
    {
        if (dependencies.ContainsKey(typeof(T)))
        {
            return;
        }
        dependencies.Add(typeof(T), instance);
    }

    // aby u¿yæ, w innym skrypcie Dependency.Instance.GetDependancy<>()
    public T GetDependancy<T>()
    {
        if (dependencies.TryGetValue(typeof(T), out var value))
        {
            return (T)value;
        }

        return default;
    }

    public void UnregisterDependency<T>()
    {
        if (dependencies.ContainsKey(typeof(T)))
        {
            dependencies.Remove(typeof(T));
        }
    }
}