using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using TMPro;

public class Connector : MonoBehaviourPunCallbacks
{
    public int characterChosen;
    float timeLimit;
    public float timer;
    int timerInt;
    bool playersYes = false;
    bool canCount = false;
    public GameObject timerText;
    Room room;
    bool timerStarted;
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

    private void Start()
    {
        photonViews = GetComponent<PhotonView>();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
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
        if(PhotonNetwork.IsMasterClient && room == null)
        {
            room = PhotonNetwork.CurrentRoom;
        }
        if (playersYes && characterChosen >= 3 && PhotonNetwork.IsMasterClient)
        {           
            Debug.Log("Game Start");
            photonViews.RPC("StartCountdown", RpcTarget.All);
            canCount = true;
        }

        if (timerStarted == true)
        {
            UpdateTimer();
        }

        timerInt = (int) timer;

        if(canCount == true)
        {
            timerText.GetComponent<TextMeshProUGUI>().text = "Game Starting In:" + timerInt.ToString();
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
            photonViews.RPC("LoadMain", RpcTarget.AllBuffered, null);
        }
    }

}

