using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CharacterComputer : MonoBehaviour
{

PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if(photonView.IsMine)
        {
            GameObject.Find("Environment").transform.GetChild(4).GetComponent<Computers>().AddRenderingComputer();
        }
    }
}
