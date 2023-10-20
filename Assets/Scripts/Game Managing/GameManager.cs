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

    private int index;
    bool loadChar = false;

    public Door finalDoor1;
    public Door finalDoor2;

    [PunRPC]
    void SpawnStuff()
    {
        PhotonNetwork.Instantiate(characters[PhotonNetwork.LocalPlayer.CustomProperties["selected"].Get<int>()].name, Vector3.zero, Quaternion.identity);
    }

    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsMasterClient && loadChar == false)
        {
            gameObject.GetComponent<PhotonView>().RPC("SpawnStuff", RpcTarget.AllBuffered, null);
            loadChar = true;
        }
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
