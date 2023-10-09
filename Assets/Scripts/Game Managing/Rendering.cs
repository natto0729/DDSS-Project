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

    public Coroutine progress;

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
