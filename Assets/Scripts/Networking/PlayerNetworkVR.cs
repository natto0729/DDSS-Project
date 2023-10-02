using UnityEngine;
using Photon.Pun;
using StarterAssets;

public class PlayerNetworkVR : MonoBehaviour
{

    PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            //Belongs to the player
            gameObject.transform.GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            //Belongs to a different player
            GetComponent<OVRPlayerController>().enabled = false;
        }
        
    }
}
