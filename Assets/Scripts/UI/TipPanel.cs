using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	public class TipPanelData : UIPanelData
	{
		public string tip;
		public Action callback;
	}
	public partial class TipPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as TipPanelData ?? new TipPanelData();
			tipText.text = mData.tip;
			Debug.Log($"tipText is null? {tipText == null}");
			Debug.Log($"closeBtn is null? {closeBtn == null}");
			closeBtn.onClick.AddListener(() =>
			{
				Debug.Log("Close button clicked");
				mData.callback?.Invoke();
				CloseSelf();
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
	}
}
