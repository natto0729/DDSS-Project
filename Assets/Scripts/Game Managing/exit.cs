using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class exit : MonoBehaviour
{
    public GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

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
