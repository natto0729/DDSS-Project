using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System;

public class CharacterSelect : MonoBehaviour
{
    public List<GameObject> characters = new List<GameObject>();
    List<String> chosenCharacters = new List<String>();
    public int selectedCharacter = 0;

    public GameObject timerText;

    public GameObject startButton;
    public GameObject leftButton;
    public GameObject rightButton;

    PhotonView photonView;

    [PunRPC]
    public void AddToListNet(String add)
    {
        AddToList(add);
    }

    public void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void NextCharacter()
    {
        timerText.GetComponent<TextMeshProUGUI>().text = null;
        characters[selectedCharacter].SetActive(false);
        selectedCharacter = (selectedCharacter + 1) % characters.Count;
        characters[selectedCharacter].SetActive(true);
    }
    public void PreviousCharacter()
    {
        timerText.GetComponent<TextMeshProUGUI>().text = null;
        characters[selectedCharacter].SetActive(false);
        selectedCharacter--;
        if (selectedCharacter < 0)
        {
            selectedCharacter += characters.Count;
        }
        characters[selectedCharacter].SetActive(true);
    }

    public void StartGame()
    {
        if(GameObject.Find("Game Manager").GetComponent<NetworkManagerLobby>().canSelect)
        {
            if(chosenCharacters.Contains(characters[selectedCharacter].name))
            {
                timerText.GetComponent<TextMeshProUGUI>().text = "Character Already Chosen, Next To Try Again";
            }
            else
            {
                PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
                photonView.RPC ("AddToListNet",RpcTarget.AllBuffered, characters[selectedCharacter].name);
                startButton.SetActive(false);
                leftButton.SetActive(false);
                rightButton.SetActive(false);
                GameObject.Find("Game Manager").GetComponent<PhotonView>().RPC ("ChosenPlayersNet", RpcTarget.AllBuffered, null);
                timerText.GetComponent<TextMeshProUGUI>().text = "Waiting For Other Players...";
            }
        }
    }

    public void AddToList(String added)
    {
        chosenCharacters.Add(added);
    }
}
