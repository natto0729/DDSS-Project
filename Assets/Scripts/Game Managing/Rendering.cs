using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
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

    public bool isRendering = false;

    [PunRPC]
    public void StopProgress()
    {
        isRendering =false;
        StopCoroutine(RenderingProgress());
    }

    [PunRPC]
    public void StartProgress()
    {
        renderingPopUp.GetComponent<TextMeshPro>().SetText("Rendering Stopped!");
        renderingPopUp.GetComponent<TextMeshPro>().color = Color.red;
        StartCoroutine(RenderingProgress());
    }

    private void Start()
    {
        renderingPopUp = gameObject.transform.GetChild(1);
    }

    public void Update()
    {
        if(isRendering)
        {
            RenderText();
        }
    }

    public void RenderText()
    {
        renderingPopUp.GetComponent<TextMeshPro>().SetText("Rendering:" + currentRender + "%");
        renderingPopUp.GetComponent<TextMeshPro>().color = Color.white;
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
        isRendering = true;
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

        isRendering = false;
    }
}
