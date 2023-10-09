using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public Computers InteractablesParent;
    public int renderTotal;
    private int index;

    public Door finalDoor1;
    public Door finalDoor2;

    // Update is called once per frame
    void Update()
    {
        RenderProgress();
        if(renderTotal == 100)
        {
            finalDoor1.GetComponent<PhotonView>().RPC ("OpenNetwork",RpcTarget.AllBuffered, new Vector3(0,0,0));
            finalDoor2.GetComponent<PhotonView>().RPC ("OpenNetwork",RpcTarget.AllBuffered, new Vector3(0,0,0));
        }
    }

    private void RenderProgress()
    {
        renderTotal = 0;
        foreach(Transform computer in InteractablesParent.renderComputers)
        {
            renderTotal += (int) InteractablesParent.renderComputers[index].GetComponent<Rendering>().currentRender;

            index++;
        }

        if(InteractablesParent.renderComputers.Count > 1)
        {
            renderTotal = renderTotal/InteractablesParent.renderComputers.Count;
        }

        index = 0;
    }
}
