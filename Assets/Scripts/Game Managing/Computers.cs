using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computers : MonoBehaviour
{
    private int index = 0;
    private int extraIndex = 0;

    private Transform[] computers;

    public List<Transform> renderComputers = new List<Transform>();

    void Start()
    {
        computers = gameObject.GetComponentsInChildren<Transform>();
    }

    public void AddRenderingComputer()
    {
        foreach(Transform computer in renderComputers)
        {
            renderComputers[extraIndex].GetComponent<Rendering>().currentRender = 0;
            renderComputers[extraIndex].GetComponent<Rendering>().isRendering = false;
            renderComputers[extraIndex].GetComponent<Rendering>().canRender = false;
            extraIndex ++;
        }
        renderComputers.Add(computers[Random.Range(1,computers.Length)]);
        renderComputers[index].GetComponent<Rendering>().enabled = true;
        index ++;
    }
}
