using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:3f9c2821-8fc1-4c88-a0ab-efbb7d7de1d4
	public partial class CatPanel
	{
		public const string Name = "CatPanel";
		
		[SerializeField]
		public MouseVaryItem speedUp;
		[SerializeField]
		public MouseVaryItem scan;
		[SerializeField]
		public MouseVaryItem throwUp;
		
		private CatPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			speedUp = null;
			scan = null;
			throwUp = null;
			
			mData = null;
		}
		
		public CatPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		CatPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new CatPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
