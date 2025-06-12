using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:00ca465b-78c1-4606-93c7-0b62828cd021
	public partial class TimePanel
	{
		public const string Name = "TimePanel";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI TimeText;
		
		private TimePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			TimeText = null;
			
			mData = null;
		}
		
		public TimePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		TimePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new TimePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
