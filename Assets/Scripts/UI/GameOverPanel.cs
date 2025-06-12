using HideAndSeek;
using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

namespace HideAndSeek
{
    public class GameOverPanelData : UIPanelData
    {
        public GameType mGameType;
        public bool isWin;
    }

    public partial class GameOverPanel : UIPanel,  IController
    {
        public Sprite mouseWin;
        public Sprite catWin;
        public Sprite mouseLose;
        public Sprite catLose;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as GameOverPanelData ?? new GameOverPanelData();
            // please add init code here
            if (mData.mGameType == GameType.Cat)
            {
                if (mData.isWin)
                    bg.sprite = catWin;
                else
                    bg.sprite = catLose;
            }
            else
            {
                if (mData.isWin)
                    bg.sprite = mouseWin;
                else
                    bg.sprite = mouseLose;
            }
            quitBtn.onClick.AddListener(() =>
            {
                if (NetworkClient.isConnected)
                    NetworkClient.Disconnect();

                if (NetworkServer.active)
                    NetworkServer.Shutdown();

                UIKit.CloseAllPanel();     

                SceneManager.LoadScene("StartScene"); 
            });

        }

        protected override void OnOpen(IUIData uiData = null)
        {
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
        }

        public IArchitecture GetArchitecture()
        {
            return HideAndSeek.Interface;
        }
    }
}