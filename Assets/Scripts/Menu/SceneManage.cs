using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class SceneManage : MonoBehaviour
{
    public CharacterSelect characterSelect;
    

    public void Start()
    {
        if(XRSettings.enabled)
        {
            GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.GetChild(2).gameObject.SetActive(false);
        }
    }
    public void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void CharacterSelect()
    {
        
        SceneManager.LoadScene("Character");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Title Scene");
    }

    public void Lobby()
    {
        characterSelect.StartGame();
    }

    public void WinGame()
    {
        SceneManager.LoadScene("Winning Scene");
    }

}
