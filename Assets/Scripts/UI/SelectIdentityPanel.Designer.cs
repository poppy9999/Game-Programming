using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace HideAndSeek
{
	// Generate Id:0a120b49-2367-4cba-8d84-9e1e8b065751
	public partial class SelectIdentityPanel
	{
		public const string Name = "SelectIdentityPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button catBtn;
		[SerializeField]
		public UnityEngine.UI.Button mouseBtn;
		[SerializeField]
		public UnityEngine.UI.Button cancelBtn;
		[SerializeField]
		public UnityEngine.UI.Button startBtn;
		
		private SelectIdentityPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			catBtn = null;
			mouseBtn = null;
			cancelBtn = null;
			startBtn = null;
			
			mData = null;
		}
		
		public SelectIdentityPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		SelectIdentityPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new SelectIdentityPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
