using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class PlatformSelector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(XRSettings.enabled)
        {
            SceneManager.LoadScene("MainMenuExample");
        }
        else if(!XRSettings.enabled)
        {
            SceneManager.LoadScene("Title Scene");
        }
    }
}
