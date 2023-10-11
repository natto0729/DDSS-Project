using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

public class Rendering : MonoBehaviour
{
    public Computers InteractablesParent;
    public float startRenderingDelay;
    public float renderTimeIncrement;
    public float renderValueIncrement;
    public float currentRender;
    public float maxRender;

    public Coroutine progress;

    private Transform renderingPopUp;

    private void Start()
    {
        renderingPopUp = gameObject.transform.GetChild(1);
    }

    public void Update()
    {
        RenderText();
    }

    public void RenderText()
    {
        renderingPopUp.GetComponent<TextMeshPro>().SetText("Rendering:" + currentRender + "%");
        if(XRSettings.enabled)
        {
            renderingPopUp.LookAt(GameObject.Find("CenterEyeAnchor").transform);
        }
        else
        {
            renderingPopUp.LookAt(GameObject.Find("MainCamera Variant").transform);
        }
    }

    public IEnumerator RenderingProgress()
    {
        while(!InteractablesParent.canRender)
        {
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(startRenderingDelay);
        WaitForSeconds timeToWait = new WaitForSeconds(renderTimeIncrement);

        while(currentRender < maxRender)
        {
            currentRender += renderValueIncrement;

            if(currentRender > maxRender)
            {
                currentRender = maxRender;
            }

            yield return timeToWait;
        }
    }
}
