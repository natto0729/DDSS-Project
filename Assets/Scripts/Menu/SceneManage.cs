using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    public CharacterSelect characterSelect;
    public void Mode()
    {
        SceneManager.LoadScene("Mode");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void CharacterSelect()    {
        SceneManager.LoadScene("Character");
    }

    public void MainMenu(){
        SceneManager.LoadScene("Title Scene");
    }

    public void Lobby()
    {
        characterSelect.StartGame();
    }
}
