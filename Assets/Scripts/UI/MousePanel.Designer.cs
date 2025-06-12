using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:b0d83ac2-ab40-4653-a155-052138dc1661
	public partial class MousePanel
	{
		public const string Name = "MousePanel";
		
		[SerializeField]
		public UnityEngine.UI.Button SelectVaryButton;
		[SerializeField]
		public RectTransform Skill;
		[SerializeField]
		public MouseVaryItem Street_Lamp;
		[SerializeField]
		public MouseVaryItem Hydrant;
		[SerializeField]
		public MouseVaryItem Car;
		
		private MousePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			SelectVaryButton = null;
			Skill = null;
			Street_Lamp = null;
			Hydrant = null;
			Car = null;
			
			mData = null;
		}
		
		public MousePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		MousePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new MousePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
