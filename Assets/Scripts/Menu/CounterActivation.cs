using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.XR;

public class CounterActivation : MonoBehaviour
{
    public CharacterSelect characterSelect;

    [PunRPC]
    public void CounterUpdate(int timer)
    {
        gameObject.GetComponent<TextMeshProUGUI>().SetText("Game Starting In:" + timer.ToString());
    }

    [PunRPC]
    public void WaitingUpdate()
    {
        if(XRSettings.enabled || characterSelect.hasSelected)
        {
            gameObject.GetComponent<TextMeshProUGUI>().SetText("Waiting For Other Players...");
        }
    }
}
