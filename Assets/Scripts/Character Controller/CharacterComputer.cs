using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CharacterComputer : MonoBehaviour
{

public bool isVR;


    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            GameObject.Find("Environment").transform.GetChild(4).GetComponent<Computers>().AddRenderingComputer(isVR);
        }
    }
}
