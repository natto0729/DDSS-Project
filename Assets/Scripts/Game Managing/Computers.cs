using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computers : MonoBehaviour
{
    public int index = 0;
    private int extraIndex = 0;

    public bool canRender = true;

    private Rendering[] computers;

    private Transform rand;

    public List<Transform> renderComputers = new List<Transform>();

    void Start()
    {
        computers = gameObject.GetComponentsInChildren<Rendering>();
    }
    
    public void AddRenderingComputer()
    {  
        canRender = false;      
        rand = computers[Random.Range(1,computers.Length)].transform;
        while(renderComputers.Contains(rand))
        {
            rand = computers[Random.Range(1,computers.Length)].transform;
        }
        renderComputers.Add(rand);
        renderComputers[index].GetComponent<Rendering>().enabled = true;
        renderComputers[index].GetChild(0).gameObject.SetActive(true);
        renderComputers[index].GetChild(1).gameObject.SetActive(true);
        index ++;

        foreach(Transform computer in renderComputers)
        {
            if(renderComputers[extraIndex].GetComponent<Rendering>().progress != null)
            {
                StopCoroutine(renderComputers[extraIndex].GetComponent<Rendering>().progress);
                renderComputers[extraIndex].GetComponent<Rendering>().progress = null;
            }
            renderComputers[extraIndex].GetComponent<Rendering>().currentRender = 0;
            if(renderComputers[extraIndex].GetComponent<Rendering>().progress == null)
            {
                renderComputers[extraIndex].GetComponent<Rendering>().progress = StartCoroutine(renderComputers[extraIndex].GetComponent<Rendering>().RenderingProgress());
            }
            extraIndex ++;
        }
        
        canRender = true;
        extraIndex = 0;
    }
}
