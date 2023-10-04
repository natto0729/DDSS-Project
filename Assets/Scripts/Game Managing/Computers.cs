using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computers : MonoBehaviour
{
    int index = 0;

    private Transform[] computers;

    private List<Transform> renderComputers = new List<Transform>();

    void Start()
    {
        computers = gameObject.GetComponentsInChildren<Transform>();
    }

    public void AddRenderingComputer()
    {
        renderComputers.Add(computers[Random.Range(1,computers.Length)]);
        renderComputers[index].GetComponent<Rendering>().enabled = true;
        index ++;
    }
}
