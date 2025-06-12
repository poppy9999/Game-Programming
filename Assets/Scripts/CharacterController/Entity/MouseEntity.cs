using System;
using System.Collections.Generic;
using Mirror;
using QFramework;
using QFramework.Example;
using Unity.VisualScripting;
using UnityEngine;

namespace HideAndSeek
{
    public enum MouseVary
    {
        Normal,
        Street_Lamp,
        Hydrant,
        Car
    }
    public class MouseEntity : AnimalEntity
    {
        public List<MouseVaryClass> mouseVarys = new();
        private bool isVary;

        private void Start()
        {
            
        }
        public override void Init()
        {
            base.Init();
            if (isLocalPlayer && m_player.m_gameType == GameType.Mouse)
            {
                UIKit.OpenPanel<MousePanel>(new MousePanelData()
                {
                    mouseEntity = this
                });
            }
        }

        public void SetMouseVary(MouseVary mouseVary)
        {
            CmdSetMouseVary(mouseVary,m_netId);
        }
        [Command(requiresAuthority = false)]
        protected void CmdSetMouseVary(MouseVary mouseVary,uint netId)
        {
            Rpc_SetMouseVary(mouseVary,netId);
        }
        [ClientRpc]
        protected void Rpc_SetMouseVary(MouseVary mouseVary,uint netId)
        {
            if (netId == m_netId)
            {
                HideAllVary();
                GetVary(mouseVary).Show();
                if (mouseVary!=MouseVary.Normal)
                {
                    lateVary = mouseVary;
                    ActionKit.Delay(30f, () =>
                    {
                        HideAllVary();
                        GetVary(MouseVary.Normal).Show();
                        isVary = false;
                    }).Start(this);
                    isVary = true;
                }
            }
            else
            {
                var obj = FindPlayerByNetId(netId);
                if (obj)
                {
                    var mouse = obj.GetComponent<MouseEntity>();
                    mouse.HideAllVary();
                    mouse.GetVary(mouseVary).Show();
                    if (mouseVary!=MouseVary.Normal)
                    {
                        lateVary = mouseVary;
                        ActionKit.Delay(30f, () =>
                        {
                            mouse.HideAllVary();
                            mouse.GetVary(MouseVary.Normal).Show();
                            isVary = false;
                        }).Start(this);
                        isVary = true;
                    }
                }
            }
        }

        protected GameObject GetVary(MouseVary mouseVary)
        {
            GameObject obj = null;
            obj = mouseVarys.Find(x => x.mouseVary == mouseVary).mouseVaryGameObject;
            return obj;
        }

        protected void HideAllVary()
        {
            mouseVarys.ForEach(x =>
            {
                x.mouseVaryGameObject.Hide();
            });
        }


        private MouseVary lateVary;
        public void Scan()
        {
            if (!isVary) return;
            Cmd_Scan(m_netId);
        }
        [Command]
        public void Cmd_Scan(uint id)
        {
            Rpc_Scan(m_netId);
        }
        [ClientRpc]
        public void Rpc_Scan(uint id)
        {
            if (netId == m_netId)
            {
                HideAllVary();
                GetVary(MouseVary.Normal).Show();
            }
            else
            {
                var obj = FindPlayerByNetId(netId);
                if (obj)
                {
                    var mouse = obj.GetComponent<MouseEntity>();
                    mouse.HideAllVary();
                    mouse.GetVary(MouseVary.Normal).Show();
                }
            }
        }
        public void UnScan()
        {
            if (!isVary) return;
            Cmd_UnScan(m_netId);
        }
        [Command]
        public void Cmd_UnScan(uint id)
        {
            Rpc_UnScan(m_netId);
        }
        [ClientRpc]
        public void Rpc_UnScan(uint id)
        {
            if (netId == m_netId)
            {
                HideAllVary();
                GetVary(lateVary).Show();
            }
            else
            {
                var obj = FindPlayerByNetId(netId);
                if (obj)
                {
                    var mouse = obj.GetComponent<MouseEntity>();
                    mouse.HideAllVary();
                    mouse.GetVary(lateVary).Show();
                }
            }
        }

        public void MouseLose()
        {
            Cmd_MouseLose(m_netId);
        }
        [Command]
        public void Cmd_MouseLose(uint id)
        {
            Rpc_MouseLose(id);
        }
        [ClientRpc]
        public void Rpc_MouseLose(uint id)
        {
            if (isLocalPlayer && m_player.m_gameType == GameType.Mouse)
            {
                UIKit.OpenPanel<GameOverPanel>(new GameOverPanelData()
                {
                    mGameType = GameType.Mouse,
                    isWin = false
                });
            }
            else
            {
                UIKit.OpenPanel<GameOverPanel>(new GameOverPanelData()
                {
                    mGameType = GameType.Cat,
                    isWin = true
                });
            }
        }
    }
    [Serializable]
    public class MouseVaryClass
    {
        public MouseVary mouseVary;
        public GameObject mouseVaryGameObject;
    }
}