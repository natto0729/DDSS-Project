using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class VRRig : MonoBehaviourPunCallbacks
{
    public float turnSmoothness;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Transform mainC;
    public Transform headConstraint;
    private Vector3 headBodyOffset;

    [Header("Interaction Actions")]
    public TextMeshPro useText;
    public float maxUseDistance = 1f;
    public LayerMask useLayers;
    bool hasKey = true;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        headBodyOffset = transform.position - headConstraint.position;
    }

    void Update()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount <= 1 && gameManager.renderTotal <= 100 && XRSettings.enabled)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(8);
        }
        if (gameManager.renderTotal >= 100 && XRSettings.enabled)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(7);
        }
        Prompt();
        if(OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            OnInteractDoor();
            OnInteractComputer();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 eulerRotation = new Vector3(transform.eulerAngles.x, head.rigTarget.transform.eulerAngles.y, transform.eulerAngles.z);
        transform.rotation = Quaternion.Euler(eulerRotation);
        transform.position = headConstraint.position + headBodyOffset;
        transform.forward = Vector3.Lerp(transform.forward,
            Vector3.ProjectOnPlane(mainC.forward, Vector3.up).normalized, Time.deltaTime * turnSmoothness);

        head.Map();
        leftHand.Map();
        rightHand.Map();
    }

    private void Prompt()
    {
        if(Physics.Raycast(mainC.transform.position, mainC.transform.forward, out RaycastHit hit, maxUseDistance, useLayers) && hit.collider.TryGetComponent<Door>(out Door door) && !door.needsKey && !door.isFinalDoor)
        {
            if(door.isOpen)
            {
                useText.SetText("Close \"Right Trigger\"");
            }
            else
            {
                useText.SetText("Open \"Right Trigger\"");
            }
            useText.gameObject.SetActive(true);
            useText.transform.position = hit.point - (hit.point - mainC.transform.position).normalized * 1f;
            useText.transform.rotation = Quaternion.LookRotation((hit.point - mainC.transform.position).normalized);
        }
        else if(Physics.Raycast(mainC.transform.position, mainC.transform.forward, out hit, maxUseDistance, useLayers) && hit.collider.TryGetComponent<Door>(out door) && door.needsKey && !door.isFinalDoor)
        {
            if(!hasKey)
            {
                useText.SetText("Access Card Needed");
            }
            else if(door.isOpen)
            {
                useText.SetText("Close \"Right Trigger\"");
            }
            else
            {
                useText.SetText("Open \"Right Trigger\"");
            }
            useText.gameObject.SetActive(true);
            useText.transform.position = hit.point - (hit.point - mainC.transform.position).normalized * 1f;
            useText.transform.rotation = Quaternion.LookRotation((hit.point - mainC.transform.position).normalized);
        }
        else if(Physics.Raycast(mainC.transform.position, mainC.transform.forward, out hit, maxUseDistance, useLayers) && hit.collider.TryGetComponent<Rendering>(out Rendering computer) && computer.enabled && computer.isRendering)
        {
            useText.SetText("Stop Rendering \"Right Trigger\"");
            useText.gameObject.SetActive(true);
            useText.transform.position = hit.point - (hit.point - mainC.transform.position).normalized * 1f;
            useText.transform.rotation = Quaternion.LookRotation((hit.point - mainC.transform.position).normalized);
        }
        else
        {
            useText.gameObject.SetActive(false);
        }
    }

    public void OnInteractDoor()
    {
        if(Physics.Raycast(mainC.transform.position, mainC.transform.forward, out RaycastHit hit, maxUseDistance, useLayers))
        {
            if(hit.collider.TryGetComponent<Door>(out Door door) && door.needsKey && !door.isFinalDoor)
            {
                if(door.isOpen && hasKey)
                {
                    door.GetComponent<PhotonView>().RPC ("CloseNetwork",RpcTarget.AllBuffered, null);
                }
                else if(!door.isOpen && hasKey)
                {
                    door.GetComponent<PhotonView>().RPC ("OpenNetwork",RpcTarget.AllBuffered, transform.position);
                }
            }
            else if(hit.collider.TryGetComponent<Door>(out door) && !door.needsKey && !door.isFinalDoor)
            {
                if(door.isOpen)
                {
                    door.GetComponent<PhotonView>().RPC ("CloseNetwork",RpcTarget.AllBuffered, null);
                }
                else
                {
                    door.GetComponent<PhotonView>().RPC ("OpenNetwork",RpcTarget.AllBuffered, transform.position);
                }
            }
        }
    }

    public void OnInteractComputer()
    {
        if(Physics.Raycast(mainC.transform.position, mainC.transform.forward, out RaycastHit hit, maxUseDistance, useLayers))
        {
            if(hit.collider.TryGetComponent<Rendering>(out Rendering computer) && computer.enabled &&  computer.isRendering)
            {
                computer.GetComponent<PhotonView>().RPC ("StopProgress",RpcTarget.AllBuffered, null);
            }
        }
    }
}


