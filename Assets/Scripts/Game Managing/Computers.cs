using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR;

public class Computers : MonoBehaviour
{
    public bool canRender = true;

    public Rendering[] computers;

    private int rand;
    private int saved;

    private int index = 0;
    public bool isXR = false;

    PhotonView photonViews;

    void Start()
    {
        if(XRSettings.enabled)
        {
            isXR = true;
        }
        photonViews = gameObject.GetComponent<PhotonView>();
        computers = gameObject.GetComponentsInChildren<Rendering>();
    }

    [PunRPC]
    public void SyncTime(int number, bool XRCheck, bool oppar, bool nerd)
    {
        Debug.Log(XRCheck);
        if(!XRCheck)
        {
            if(oppar)
            {
                computers[number].GetComponent<Rendering>().isOppar = true;
            }
            if(nerd)
            {
                computers[number].GetComponent<Rendering>().isNerd = true;
            }
            computers[number].GetComponent<Rendering>().enabled = true;
            computers[number].transform.GetChild(0).gameObject.SetActive(true);
            computers[number].transform.GetChild(1).gameObject.SetActive(true);
            computers[number].transform.GetChild(2).GetChild(3).GetComponent<VideoPlayer>().enabled = true;
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
        canRender = true;
        index = 0;
        if(XRCheck)
        {
            isXR = false;
        }   
    }

    public void AddRenderingComputer(bool isVR, bool isOppar, bool isNerd)
    {    
        rand = Random.Range(1,computers.Length);
        Debug.Log(rand);
        while(computers[rand].GetComponent<Rendering>().enabled)
        {
            rand = Random.Range(1,computers.Length);
            Debug.Log(rand);
        }
        saved = rand;
        if(isVR)
        {
            photonViews.RPC("SyncTime", RpcTarget.AllBuffered, saved, isVR, false, false); 
        }
        else
        {
            photonViews.RPC("SyncTime", RpcTarget.AllBuffered, saved, isVR, isOppar, isNerd); 
        }
    }
}
