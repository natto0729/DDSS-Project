//12.09.2023 - v0.0
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class NetworkManagerLobby : MonoBehaviourPunCallbacks
{
    public string gameVersion;
    string networkStatus;

    public bool canSelect = false;

    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    void OnGUI()
    {
        //Shows the status of the network
        GUILayout.Label(networkStatus);
    }

    void Connect()
    {
        //Connect to Photon servers
        PhotonNetwork.ConnectUsingSettings();
        //Sets game version
        PhotonNetwork.GameVersion = gameVersion;
        networkStatus = "Connecting to Photon";
    }

    //Gets called once the player has connected to the master
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        networkStatus = "Connected to Master";
        PhotonNetwork.JoinLobby();
    }

    //Gets called once the player has joined a lobby
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        networkStatus = "Joined Lobby";
        //Either creates or joins a random room
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    //Gets called when the player has joined a room
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        networkStatus = "Room Joined";
        canSelect = true;
        if(XRSettings.enabled && PhotonNetwork.CurrentRoom.CustomProperties["VRCheck"].Get<bool>() == true)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("MainMenuExample"); 
        }
        if(PhotonNetwork.CurrentRoom.PlayerCount > 4 && !XRSettings.enabled)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Title Scene");
        }
    }
}
