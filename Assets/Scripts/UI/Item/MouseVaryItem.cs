using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseVaryItem : MonoBehaviour
{
    public Image maskImg;
    public TextMeshProUGUI maskText;
    public Button m_varyBtn;
    private CountdownTimer timer;

    public bool showOtherObj;
    public GameObject otherObj;

    private void Awake()
    {
        maskImg.Hide();
        if (showOtherObj)
        {
            otherObj.Hide();
        }
        
    }

    public void SkillCollingEnd()
    {
        maskImg.Hide();
        timer = null;
        if (showOtherObj)
        {
            otherObj.Hide();
        }
    }

    public bool IsSkillAllReady()
    {
        if (timer == null)
        {
            return true;
        }
        return timer.IsRunning == false;
    }

    public void SkillCooling(CountdownTimer timer)
    {
        maskImg.Show();
        this.timer = timer;
        if (showOtherObj)
        {
            otherObj.Show();
        }
    }

    private void Update()
    {
        if (timer != null && timer.IsRunning)
        {
            maskImg.fillAmount = timer.GetProgress();
            maskText.text = timer.RemainingTime.ToString("0.0");
        }
    }
}
