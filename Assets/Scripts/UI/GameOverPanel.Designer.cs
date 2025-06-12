using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace HideAndSeek
{
	// Generate Id:97c37ca7-e4f4-4c92-9db8-a5d938582914
	public partial class GameOverPanel
	{
		public const string Name = "GameOverPanel";
		
		[SerializeField]
		public UnityEngine.UI.Image bg;
		[SerializeField]
		public UnityEngine.UI.Button replayBtn;
		[SerializeField]
		public UnityEngine.UI.Button quitBtn;
		
		private GameOverPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			bg = null;
			replayBtn = null;
			quitBtn = null;
			
			mData = null;
		}
		
		public GameOverPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		GameOverPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new GameOverPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
