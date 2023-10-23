using System.Threading;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using TMPro;
using ExitGames.Client.Photon.StructWrapping;

public class Connector : MonoBehaviourPunCallbacks
{
    public CharacterSelect characterSelect;
    public int characterChosen;
    public GameObject timerText;
    public GameObject counter;
    float timer;
    public float timeLimit;
    int timerInt;
    bool playersYes = false;
    Room room;
    bool timerStarted = false;
    PhotonView photonViews;

    [PunRPC]
    private void ChosenPlayersNet()
    {
        characterChosen += 1;
    }

    [PunRPC]
    private void LoadMain()
    {
        SceneManager.LoadScene("Playground");
    }

    [PunRPC]
    public void StartCountdown()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timerStarted = true;
        }
        else
        {
            Debug.Log(room.CustomProperties["Time"].Get<float>());
            timer = room.CustomProperties["Time"].Get<float>();
            timerStarted = true;
        }
    }

    private void Start()
    {
        photonViews = GetComponent<PhotonView>();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(characterSelect.hasSelected)
        {
            counter.GetComponent<PhotonView>().RPC("WaitingUpdate", RpcTarget.All, null);
        }
        timerStarted = false;
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log(PhotonNetwork.IsMasterClient);
        if(PhotonNetwork.CurrentRoom.PlayerCount >= 3)
        {
            playersYes = true;
        }
    }

    private void Update()
    {
        if(room == null)
        {
            room = PhotonNetwork.CurrentRoom;
        }
        if (!timerStarted && playersYes && characterChosen >= PhotonNetwork.CurrentRoom.PlayerCount && PhotonNetwork.IsMasterClient)
        { 
            timer = timeLimit;
            Hashtable ht = new Hashtable() {{ "Time", timer }};
            room.SetCustomProperties(ht);          
            Debug.Log("Game Start");
            photonViews.RPC("StartCountdown", RpcTarget.All);
        }

        if (timerStarted == true)
        {
            UpdateTimer();
        }
    }

    void UpdateTimer()
    {
        timer -= Time.deltaTime;
        Hashtable ht = room.CustomProperties;
        ht.Remove("Time");
        ht.Add("Time", timer);
        room.SetCustomProperties(ht);
        timerInt = (int) timer;
        counter.GetComponent<PhotonView>().RPC("CounterUpdate", RpcTarget.All, timerInt);
        if (timer <= 0)
        {
            photonViews.RPC("LoadMain", RpcTarget.AllBuffered, null);
        }
    }

}

