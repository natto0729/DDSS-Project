using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.XR;
using TMPro;

public class UIPlayer : MonoBehaviour
{
    public GameObject StaminaSlider;
    public GameObject BatterySlider;
    public GameObject RenderPercentage;
    public GameManager gameManager;
    public GameObject RenderStatus;

    GameObject player;

    private void Update()
    {
        if(!XRSettings.enabled)
        {
            if(!player)
            {
                FindPlayer();
            }

            else if(player && player.GetComponent<PhotonView>().IsMine)
            {
                StaminaUI();
                BatteryUI();
                RenderingUI();
            }
        }
        else if(XRSettings.enabled)
        {
            gameObject.GetComponent<Canvas>().enabled = false;
        }
    }

    private void FindPlayer()
    {
        player = GameObject.Find("PlayerArmature(Clone)");
    }

    private void StaminaUI()
    {
        StaminaSlider.transform.GetChild(0).GetComponent<Image>().fillAmount = player.GetComponent<ThirdPersonController>().currentStamina / 100;
    }

    private void BatteryUI()
    {
        BatterySlider.transform.GetChild(1).GetComponent<Image>().fillAmount = player.GetComponent<ThirdPersonController>().currentBattery / 100;
    }

    private void RenderingUI()
    {
        RenderPercentage.GetComponent<TextMeshProUGUI>().text = gameManager.renderTotal.ToString() + "%";
        if(gameManager.renderTotal == 100)
        {
            RenderStatus.GetComponent<TextMeshProUGUI>().text = "Rendering Completed";
        }
        else
        {
            RenderStatus.GetComponent<TextMeshProUGUI>().text = "Rendering In Progres";
        }

    }

}
