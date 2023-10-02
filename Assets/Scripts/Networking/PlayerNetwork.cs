using UnityEngine;
using Photon.Pun;
using StarterAssets;

public class PlayerNetwork : MonoBehaviour
{

    PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            //Belongs to the player
            gameObject.transform.GetChild(5).gameObject.SetActive(true);
        }
        else
        {
            //Belongs to a different player
            GetComponent<ThirdPersonController>().enabled = false;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        
    }
}
