using System;
using Mirror;
using QFramework;
using QFramework.Example;
using UnityEngine;

namespace HideAndSeek
{
    public class CatEntity : AnimalEntity
    {
        public GameObject scanSkill;
        public Transform throwPoint;
        public GameObject throwSkill;
        public float throwForce = 50f;
        ThirdPerSonController m_thirdPerSonController; 

        private void Start()
        {
            scanSkill.Hide();
            m_thirdPerSonController = GetComponent<ThirdPerSonController>();
        }

        public override void Init()
        {
            base.Init();
            if (isLocalPlayer && m_player.m_gameType == GameType.Cat)
            {
                UIKit.OpenPanel<CatPanel>(new CatPanelData()
                {
                    catEntity = this
                });
            }

            Cmd_CatStartAction();
        }

        [Command]
        public void Cmd_CatStartAction()
        {
            Rpc_CatStartAction();
        }
        [ClientRpc]
        public void Rpc_CatStartAction()
        {
            if (isLocalPlayer && m_player.m_gameType == GameType.Cat)
            {
                m_thirdPerSonController.enabled = false;
                UIKit.GetPanel<CatPanel>().LockAllSkill();
            }
            
            UIKit.OpenPanel<TimePanel>(new TimePanelData()
            {
                startTime = true,
                timer = new CountdownTimer(30f, () =>
                {
                    UIKit.ClosePanel<TimePanel>();
                    if (isLocalPlayer && m_player.m_gameType == GameType.Cat)
                    {
                        m_thirdPerSonController.enabled = true;
                        UIKit.GetPanel<CatPanel>().UnLockAllSkill();
                    }
                    
                    ActionKit.Delay(1f, () =>
                    {
                        UIKit.OpenPanel<TimePanel>(new TimePanelData()
                        {
                            startTime = false,
                            timer = new CountdownTimer(600f, () =>
                            {
                                UIKit.ClosePanel<TimePanel>();
                                //游戏结束
                                UIKit.OpenPanel<GameOverPanel>(new GameOverPanelData()
                                {
                                    mGameType = m_player.m_gameType,
                                    isWin = m_player.m_gameType == GameType.Mouse
                                });
                            })
                        });
                    }).Start(this);
                })
            });
        }

        public void ScanSkill()
        {
            Cmd_ScanSkill(m_netId);
        }
        [Command]
        protected void Cmd_ScanSkill(uint id)
        {
            // 查找扫描范围内的老鼠
            bool mouseFound = false;
            var cat = NetworkServer.spawned[id].GetComponent<CatEntity>();
            Collider[] hits = Physics.OverlapSphere(cat.transform.position, 10f); // 设置扫描半径为10

            foreach (var hit in hits)
            {
                var mouse = hit.GetComponent<MouseEntity>(); // 假设有MouseEntity类
                if (mouse != null)
                {
                    mouseFound = true;
                    break;
                }
            }

            Rpc_ScanSkill(id, mouseFound);
        }

        [ClientRpc]
        protected void Rpc_ScanSkill(uint id, bool mouseFound)
        {
            scanSkill.Show();
            
            if (isLocalPlayer && m_player.m_gameType == GameType.Cat)
            {
                var panel = UIKit.GetPanel<CatPanel>();
                panel?.ShowScanTip(mouseFound); // 安全调用

                ActionKit.Delay(10f, () =>
                {
                    scanSkill.Hide();
                    scanSkill.LocalIdentity();
                }).Start(this);
            }
        }

        public void AccelerateSkill()
        {
            // 本地立即生效（客户端预测）
            m_thirdPerSonController.Accelerate();
    
            // 同步到服务端和其他客户端
            Cmd_AccelerateSkill(true); // true表示加速，false表示减速

            ActionKit.Delay(10f, () =>
            {
                m_thirdPerSonController.Decelerate();
                Cmd_AccelerateSkill(false);
            }).Start(this);
        }

        [Command]
        private void Cmd_AccelerateSkill(bool isAccelerating)
        {
            Rpc_AccelerateSkill(isAccelerating);
        }

        [ClientRpc]
        private void Rpc_AccelerateSkill(bool isAccelerating)
        {
            if (isAccelerating)
                m_thirdPerSonController.Accelerate();
            else
                m_thirdPerSonController.Decelerate();
        }
        public void ThrowSkill()
        {
            Cmd_ThrowSkill(m_netId);
        }
        [Command]
        protected void Cmd_ThrowSkill(uint id)
        {
            Rpc_ThrowSkill(id);
        }
        [ClientRpc]
        protected void Rpc_ThrowSkill(uint id)
        {
            var throwSkillclone =throwSkill.Instantiate()
                                             .Show()
                                             .LocalPosition(throwPoint.position);
            throwSkillclone.transform.SetParent(null);
            // Vector3 cameraForward = transform.GetComponentInChildren<Camera>().Position();
            // cameraForward.y = 0f; 
            // cameraForward.Normalize();
            throwSkillclone.GetComponent<Rigidbody>().AddForce(transform.forward*throwForce);
            ActionKit.Delay(10f, () =>
            {
                throwSkillclone.DestroySelf();
            }).Start(this);
        }
    }
}