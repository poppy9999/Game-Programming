using System;
using HideAndSeek;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Events;

namespace QFramework.Example
{
	public class MousePanelData : UIPanelData
	{
		public MouseEntity mouseEntity;
	}
	public partial class MousePanel : UIPanel
	{
		public UnityEvent TimerUpdte;
		public GameObject map1Mark;
    	public GameObject map2Mark;
    	public GameObject map3Mark;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as MousePanelData ?? new MousePanelData();
			Skill.Hide();
			SelectVaryButton.onClick.AddListener(() =>
			{
				Skill.gameObject.SetActive(!Skill.gameObject.activeSelf);
			});
			Street_Lamp.m_varyBtn.onClick.AddListener(() =>
			{
				
				mData.mouseEntity.SetMouseVary(MouseVary.Street_Lamp);
				var timer = new CountdownTimer(30f, () =>
				{
					Street_Lamp.SkillCollingEnd();
					Hydrant.SkillCollingEnd();
					Car.SkillCollingEnd();
					TimerUpdte.RemoveAllListeners();
				});
				TimerUpdte.AddListener(() =>
				{
					timer.Tick(Time.deltaTime);
				});
				SetAllSkillCooling(timer);
				map1Mark.SetActive(false);
				map2Mark.SetActive(false);
				map3Mark.SetActive(true);
			});
			Hydrant.m_varyBtn.onClick.AddListener(() =>
			{
				
				mData.mouseEntity.SetMouseVary(MouseVary.Hydrant);
				var timer = new CountdownTimer(30f, () =>
				{
					Street_Lamp.SkillCollingEnd();
					Hydrant.SkillCollingEnd();
					Car.SkillCollingEnd();
					TimerUpdte.RemoveAllListeners();
				});
				TimerUpdte.AddListener(() =>
				{
					timer.Tick(Time.deltaTime);
				});
				SetAllSkillCooling(timer);
				map1Mark.SetActive(true);
				map2Mark.SetActive(false);
				map3Mark.SetActive(false);
			});
			Car.m_varyBtn.onClick.AddListener(() =>
			{
				mData.mouseEntity.SetMouseVary(MouseVary.Car);
				
				var timer = new CountdownTimer(30f, () =>
				{
					Street_Lamp.SkillCollingEnd();
					Hydrant.SkillCollingEnd();
					Car.SkillCollingEnd();
					TimerUpdte.RemoveAllListeners();
				});
				TimerUpdte.AddListener(() =>
				{
					timer.Tick(Time.deltaTime);
				});
				SetAllSkillCooling(timer);
				
				map1Mark.SetActive(false);
				map2Mark.SetActive(true);
				map3Mark.SetActive(false);
			});
		}

		public void ShowAllBtn(bool isShow)
		{
			Street_Lamp.gameObject.SetActive(isShow);
			Hydrant.gameObject.SetActive(isShow);
			Car.gameObject.SetActive(isShow);
		}

		public void SetAllSkillCooling(CountdownTimer timer)
		{
			Street_Lamp.SkillCooling(timer);
			Hydrant.SkillCooling(timer);
			Car.SkillCooling(timer);
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			map1Mark.SetActive(true);
        	map2Mark.SetActive(false);
        	map3Mark.SetActive(false);
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		private void Update()
		{
			TimerUpdte?.Invoke();
		}
	}
}
