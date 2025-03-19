using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 事件中心类
public class EventCenter : MonoBehaviour
{
    // 单例模式
    private static EventCenter instance;
    public static EventCenter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("EventCenter").AddComponent<EventCenter>();
            }
            return instance;
        }
    }

    // 在Awake中实例化单例
    private void Awake()
    {
        instance = this;
    }

    // 事件表：支持多种类型的委托 (Action、Action<T>、IEnumerator)
    private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

    // 订阅无参数事件
    public void Subscribe(string eventType, Action listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = Delegate.Combine(eventTable[eventType], listener);
        }
        else
        {
            eventTable[eventType] = listener;
        }
    }

    // 订阅带参数事件
    public void Subscribe<T>(string eventType, Action<T> listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = Delegate.Combine(eventTable[eventType], listener);
        }
        else
        {
            eventTable[eventType] = listener;
        }
    }

    // 订阅IEnumerator类型事件（用于协程）
    public void Subscribe(string eventType, Action<IEnumerator> listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = Delegate.Combine(eventTable[eventType], listener);
        }
        else
        {
            eventTable[eventType] = listener;
        }
    }

    // 触发无参数事件
    public void TriggerEvent(string eventType)
    {
        if (eventTable.ContainsKey(eventType))
        {
            var eventAction = eventTable[eventType] as Action;
            eventAction?.Invoke();
        }
    }

    // 触发带参数事件
    public void TriggerEvent<T>(string eventType, T arg)
    {
        if (eventTable.ContainsKey(eventType))
        {
            var eventAction = eventTable[eventType] as Action<T>;
            eventAction?.Invoke(arg);
        }
    }

    // 触发IEnumerator类型事件（用于协程）
    public void TriggerEvent(string eventType, IEnumerator coroutine)
    {
        if (eventTable.ContainsKey(eventType))
        {
            var eventAction = eventTable[eventType] as Action<IEnumerator>;
            eventAction?.Invoke(coroutine);
        }
    }

    // 取消订阅无参数事件
    public void Unsubscribe(string eventType, Action listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = Delegate.Remove(eventTable[eventType], listener);
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }

    // 取消订阅带参数事件
    public void Unsubscribe<T>(string eventType, Action<T> listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = Delegate.Remove(eventTable[eventType], listener);
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }

    // 取消订阅IEnumerator类型事件
    public void Unsubscribe(string eventType, Action<IEnumerator> listener)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = Delegate.Remove(eventTable[eventType], listener);
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }
}
