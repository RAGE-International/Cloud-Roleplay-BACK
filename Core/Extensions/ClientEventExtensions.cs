using GTANetworkMethods;
using System;
using System.Reflection;

public static class ClientEventExtensions
{
    public static void Register(this ClientEvent clientEvent, string eventName, object classInstance, Action action)
        => clientEvent.Register(eventName, classInstance, action.Method);

    public static void Register<T>(this ClientEvent clientEvent, string eventName, object classInstance, Action<T> action)
        => clientEvent.Register(eventName, classInstance, action.Method);

    public static void Register<T1, T2>(this ClientEvent clientEvent, string eventName, object classInstance, Action<T1, T2> action)
        => clientEvent.Register(eventName, classInstance, action.Method);

    public static void Register<T1, T2, T3>(this ClientEvent clientEvent, string eventName, object classInstance, Action<T1, T2, T3> action)
        => clientEvent.Register(eventName, classInstance, action.Method);

    public static void Register<T1, T2, T3, T4>(this ClientEvent clientEvent, string eventName, object classInstance, Action<T1, T2, T3, T4> action)
        => clientEvent.Register(eventName, classInstance, action.Method);

    public static void Register<T1, T2, T3, T4, T5>(this ClientEvent clientEvent, string eventName, object classInstance, Action<T1, T2, T3, T4, T5> action)
        => clientEvent.Register(eventName, classInstance, action.Method);

    public static void Register<T1, T2, T3, T4, T5, T6>(this ClientEvent clientEvent, string eventName, object classInstance, Action<T1, T2, T3, T4, T5, T6> action)
        => clientEvent.Register(eventName, classInstance, action.Method);
    public static void Register<T1, T2, T3, T4, T5,T6,T7,T8,T9,T10,T11>(this ClientEvent clientEvent, string eventName, object classInstance, Action<T1, T2, T3, T4, T5,T6, T7, T8, T9, T10, T11> action)
        => clientEvent.Register(eventName, classInstance, action.Method);

    public static void Register(this ClientEvent clientEvent, string eventName, object classInstance, Delegate del)
        => clientEvent.Register(eventName, classInstance, del.Method);

    public static void Register(this ClientEvent clientEvent, string eventName, object classInstance, MethodInfo methodInfo)
    {
        clientEvent.Register(methodInfo, eventName, classInstance);
    }
}