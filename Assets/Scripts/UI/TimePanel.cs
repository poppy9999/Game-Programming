using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class TimePanelData : UIPanelData
	{
		public bool startTime;
		public CountdownTimer timer;
	}
	public partial class TimePanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as TimePanelData ?? new TimePanelData();
			// please add init code here
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

		private void Update()
		{
			if (mData.timer!=null)
			{
				mData.timer.Tick(Time.deltaTime);
				if (mData.startTime)
				{
					TimeText.text = "There are "+mData.timer.RemainingTime.ToString("00")+" seconds left until the cat action";
				}
				else
				{
					TimeText.text = "There are " + mData.timer.RemainingTime.ToString("00") +
					                " seconds left until the end of the game";
				}
			}
		}
	}
}
