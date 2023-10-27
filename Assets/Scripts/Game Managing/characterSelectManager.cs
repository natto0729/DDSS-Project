using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class characterSelectManager : MonoBehaviour
{
    public GameObject VRRig;
    public GameObject canvas;
    public GameObject pcCamera;
  

    void Start()
    {
        if(XRSettings.enabled)
        {
            VRRig.SetActive(true);
            canvas.SetActive(false);
            pcCamera.SetActive(true);
        }
        else
        {
            canvas.SetActive(true);
            pcCamera.SetActive(true);
            VRRig.SetActive(false);
        }
    }

   
}
