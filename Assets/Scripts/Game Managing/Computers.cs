using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

public class Computers : MonoBehaviour
{
    public int numberOfComputers = 0;

    public bool canRender = true;

    public Rendering[] computers;

    private int rand;

    private int index = 0;

    private int saved;

    void Start()
    {
        computers = gameObject.GetComponentsInChildren<Rendering>();
    }

    [PunRPC]
    public void ActivateComputer(int rand)
    {
        saved = rand;
        computers[saved].GetComponent<Rendering>().enabled = true;
        computers[saved].transform.GetChild(0).gameObject.SetActive(true);
        computers[saved].transform.GetChild(1).gameObject.SetActive(true);
    }

    [PunRPC]
    public void SyncTime()
    {
        canRender =false;
        if(gameObject.GetComponent<PhotonView>().IsMine && !XRSettings.enabled)
        {
            gameObject.GetComponent<PhotonView>().RPC("ActivateComputer", RpcTarget.AllBuffered, rand);
        }
        foreach(Rendering computer in computers)
        {
            if(computers[index].enabled)
            {
                if(computers[index].progress != null)
                {
                    StopCoroutine(computers[index].progress);
                    computers[index].progress = null;
                }
                computers[index].currentRender = 0;
                if(computers[index].progress == null)
                {
                    computers[index].progress = computers[index].RenderingProgress();
                    StartCoroutine(computers[index].progress);
                }
            }
            index ++;
        }
        numberOfComputers += 1;
        if(numberOfComputers >= PhotonNetwork.CurrentRoom.PlayerCount - 1)
        {
            canRender = true;
        }
        index = 0;
    }

    public void AddRenderingComputer()
    {     
        if(!XRSettings.enabled)
        {
            rand = Random.Range(1,computers.Length);
            while(computers[rand].GetComponent<Rendering>().enabled)
            {
                rand = Random.Range(1,computers.Length);
            }
        }
        gameObject.GetComponent<PhotonView>().RPC("SyncTime", RpcTarget.AllBuffered, null);
        
    }
}
