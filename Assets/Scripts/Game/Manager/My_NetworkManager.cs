using System;
using System.Collections;
using System.Collections.Generic;
using HideAndSeek;
using Mirror;
using QFramework;
using QFramework.Example;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class My_NetworkManager : NetworkManager
{
    UnityEvent onStartServerEvent;
    UnityEvent onclientConnectEvent;
    private readonly Dictionary<string, UnityAction> serverDir = new();
    private readonly Dictionary<string, UnityAction> clientDir = new();
    public static My_NetworkManager Instance;
    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public bool RegisterOnStartServerEvent(UnityAction action)
    {
        if (onStartServerEvent == null)
        {
            onStartServerEvent = new UnityEvent();
        }

        string key = action.Target.GetType().FullName + action.GetMethodName();
        if (!serverDir.ContainsKey(key))
        {
            onStartServerEvent.AddListener(action);
            serverDir.Add(key, action);
            if (NetworkServer.active) action?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool RegisterOnclientConnectEvent(UnityAction action)
    { 
        if (onclientConnectEvent == null)
        {
            onclientConnectEvent = new UnityEvent();
        }
        string key = action.Target.GetType().FullName + action.GetMethodName();
        if (!clientDir.ContainsKey(key))
        {
            onclientConnectEvent.AddListener(action);
            clientDir.Add(key, action);
            if (NetworkClient.isConnected)
            {
                action?.Invoke();
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public void UnRegisterOnStartServerEvent(UnityAction action)
    {
        string key = action.Target.GetType().FullName + action.GetMethodName();
        if (onStartServerEvent == null || !serverDir.ContainsKey(key)) return;
        onStartServerEvent.RemoveListener(action);
        serverDir.Remove(key);
    }
    public void UnRegisterOnStartClientEvent(UnityAction action)
    {
        string key = action.Target.GetType().FullName + action.GetMethodName();
        if (onclientConnectEvent == null || !clientDir.ContainsKey(key)) return;
        onclientConnectEvent.RemoveListener(action);
        clientDir.Remove(key);
    }
    /// <summary>
    /// 客户端连接时服务端执行此方法
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
    }
    /// <summary>
    /// 当 ​客户端成功连接到服务器​ 时，服务器端触发此方法。
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
    }
    /// <summary>
    /// 当 ​客户端成功连接到服务器​ 时，客户端触发此方法。
    /// </summary>
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        onclientConnectEvent?.Invoke();
    }
    /// <summary>
    /// 启动服务端后在服务器执行的代码(初始化和注册消息)
    /// </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        onStartServerEvent?.Invoke();
    }
    /// <summary>
    /// 启动客户端在客户端执行的代码
    /// </summary> 
    public override void OnStartClient()
    {
        base.OnStartClient();
        
    }
    /// <summary>
    /// 停止服务端执行的方法
    /// </summary>
    public override void OnStopServer()
    {
        base.OnStopServer();
    }
    /// <summary>
    /// 停止客户端执行的方法
    /// </summary>
    public override void OnStopClient()
    {
        base.OnStopClient();
    }
    /// <summary>
    /// 当客户端断开时，服务器会调用此方法
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        if (!NetworkClient.isConnected)
        {
            Debug.LogWarning("连接失败（尚未建立连接即断开）");
            
            UIKit.OpenPanel<TipPanel>(new TipPanelData()
            {
                tip = "Fail to connect.",
                callback = () =>
                {
                    UIKit.ClosePanel<LodinPanel>();
                }
            });
        }
        else
        {
            OnClientError();
            Debug.Log("已连接的客户端断开连接");
        }
    }
    public void OnClientError()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == "Game" && scene.isLoaded)
            {
                SceneManager.UnloadSceneAsync(scene);
                break;
            }
        }
        UIKit.CloseAllPanel();
        UIKit.OpenPanel<StartPanel>();
    }
}


