using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:27f4d8ba-ca01-43fb-81e8-74fc1ad41459
	public partial class TipPanel
	{
		public const string Name = "TipPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button closeBtn;
		[SerializeField]
		public TMPro.TextMeshProUGUI tipText;
		
		private TipPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			closeBtn = null;
			tipText = null;
			
			mData = null;
		}
		
		public TipPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		TipPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new TipPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
