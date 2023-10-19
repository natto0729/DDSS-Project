using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping;

public class GameManager : MonoBehaviour
{
    public Computers InteractablesParent;
    public int renderTotal;
    public GameObject[] characters;

    bool isMaster = false;

    private int index;

    public Door finalDoor1;
    public Door finalDoor2;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.Instantiate(characters[PhotonNetwork.LocalPlayer.CustomProperties["selected"].Get<int>()].name, Vector3.zero, Quaternion.identity);
    }


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
