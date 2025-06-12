using System.Collections;
using System.Collections.Generic;
using HideAndSeek;
using Mirror;
using QFramework;
using UnityEngine;

public class PlayerManager : NetworkBehaviour,IController
{
    private ResLoader resLoader;
    private Transform startPos;

    private void Awake()
    {
        // if(!isLocalPlayer) return;
        resLoader = ResLoader.Allocate();
        this.GetSystem<GameEventSystem>().OnEnterGame.Register(() =>
        {
            startPos = GameObject.Find("StartPos").transform;
            IPlayerModel playerModel = this.GetModel<IPlayerModel>();
            Cmd_InitPlayer(playerModel.player.m_gameType);
        }).UnRegisterWhenGameObjectDestroyed(this);
    }
    [Command(requiresAuthority = false)]
    public void Cmd_InitPlayer(GameType gameType)
    {
        GameObject prefab = null;
        switch (gameType)
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
            NetworkServer.Spawn(playerObj, connectionToClient);
        }
        else
        {
            print("未找到");
        }
    }
    // [ClientRpc]
    // public void Rpc_InitPlayer(GameType gameType)
    // {
    //     
    // }

    public Vector3 GetRandomPostion()
    {
        var pos = startPos.position;
        pos.x += Random.Range(0, 1.5f);
        return pos;
    }

    private void OnDestroy()
    {
        resLoader.Recycle2Cache();
        resLoader = null;
    }

    public IArchitecture GetArchitecture()
    {
        return HideAndSeek.HideAndSeek.Interface;
    }
}
