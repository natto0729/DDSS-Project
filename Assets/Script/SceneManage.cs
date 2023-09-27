using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManage : MonoBehaviour
{
    public void Mode()
    {
        SceneManager.LoadScene("Mode");
    }

    public void Exit()
    {

    }

    public void CharacterSelect()    {
        SceneManager.LoadScene("Character");
    }

    public void MainMenu(){
        SceneManager.LoadScene("Title Scene");
    }
}
