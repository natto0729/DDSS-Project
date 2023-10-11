using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class Connector : MonoBehaviourPunCallbacks
{
    float timeLimit;
    float timer;
    GameObject timerText;
    Room room;
    bool timerStarted;

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 3 && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Game Start");
            photonView.RPC("StartCountdown", RpcTarget.All);
        }


    }

    private void Update()
    {
        if (timerStarted == true)
        {
            UpdateTimer();
        }
    }

    [PunRPC]
    public void StartCountdown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timer = timeLimit;
            Hashtable ht = new Hashtable() { { "Time", timer } };
            room.SetCustomProperties(ht);
            timerStarted = true;
        }
        else
        {
            timer = (float)room.CustomProperties["Time"];
            timerStarted = true;
        }
    }

    void UpdateTimer()
    {
        timer -= Time.deltaTime;
        Hashtable ht = room.CustomProperties;
        ht.Remove("Time");
        ht.Add("Time", timer);
        room.SetCustomProperties(ht);
        if (timer <= 0)
        {
            SceneManager.LoadScene("");
        }
    }

}

