using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class CounterActivation : MonoBehaviour
{
    [PunRPC]
    public void CounterUpdate(int timer)
    {
        gameObject.GetComponent<TextMeshProUGUI>().SetText("Game Starting In:" + timer.ToString());
    }

    [PunRPC]
    public void WaitingUpdate()
    {
        gameObject.GetComponent<TextMeshProUGUI>().SetText("Waiting For Other Players...");
    }
}
