using System;
using System.Collections;
using System.Collections.Generic;
using HideAndSeek;
using UnityEngine;

public class ThrowEntity : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject)
        {
            var mouse = other.gameObject.GetComponent<MouseEntity>();
            if (mouse)
            {
                mouse.MouseLose();
            }
        }
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.gameObject)
    //     {
    //         var mouse = other.gameObject.GetComponent<MouseEntity>();
    //         if (mouse)
    //         {
    //             mouse.UnScan();
    //         }
    //     }
    // }
}
