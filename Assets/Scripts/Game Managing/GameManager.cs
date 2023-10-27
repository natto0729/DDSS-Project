using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Computers InteractablesParent;
    public int renderTotal;
    public GameObject[] characters;

    private int index;
    private int numberOfComputers;
    bool loadChar = false;
    public bool testPlay = false;

    private GameManager gameManager;

    public Door finalDoor1;
    public Door finalDoor2;
    public Door finalDoor3;
    public Door finalDoor4;

    bool isVR = false;

    [PunRPC]
    void SpawnStuff()
    {
        if(XRSettings.enabled)
        {
            PhotonNetwork.Instantiate("OVRPlayerController Variant", Vector3.zero, Quaternion.identity);
        }
        else if(!XRSettings.enabled)
        {
            PhotonNetwork.Instantiate(characters[PhotonNetwork.LocalPlayer.CustomProperties["selected"].Get<int>()].name, Vector3.zero, Quaternion.identity);
        }
    }

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    // Update is called once per frame
    void Update()
    {
  
        if (PhotonNetwork.IsMasterClient && loadChar == false && testPlay == false)
        {
            gameObject.GetComponent<PhotonView>().RPC("SpawnStuff", RpcTarget.AllBuffered, null);
            loadChar = true;
        }
        RenderProgress();
        if(renderTotal == 100)
        {
            finalDoor1.GetComponent<PhotonView>().RPC ("OpenNetwork",RpcTarget.AllBuffered, new Vector3(0,0,0));
            finalDoor2.GetComponent<PhotonView>().RPC ("OpenNetwork",RpcTarget.AllBuffered, new Vector3(0,0,0));
            finalDoor3.GetComponent<PhotonView>().RPC("OpenNetwork", RpcTarget.AllBuffered, new Vector3(0,0,0));
            finalDoor4.GetComponent<PhotonView>().RPC("OpenNetwork", RpcTarget.AllBuffered, new Vector3(0,0,0));
        }
    }

    private void RenderProgress()
    {
        renderTotal = 0;
        foreach(Rendering computer in InteractablesParent.computers)
        {
            if(InteractablesParent.computers[index].enabled)
            {
                renderTotal += (int) InteractablesParent.computers[index].GetComponent<Rendering>().currentRender;
                numberOfComputers += 1;
            }

            index++;
        }
        
        if(numberOfComputers > 1)
        {
            renderTotal = renderTotal/numberOfComputers;
        }

        numberOfComputers = 0;
        index = 0;
    }

}
