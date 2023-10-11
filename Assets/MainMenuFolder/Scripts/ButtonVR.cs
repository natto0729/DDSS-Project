using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ButtonVR : MonoBehaviour
{
    public GameObject highlight;
    public UnityEvent onPress;

    float Timer;

    private void Update()
    {
        if(Timer > 0)
        {
            Timer -= Time.deltaTime;
            if(Timer <= 0)
            {
                highlight.SetActive(false);
            }
        }
    }

    public void OnHover()
    {
        highlight.SetActive(true);
        Timer = 0.1f;
    }

    public void Interact()
    {
        Debug.Log("Button Pressed");
        onPress.Invoke();
    }
}
