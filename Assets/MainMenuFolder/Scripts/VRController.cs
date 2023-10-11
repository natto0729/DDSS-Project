using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRController : MonoBehaviour
{

    public LineRenderer lineRenderer;
    public LayerMask layerMask;

    private void Update()
    {
        if(lineRenderer != null)
        UpdateLineRenderer();
    }

    void UpdateLineRenderer()
    {
        lineRenderer.SetPosition(0, transform.position); //Start LineRenderer
        //Check if a ray hits something
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, 10, layerMask))
        {
            //Hits something
            //Stop ray at gameobject position
            lineRenderer.SetPosition(1, hit.point);
            if(hit.collider.gameObject.tag == "Button")
            {
                hit.collider.gameObject.GetComponent<ButtonVR>().OnHover();
            }
        }
        else
        {
            //didn' hit anything
            //Set rays end 10 units forward
            lineRenderer.SetPosition(1, transform.position + (transform.forward * 10));
        }
    }

    public void TriggerPressed() 
    {
        Debug.Log("TriggerPressed");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10, layerMask))
        {
            //Hits something
            if (hit.collider.gameObject.tag == "Button")
            {
                hit.collider.gameObject.GetComponent<ButtonVR>().Interact();
            }
        }

    }
}
