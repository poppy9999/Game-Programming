using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace HideAndSeek
{
	// Generate Id:4c8fdfe2-f200-4db6-a28b-da052d6aa822
	public partial class StartPanel
	{
		public const string Name = "StartPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button serverBtn;
		[SerializeField]
		public UnityEngine.UI.Button clientBtn;
		[SerializeField]
		public TMPro.TMP_InputField ipInput;
		
		private StartPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			serverBtn = null;
			clientBtn = null;
			ipInput = null;
			
			mData = null;
		}
		
		public StartPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		StartPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new StartPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
