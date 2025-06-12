using Mirror;
using QFramework;
using QFramework.Example;
using UnityEngine; 
using UnityEngine.UI; 

namespace HideAndSeek
{
    public class StartPanelData : UIPanelData
    {
    }

    public partial class StartPanel : UIPanel, IController
    {
        private My_NetworkManager manager => My_NetworkManager.Instance;

        public GameObject netTipPanel; 
        public Button closeTipBtn;  
        public static string LastUsedIP = "";   

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as StartPanelData ?? new StartPanelData();

            manager.RegisterOnStartServerEvent(OnStartServer);
            manager.RegisterOnclientConnectEvent(OnStartClient);

            serverBtn.onClick.AddListener(() =>
            {
                manager.networkAddress = "localhost";
                manager.StartHost();
                UIKit.OpenPanel<LodinPanel>();
            });

            clientBtn.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(ipInput.text))
                {
                    UIKit.OpenPanel<TipPanel>(new TipPanelData()
                    {
                        tip = "Please enter IP first.",
                    });
                    return;
                }
                else if (!IPValidator.IsValidIPv4(ipInput.text))
                {
                    UIKit.OpenPanel<TipPanel>(new TipPanelData()
                    {
                        tip = "IP format error.",
                    });
                    return;
                }

                LastUsedIP = ipInput.text;
                UIKit.OpenPanel<LodinPanel>();
                manager.StartClient();
                manager.networkAddress = ipInput.text;
            });

            if (closeTipBtn != null)
            {
                closeTipBtn.onClick.AddListener(() =>
                {
                    netTipPanel.SetActive(false);
                });
            }
        }

        protected override void OnOpen(IUIData uiData = null)
        {
        }

        protected override void OnShow()
        {
            if (netTipPanel != null)
            {
                netTipPanel.SetActive(true);
            }

            if (!string.IsNullOrEmpty(LastUsedIP))
            {
                ipInput.text = LastUsedIP;
            }
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            manager.UnRegisterOnStartServerEvent(OnStartServer);
            manager.UnRegisterOnStartClientEvent(OnStartClient);
        }

        [Server]
        private void OnStartServer()
        {
            NetworkServer.RegisterHandler<JoinRequest>(JoinRequestHandler);
        }

        [Server]
        private void JoinRequestHandler(NetworkConnectionToClient arg1, JoinRequest arg2)
        {
            var result = this.SendCommand<(bool, Player)>(new JoinConnectonCommand()
            {
                connection = arg1
            });

            if (result.Item1)
            {
                arg1.Send<JoinRespond>(new JoinRespond()
                {
                    success = true,
                    player = result.Item2
                });
            }
            else
            {
                arg1.Send<JoinRespond>(new JoinRespond()
                {
                    success = false
                });
            }

            if (this.GetModel<IPlayerConnectionModel>().IsStartCondition())
            {
                this.SendCommand<TwoConnectionCommand>();
            }
        }

        [Client]
        private void OnStartClient()
        {
            NetworkClient.Send<JoinRequest>(new JoinRequest());
            NetworkClient.RegisterHandler<JoinRespond>(JoinRespondHandler);
            NetworkClient.RegisterHandler<StartCondition>(StartConditionHandler);
        }

        [Client]
        private void StartConditionHandler(StartCondition obj)
        {
            UIKit.ClosePanel<LodinPanel>();
            CloseSelf();
            UIKit.OpenPanel<SelectIdentityPanel>(new SelectIdentityPanelData()
            {
                identity = this.GetModel<IPlayerModel>().player.m_identity
            });
        }

        [Client]
        private void JoinRespondHandler(JoinRespond obj)
        {
            if (obj.success)
            {
                this.SendCommand<SetPlayerCommand>(new SetPlayerCommand()
                {
                    m_player = obj.player
                });
            }
            else
            {
                UIKit.OpenPanel<TipPanel>(new TipPanelData()
                {
                    tip = "Fail to connect.",
                    callback = () =>
                    {
                        UIKit.OpenPanel<LodinPanel>();
                    }
                });
            }
        }

        public IArchitecture GetArchitecture()
        {
            return HideAndSeek.Interface;
        }
    }

    public struct JoinRequest : NetworkMessage { }
    public struct JoinRespond : NetworkMessage
    {
        public bool success;
        public Player player;
    }
    public struct StartCondition : NetworkMessage { }
}
