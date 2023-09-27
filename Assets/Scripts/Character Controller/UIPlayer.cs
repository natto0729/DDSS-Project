using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIPlayer : MonoBehaviour
{
    public GameObject StaminaSlider;
    public GameObject BatterySlider;

    GameObject player;

    private void Update(){
        if(!player)
        {
            FindPlayer();
        }
        else if(player && player.GetComponent<PhotonView>().IsMine)
        {
            StaminaUI();
            BatteryUI();
        }
    }

    private void FindPlayer(){
        player = GameObject.Find("PlayerArmature(Clone)");
    }

    private void StaminaUI(){
        StaminaSlider.transform.GetChild(0).GetComponent<Image>().fillAmount = player.GetComponent<ThirdPersonController>().currentStamina / 100;
    }

        private void BatteryUI(){
        BatterySlider.transform.GetChild(1).GetComponent<Image>().fillAmount = player.GetComponent<ThirdPersonController>().currentBattery / 100;
    }

}
