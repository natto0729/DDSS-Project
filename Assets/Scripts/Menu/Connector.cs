using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.XR;

public class Connector : MonoBehaviourPunCallbacks
{
    public int characterChosen;
    public GameObject counter;
    float timer;
    public float timeLimit;
    int timerInt;
    bool playersYes = false;
    bool VRConnected = true;
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
        SceneManager.LoadScene(5);
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
        base.OnPlayerEnteredRoom(newPlayer);
        if(XRSettings.enabled)
        {
            if(!room.CustomProperties["VRCheck"].Get<bool>())
            {
                Hashtable VRCheck = new Hashtable() {{ "VRCheck", VRConnected }};
                room.SetCustomProperties(VRCheck); 
            }
        }
        counter.GetComponent<PhotonView>().RPC("WaitingUpdate", RpcTarget.AllBuffered, null);
        timerStarted = false;
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log(PhotonNetwork.IsMasterClient);
        if(PhotonNetwork.CurrentRoom.PlayerCount >= 3 && room.CustomProperties["VRCheck"].Get<bool>() == true)
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

