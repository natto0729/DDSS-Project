using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Rendering : MonoBehaviour
{
    public Computers InteractablesParent;
    public float startRenderingDelay;
    public float renderTimeIncrement;
    public float renderValueIncrement;
    public float currentRender;
    public float maxRender;
    private int index = 0;
    public bool canRender = false;
    public bool isRendering = false;

    private void Update()
    {
        if(!canRender && !isRendering)
        {
            StopCoroutine(RenderingProgress());
            foreach(Transform computers in InteractablesParent.renderComputers)
            {
                if(InteractablesParent.renderComputers[index].GetComponent<Rendering>().currentRender != 0)
                {
                    canRender = false;
                    index = 0;
                }
                else
                {
                    if(index + 1 == InteractablesParent.renderComputers.Count)
                    {
                        canRender = true;
                    }
                    else
                    {
                        index ++;
                    }
                }
            }
        }
        else if(canRender && !isRendering)
        {
            StartCoroutine(RenderingProgress());
        }
    }

    private IEnumerator RenderingProgress()
    {
        isRendering = true;
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
