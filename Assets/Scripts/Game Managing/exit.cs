using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class exit : MonoBehaviour
{
    public GameManager gameManager;

    public void EndOfGame()
    {
        if (gameManager.renderTotal >= 100)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Winning Scene");
        }  
        else if(gameManager.renderTotal < 100)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("GameOver");
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
 
    }
}
