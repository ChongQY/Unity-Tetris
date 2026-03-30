using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter
{
    private static Dictionary<EventType , Delegate> m_EventTable  = new Dictionary<EventType, Delegate> ();

    // 添加监听前的验证检查
    private static void OnListenerAdding(EventType eventType , Delegate callBack) {
        // 如果不存在事件码 添加一个空的
        if (!m_EventTable.ContainsKey(eventType)) {
            m_EventTable.Add(eventType, null);
        }

        Delegate d = m_EventTable[eventType];
        if (d != null && d.GetType() != callBack.GetType()) {
            // 类型不匹配 抛出一个异常
            throw new Exception(
                    string.Format("尝试为事件{0}添加不同类型的委托, 当前事件所对应的委托是{1}, 要添加的委托类型是{2}",
                    eventType,
                    d.GetType(),
                    callBack.GetType())
                );
        }

    }

    // 移除监听前的验证检查
    private static void OnListenerRemoving(EventType eventType, Delegate callBack) {
        if (m_EventTable.ContainsKey(eventType)) {
            Delegate d = m_EventTable[eventType];
            if (d == null) {
                throw new Exception(
                    string.Format("移除监听错误: 事件{0} 没有对应的委托",
                    eventType)
                );
            } else if (d.GetType() != callBack.GetType()) {
                throw new Exception(
                    string.Format("移除监听错误: 尝试为事件{0}移除不同类型的委托, 当前委托类型为{1}, 要移除的委托类型为{2}",
                    eventType,
                    d.GetType(),
                    callBack.GetType())
                );
            }
        } else {
            throw new Exception(
                string.Format("移除监听错误: 没有事件码{0}",
                eventType)
            );
        }
    }

    // 移除监听后的验证检查
    private static void OnListenerRemoved(EventType eventType) {
        if (m_EventTable[eventType] == null) {
            m_EventTable.Remove(eventType);
        }
    }
    // ======================== 添加监听 ========================
    // 无参 添加监听
    public static void AddListener(EventType eventType, CallBack callBack) {
        // 验证检查
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack)m_EventTable[eventType] + callBack;
    }

    // 一个参数 添加监听
    public static void AddListener<T>(EventType eventType, CallBack<T> callBack) {
        // 验证检查
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] + callBack;
    }

    // 二个参数 添加监听
    public static void AddListener<T, X>(EventType eventType, CallBack<T, X> callBack) {
        // 验证检查
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X>)m_EventTable[eventType] + callBack;
    }

    // 三个参数 添加监听
    public static void AddListener<T, X , Y>(EventType eventType, CallBack<T, X, Y> callBack) {
        // 验证检查
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y>)m_EventTable[eventType] + callBack;
    }

    // 四个参数 添加监听
    public static void AddListener<T, X, Y , Z>(EventType eventType, CallBack<T, X, Y, Z> callBack) {
        // 验证检查
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z>)m_EventTable[eventType] + callBack;
    }

    // 五个参数 添加监听
    public static void AddListener<T, X, Y, Z , W>(EventType eventType, CallBack<T, X, Y, Z, W> callBack) {
        // 验证检查
        OnListenerAdding(eventType, callBack);
        m_EventTable[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[eventType] + callBack;
    }
    // ======================== 移除监听 ========================
    // 无参 移除监听
    public static void RemoveListener(EventType eventType, CallBack callBack) {
        // 验证检测
        OnListenerRemoving(eventType, callBack);

        //移除
        m_EventTable[eventType] = (CallBack)m_EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }

    // 一个参数 移除监听
    public static void RemoveListener<T>(EventType eventType, CallBack<T> callBack) {
        // 验证检测
        OnListenerRemoving(eventType, callBack);

        //移除
        m_EventTable[eventType] = (CallBack<T>)m_EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }

    // 二个参数 移除监听
    public static void RemoveListener<T , X>(EventType eventType, CallBack<T, X> callBack) {
        // 验证检测
        OnListenerRemoving(eventType, callBack);

        //移除
        m_EventTable[eventType] = (CallBack<T, X>)m_EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }

    // 三个参数 移除监听
    public static void RemoveListener<T, X , Y>(EventType eventType, CallBack<T, X, Y> callBack) {
        // 验证检测
        OnListenerRemoving(eventType, callBack);

        //移除
        m_EventTable[eventType] = (CallBack<T, X, Y>)m_EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }

    // 四个参数 移除监听
    public static void RemoveListener<T, X, Y , Z>(EventType eventType, CallBack<T, X, Y, Z> callBack) {
        // 验证检测
        OnListenerRemoving(eventType, callBack);

        //移除
        m_EventTable[eventType] = (CallBack<T, X, Y, Z>)m_EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }

    // 五个参数 移除监听
    public static void RemoveListener<T, X, Y, Z , W>(EventType eventType, CallBack<T, X, Y, Z, W> callBack) {
        // 验证检测
        OnListenerRemoving(eventType, callBack);

        //移除
        m_EventTable[eventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[eventType] - callBack;
        OnListenerRemoved(eventType);
    }
    // ======================== 广播监听 ========================
    // 无参 广播监听
    public static void Broadcast(EventType eventType) {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d)) {// 一次查找同时完成检查和取值
            CallBack callBack = d as CallBack;
            if(callBack != null) {
                callBack();
            } else {
                throw new Exception(
                    string.Format("广播事件错误: 事件{0}对应的委托有不同的类型",
                    eventType)
                );
            }
        }
    }

    // 一个参数 广播监听
    public static void Broadcast<T>(EventType eventType , T arg) {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d)) {// 一次查找同时完成检查和取值
            CallBack<T> callBack = d as CallBack<T>;
            if (callBack != null) {
                callBack(arg);
            } else {
                throw new Exception(
                    string.Format("广播事件错误: 事件{0}对应的委托有不同的类型",
                    eventType)
                );
            }
        }
    }

    // 二个参数 广播监听
    public static void Broadcast<T , X>(EventType eventType, T arg1 , X arg2) {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d)) {// 一次查找同时完成检查和取值
            CallBack<T, X> callBack = d as CallBack<T, X>;
            if (callBack != null) {
                callBack(arg1 , arg2);
            } else {
                throw new Exception(
                    string.Format("广播事件错误: 事件{0}对应的委托有不同的类型",
                    eventType)
                );
            }
        }
    }

    // 三个参数 广播监听
    public static void Broadcast<T , X , Y>(EventType eventType, T arg1 , X arg2 , Y arg3) {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d)) {// 一次查找同时完成检查和取值
            CallBack<T, X, Y> callBack = d as CallBack<T, X, Y>;
            if (callBack != null) {
                callBack(arg1 , arg2 , arg3);
            } else {
                throw new Exception(
                    string.Format("广播事件错误: 事件{0}对应的委托有不同的类型",
                    eventType)
                );
            }
        }
    }

    // 四个参数 广播监听
    public static void Broadcast<T, X , Y , Z>(EventType eventType, T arg1 , X arg2 , Y arg3 , Z arg4) {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d)) {// 一次查找同时完成检查和取值
            CallBack<T, X, Y, Z> callBack = d as CallBack<T, X, Y, Z>;
            if (callBack != null) {
                callBack(arg1 , arg2, arg3 , arg4);
            } else {
                throw new Exception(
                    string.Format("广播事件错误: 事件{0}对应的委托有不同的类型",
                    eventType)
                );
            }
        }
    }

    // 五个参数 广播监听
    public static void Broadcast<T, X, Y, Z , W>(EventType eventType, T arg1, X arg2, Y arg3, Z arg4 , W arg5) {
        Delegate d;
        if (m_EventTable.TryGetValue(eventType, out d)) {// 一次查找同时完成检查和取值
            CallBack<T, X, Y, Z, W> callBack = d as CallBack<T, X, Y, Z, W>;
            if (callBack != null) {
                callBack(arg1 , arg2 , arg3 , arg4 , arg5);
            } else {
                throw new Exception(
                    string.Format("广播事件错误: 事件{0}对应的委托有不同的类型",
                    eventType)
                );
            }
        }
    }
}
