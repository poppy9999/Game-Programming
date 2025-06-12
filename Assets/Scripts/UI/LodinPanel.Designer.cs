using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:2dbcf6d3-3ee8-4c5a-a59e-cc6347b28f44
	public partial class LodinPanel
	{
		public const string Name = "LodinPanel";
		
		
		private LodinPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			
			mData = null;
		}
		
		public LodinPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		LodinPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new LodinPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
