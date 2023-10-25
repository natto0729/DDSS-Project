using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Computers : MonoBehaviour
{
    public int index = 0;
    private int extraIndex = 0;

    public int numberOfComputers = 0;

    public bool canRender = true;

    public Rendering[] computers;

    private int rand;

    private int saved;

    void Start()
    {
        computers = gameObject.GetComponentsInChildren<Rendering>();
    }

    [PunRPC]
    public void ActivateComputer(int rand)
    {
        saved = rand;
        canRender = false;   
        computers[saved].GetComponent<Rendering>().enabled = true;
        computers[saved].transform.GetChild(0).gameObject.SetActive(true);
        computers[saved].transform.GetChild(1).gameObject.SetActive(true);
        numberOfComputers += 1;
        if(numberOfComputers >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            canRender = true;
        }
    }

    public void AddRenderingComputer()
    {     
        rand = Random.Range(1,computers.Length);
        while(computers[rand].GetComponent<Rendering>().enabled)
        {
            rand = Random.Range(1,computers.Length);
        }
        gameObject.GetComponent<PhotonView>().RPC("ActivateComputer", RpcTarget.AllBuffered, rand);
    }
}
