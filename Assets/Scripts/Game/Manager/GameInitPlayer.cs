using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using HideAndSeek;
using QFramework;
using QFramework.Example;
using Random = UnityEngine.Random;

public class GameInitPlayer : MonoBehaviour,IController
{
    private ResLoader resLoader;
    public Transform startPos;
    private My_NetworkManager manager => My_NetworkManager.Instance;
    private void Start()
    {
        resLoader = ResLoader.Allocate();
        manager.RegisterOnStartServerEvent(OnStartServer);
        manager.RegisterOnclientConnectEvent(OnStartClient);
    }
    [Server]
    private void OnStartServer()
    {
        NetworkServer.RegisterHandler<InitPlayerRequestMessage>(InitPlayerRequestMessageHandler);
        NetworkServer.RegisterHandler<OnAllSceneLoadRequestMessage>(OnAllSceneLoadRequestMessageHandler);
    }

    private int playerCount;
    [Server]
    private void OnAllSceneLoadRequestMessageHandler(NetworkConnectionToClient arg1, OnAllSceneLoadRequestMessage arg2)
    {
        playerCount++;
        if (playerCount>=2)
        {
            this.GetModel<IPlayerConnectionModel>().Players.ForEach(x =>
            {
                x.Value.m_connection.Send<OnAllSceneLoadResponseMessage>(new OnAllSceneLoadResponseMessage());
            });
        }
    }

    [Server]
    private void InitPlayerRequestMessageHandler(NetworkConnectionToClient arg1, InitPlayerRequestMessage arg2)
    {
        // arg1.Send<InitPlayerResponseMessage>(new InitPlayerResponseMessage());
        IPlayerModel playerModel = this.GetModel<IPlayerModel>();
        GameObject prefab = null;
        switch (arg2.gameType)
        {
            case GameType.Cat:
                prefab = My_NetworkManager.Instance.spawnPrefabs.Find(x => x.name == "catPlayer");
                break;
            case GameType.Mouse:
                prefab = My_NetworkManager.Instance.spawnPrefabs.Find(x => x.name == "mousePlayer");
                break;
        }
        if (prefab != null)
        {
            Vector3 spawnPos = GetRandomPostion();
            GameObject playerObj = Instantiate(prefab, spawnPos, Quaternion.identity);
            // NetworkServer.Spawn(playerObj, arg1);
            NetworkServer.AddPlayerForConnection(arg1, playerObj);
        }
        else
        {
            print("未找到");
        }
    }
    public Vector3 GetRandomPostion()
    {
        var pos = startPos.position;
        pos.x += Random.Range(0, 2.5f);
        return pos;
    }
    [Client]
    private void OnStartClient()
    {
        NetworkClient.RegisterHandler<InitPlayerResponseMessage>(InitPlayerResponseMessageHandler);
        NetworkClient.RegisterHandler<OnAllSceneLoadResponseMessage>(OnAllSceneLoadResponseMessageHandler);
    }
    [Client]
    private void OnAllSceneLoadResponseMessageHandler(OnAllSceneLoadResponseMessage obj)
    {
        UIKit.ClosePanel<LodinPanel>();
        var playerModel = this.GetModel<IPlayerModel>();
        if (playerModel!=null)
        {
            NetworkClient.Send<InitPlayerRequestMessage>(new InitPlayerRequestMessage()
            {
                gameType = playerModel.player.m_gameType
            });
        }
    }

    [Client]
    private void InitPlayerResponseMessageHandler(InitPlayerResponseMessage obj)
    {
        
    }

    private void OnDestroy()
    {
        resLoader.Recycle2Cache();
        resLoader = null;
        manager.UnRegisterOnStartServerEvent(OnStartServer);
        manager.UnRegisterOnStartClientEvent(OnStartClient);
    }

    public IArchitecture GetArchitecture()
    {
        return HideAndSeek.HideAndSeek.Interface;
    }
}
