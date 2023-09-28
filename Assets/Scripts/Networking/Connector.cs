using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
public class Connector : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_Text buttonText;

    public void OnConnectedToServer()
    {
        if (usernameInput.text != "")
        {
            PhotonNetwork.NickName = usernameInput.text;
            buttonText.text = "CONNECTING...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Player Lobby");
    }
}
