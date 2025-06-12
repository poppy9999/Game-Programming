using System;
using UnityEngine;

public class CountdownTimer
{
    public float TotalTime { get; private set; }     // 总时长
    public float RemainingTime { get; private set; } // 剩余时间
    public bool IsRunning { get; private set; }      // 是否正在倒计时

    private Action onCompleted;      // 倒计时结束时的回调
    private Action<float> onUpdate;  // 每帧更新时间的回调

    public CountdownTimer(float totalTime, Action onCompleted = null, Action<float> onUpdate = null)
    {
        TotalTime = totalTime;
        RemainingTime = totalTime;
        this.onCompleted = onCompleted;
        this.onUpdate = onUpdate;
        Start();
    }

    public void Start()
    {
        IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = false;
    }
    /// <summary>
    /// 获取剩余时间占总时间的百分比（0~1）
    /// </summary>
    public float GetProgress()
    {
        return TotalTime <= 0 ? 0f : Mathf.Clamp01(RemainingTime / TotalTime);
    }


    public void Reset(float newTime = -1)
    {
        RemainingTime = newTime >= 0 ? newTime : TotalTime;
    }

    public void Tick(float deltaTime)
    {
        if (!IsRunning) return;

        RemainingTime -= deltaTime;
        onUpdate?.Invoke(Mathf.Max(RemainingTime, 0));

        if (RemainingTime <= 0f)
        {
            Stop();
            onCompleted?.Invoke();
        }
    }
}