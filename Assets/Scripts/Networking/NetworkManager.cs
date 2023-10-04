//12.09.2023 - v0.0

using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;
using Oculus.Interaction;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    public string gameVersion;
    string networkStatus;

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
        if(!XRSettings.enabled)
        {
            PhotonNetwork.Instantiate("PlayerArmature", Vector3.zero, Quaternion.identity);

        }
        else if(XRSettings.enabled)
        {
            PhotonNetwork.Instantiate("OVRPlayerController Variant", Vector3.zero, Quaternion.identity);
        }
    }
}
