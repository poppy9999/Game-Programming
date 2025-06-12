using System;
using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using QFramework.Example;
using UnityEngine.SceneManagement;

namespace HideAndSeek
{
	
	public class SelectIdentityPanelData : UIPanelData
	{
		public Identity identity;
	}
	public partial class SelectIdentityPanel : UIPanel,IController
	{
		private Guid catCtrlPlayerId;
		private Guid mouseCtrlPlayerId;
		private ResLoader resLoader;
		private My_NetworkManager manager => My_NetworkManager.Instance;
		public Material fixedSkyboxMaterial;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as SelectIdentityPanelData ?? new SelectIdentityPanelData();
			// please add init code here
			manager.RegisterOnStartServerEvent(OnStartServer);
			manager.RegisterOnclientConnectEvent(OnStartClient);
			playerConnectionModel = this.GetModel<IPlayerConnectionModel>();
			playerModel = this.GetModel<IPlayerModel>();
			resLoader = ResLoader.Allocate();
			catBtn.onClick.AddListener(() =>
			{
				SelectIdentityPanelChooseBtnRequestMessage message = new SelectIdentityPanelChooseBtnRequestMessage();
				if (!mouseBtn.interactable && mouseCtrlPlayerId == playerModel.player.m_id)
				{
					message.unlockBtnName = GameType.Mouse;
				}
				
				message.playerId = playerModel.player.m_id;
				message.lockBtnName = GameType.Cat;
				NetworkClient.Send<SelectIdentityPanelChooseBtnRequestMessage>(message);
			});
			mouseBtn.onClick.AddListener(() =>
			{
				SelectIdentityPanelChooseBtnRequestMessage message = new SelectIdentityPanelChooseBtnRequestMessage();
				if (!catBtn.interactable && catCtrlPlayerId == playerModel.player.m_id)
				{
					message.unlockBtnName = GameType.Cat;
				}
				message.playerId = playerModel.player.m_id;
				message.lockBtnName = GameType.Mouse;
				NetworkClient.Send<SelectIdentityPanelChooseBtnRequestMessage>(message);
			});
			cancelBtn.onClick.AddListener(() =>
			{
				SelectIdentityPanelChooseBtnRequestMessage message = new SelectIdentityPanelChooseBtnRequestMessage();
				if (catBtn.interactable && mouseBtn.interactable)
				{
					return;
				}
				else if (!catBtn.interactable && catCtrlPlayerId == playerModel.player.m_id)
				{
					message.unlockBtnName = GameType.Cat;
				}
				else if (!mouseBtn.interactable && mouseCtrlPlayerId == playerModel.player.m_id)
				{
					message.unlockBtnName = GameType.Mouse;
				}
				message.playerId = playerModel.player.m_id;
				NetworkClient.Send<SelectIdentityPanelChooseBtnRequestMessage>(message);
			});
			if (playerModel.player.m_identity== Identity.Homeowner)
			{
				startBtn.Show();
			}
			else
			{
				startBtn.Hide();
			}
			startBtn.interactable = false;
			startBtn.onClick.AddListener(() =>
			{
				NetworkClient.Send<StartGameRequestMessage>(new StartGameRequestMessage());
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
			resLoader.Recycle2Cache();
			resLoader = null;
			manager.UnRegisterOnStartServerEvent(OnStartServer);
			manager.UnRegisterOnStartClientEvent(OnStartClient);
		}

		private IPlayerConnectionModel playerConnectionModel;
		private IPlayerModel playerModel;
		[Server]
		private void OnStartServer()
		{
			NetworkServer.RegisterHandler<SelectIdentityPanelChooseBtnRequestMessage>(SelectIdentityPanelChooseBtnRequestMessageHandler);
			NetworkServer.RegisterHandler<StartGameRequestMessage>(StartGameRequestMessageHandler);
		}
		[Server]
		private void StartGameRequestMessageHandler(NetworkConnectionToClient arg1, StartGameRequestMessage arg2)
		{
			int mapSeed = UnityEngine.Random.Range(0, int.MaxValue);
			int mapType = PlayerPrefs.GetInt("MapType", 1);

			playerConnectionModel.Players.ForEach(x =>
			{
				x.Value.m_connection.Send<StartGameResponseMessage>(new StartGameResponseMessage
				{
					mapSeed = mapSeed,
            		mapType = mapType
				});
			});
		}

		[Server]
		private void SelectIdentityPanelChooseBtnRequestMessageHandler(NetworkConnectionToClient arg1, SelectIdentityPanelChooseBtnRequestMessage arg2)
		{
			SelectIdentityPanelChooseBtnResponseMessage message = new();
			message.playerId = arg2.playerId;
			message.lockBtnName = arg2.lockBtnName;
			playerConnectionModel.Players[arg2.playerId].player.m_gameType = arg2.lockBtnName;
			message.unlockBtnName = arg2.unlockBtnName;
			playerConnectionModel.Players[arg2.playerId].player.m_gameType = GameType.None;
			playerConnectionModel.Players.ForEach(x =>
			{
				x.Value.m_connection.Send<SelectIdentityPanelChooseBtnResponseMessage>(message);
			});
		}

		[Client]
		private void OnStartClient()
		{
			NetworkClient.RegisterHandler<SelectIdentityPanelChooseBtnResponseMessage>(SelectIdentityPanelChooseBtnRespondMessageHandler);
			NetworkClient.RegisterHandler<StartGameResponseMessage>(StartGameResponseMessageHandler);
		}
		[Client]
		private void StartGameResponseMessageHandler(StartGameResponseMessage obj)
		{
			UIKit.OpenPanel<LodinPanel>();
			string sceneToLoad = obj.mapType switch
			{
				1 => "Game - Park",
				2 => "Game - Parkinglot",
				3 => "GeneratedScene"
			};

			resLoader.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive, LocalPhysicsMode.None, op =>
			{
				op.completed += _ =>
				{
					
					ActionKit.Delay(1f, () =>
					{
						Scene newScene = SceneManager.GetSceneByName(sceneToLoad);
						SceneManager.SetActiveScene(newScene);
						if (obj.mapType == 3) // 只有 Custom 才生成地图
            			{
							foreach (GameObject root in newScene.GetRootGameObjects())
							{
								var generator = root.GetComponentInChildren<CityGenerator>();
								if (generator != null)
								{
									generator.GenerateWithSeed(obj.mapSeed); // 让城市用种子生成
								}
							}
						}
						LightmapSettings.lightmaps = LightmapSettings.lightmaps;
						RenderSettings.skybox = fixedSkyboxMaterial;
						DynamicGI.UpdateEnvironment(); // 更新环境反射和光照
						NetworkClient.Send<OnAllSceneLoadRequestMessage>(new OnAllSceneLoadRequestMessage());
						
						// this.GetSystem<GameEventSystem>().OnEnterGame.Trigger();
						CloseSelf();
					}).Start(this);
				};
			});
		}

		private Material FindSkyboxFromScene(Scene scene)
		{
			foreach (GameObject root in scene.GetRootGameObjects())
			{
				var lighting = root.GetComponentInChildren<CityGenerator>(); // 或其他你设置天空盒的脚本
				if (lighting != null && lighting.skyboxMaterial != null)
				{
					return lighting.skyboxMaterial;
				}
			}

			Debug.LogWarning("未在场景中找到 skybox 材质，使用默认");
			return RenderSettings.skybox; // 或 null
		}


		[Client]
		private void SelectIdentityPanelChooseBtnRespondMessageHandler(SelectIdentityPanelChooseBtnResponseMessage obj)
		{
			switch (obj.unlockBtnName)
			{
				case GameType.Cat :
					catBtn.interactable = true;
					catCtrlPlayerId = Guid.Empty;
					break;
				case GameType.Mouse:
					mouseBtn.interactable = true;
					mouseCtrlPlayerId = Guid.Empty;
					break;
			}
			if (obj.playerId==playerModel.player.m_id)
			{
				playerModel.player.m_gameType = GameType.None;
			}

			switch (obj.lockBtnName)
			{
				case GameType.Cat :
					catBtn.interactable = false;
					catCtrlPlayerId = obj.playerId;
					break;
				case GameType.Mouse:
					mouseBtn.interactable = false;
					mouseCtrlPlayerId = obj.playerId;
					break;
			}
			if (obj.playerId==playerModel.player.m_id)
			{
				playerModel.player.m_gameType = obj.lockBtnName;
			}

			CheckAllReady();
		}

		public void CheckAllReady()
		{
			if (!catBtn.interactable && !mouseBtn.interactable)
			{
				startBtn.interactable = true;
			}
			else
			{
				startBtn.interactable = false;
			}
		}

		public IArchitecture GetArchitecture()
		{
			return HideAndSeek.Interface;
		}
	}
	[Serializable]
	public struct SelectIdentityPanelChooseBtnRequestMessage : NetworkMessage
	{
		public Guid playerId;
		public GameType lockBtnName;
		public GameType unlockBtnName;
	}
	[Serializable]
	public struct SelectIdentityPanelChooseBtnResponseMessage : NetworkMessage
	{
		public Guid playerId;
		public GameType lockBtnName;
		public GameType unlockBtnName;
	}
	[Serializable]
	public struct StartGameRequestMessage : NetworkMessage
	{
		
	}
	[Serializable]
	public struct StartGameResponseMessage : NetworkMessage
	{
		public int mapSeed;
		public int mapType;
		
	}
	[Serializable]
	public struct InitPlayerRequestMessage : NetworkMessage
	{
		public GameType gameType;
	}
	[Serializable]
	public struct InitPlayerResponseMessage : NetworkMessage
	{
		
	}

	public struct OnAllSceneLoadRequestMessage : NetworkMessage
	{
		
	}
	public struct OnAllSceneLoadResponseMessage : NetworkMessage
	{
		
	}
}
