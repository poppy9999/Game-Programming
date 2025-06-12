using System;
using HideAndSeek;
using UnityEngine;

public class PulsingScanEffect : MonoBehaviour
{
    [Header("扫描设置")]
    [Tooltip("最大扫描范围（localScale x/y/z 的目标值）")]
    public float maxRange = 5f;

    [Tooltip("扫描周期（往返一次所需时间）")]
    public float pulseDuration = 2f;

    [Tooltip("是否启用扫描效果")]
    public bool scanActive = true;

    private Vector3 initialScale;
    private float timer = 0f;

    void Start()
    {
        initialScale = Vector3.zero;
        transform.localScale = initialScale;
    }

    void Update()
    {
        if (!scanActive) return;

        timer += Time.deltaTime;

        // 计算 t 在 [0, 1] → [1, 0] 循环之间来回
        float t = Mathf.PingPong(timer / (pulseDuration / 2f), 1f);

        // 应用缩放
        float currentRange = Mathf.Lerp(0f, maxRange, t);
        transform.localScale = new Vector3(currentRange, currentRange, currentRange);
    }

    public void ActivateScan()
    {
        scanActive = true;
        timer = 0f;
    }

    public void DeactivateScan()
    {
        scanActive = false;
        transform.localScale = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject)
        {
            var mouse = other.gameObject.GetComponent<MouseEntity>();
            if (mouse)
            {
                mouse.Scan();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject)
        {
            var mouse = other.gameObject.GetComponent<MouseEntity>();
            if (mouse)
            {
                mouse.UnScan();
            }
        }
    }
}
