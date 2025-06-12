using System;
using System.Collections;
using System.Collections.Generic;
using HideAndSeek;
using Mirror;
using QFramework;
using UnityEngine;

public class Launch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ResKit.Init();
        UIKit.Root.SetResolution(1920,1080,1);
        UIKit.OpenPanel<StartPanel>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
