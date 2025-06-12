using System;
using HideAndSeek;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Events;

namespace QFramework.Example
{
	public class CatPanelData : UIPanelData
	{
		public CatEntity catEntity;
	}
	public partial class CatPanel : UIPanel
	{
		private UnityEvent scanTimer=new();
		private UnityEvent throwTimer=new();
		private UnityEvent seedUpTimer=new();

		public GameObject tipMouseFound;
		public GameObject tipMouseNotFound;
		
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as CatPanelData ?? new CatPanelData();
			// please add init code here
			scan.m_varyBtn.onClick.AddListener(() =>
			{
				mData.catEntity.ScanSkill();
				var timer = new CountdownTimer(20f, () =>
				{
					scan.SkillCollingEnd();
					scanTimer.RemoveAllListeners();
				});
				scanTimer.AddListener(()=>timer.Tick(Time.deltaTime));
				scan.SkillCooling(timer);
			});
			throwUp.m_varyBtn.onClick.AddListener(() =>
			{
				ThrowUpKill();
			});
			speedUp.m_varyBtn.onClick.AddListener(() =>
			{
				mData.catEntity.AccelerateSkill();
				var timer = new CountdownTimer(15f, () =>
				{
					speedUp.SkillCollingEnd();
					seedUpTimer.RemoveAllListeners();
				});
				seedUpTimer.AddListener(()=>timer.Tick(Time.deltaTime));
				speedUp.SkillCooling(timer);
			});
		}

		public void ThrowUpKill()
		{
			mData.catEntity.ThrowSkill();
			var timer = new CountdownTimer(3f, () =>
			{
				throwUp.SkillCollingEnd();
				throwTimer.RemoveAllListeners();
			});
			throwTimer.AddListener(()=>timer.Tick(Time.deltaTime));
			throwUp.SkillCooling(timer);
		}

		public void LockAllSkill()
		{
			scan.m_varyBtn.interactable = false;
			throwUp.m_varyBtn.interactable = false;
			speedUp.m_varyBtn.interactable = false;
		}
		public void UnLockAllSkill()
		{
			scan.m_varyBtn.interactable = true;
			throwUp.m_varyBtn.interactable = true;
			speedUp.m_varyBtn.interactable = true;
		}

		public void ShowScanTip(bool found)
		{
			tipMouseFound.SetActive(found);
			tipMouseNotFound.SetActive(!found);

			ActionKit.Delay(4f, () =>
			{
				tipMouseFound.SetActive(false);
				tipMouseNotFound.SetActive(false);
			}).Start(this);
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			tipMouseFound.SetActive(false);
    		tipMouseNotFound.SetActive(false);
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		private void Update()
		{
			scanTimer?.Invoke();
			throwTimer?.Invoke();
			seedUpTimer?.Invoke();
			if (throwUp.m_varyBtn.interactable &&Input.GetKeyDown(KeyCode.V) && throwUp.IsSkillAllReady())
			{
				ThrowUpKill();
			}
		}
	}
}
