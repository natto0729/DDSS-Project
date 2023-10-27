using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using StarterAssets;

public class RagdollController : MonoBehaviour
{
    public ThirdPersonController player;
    public CharacterController controller;
    public Rigidbody hips;
    public GameObject rig;
    public Animator animator;
    public GameObject cameraRoot;

    public bool mode = false;
    public bool canSwitch = true;
    public bool isGrabbed = false;

    Collider[] ragDollColliders;
    Rigidbody[] limbsRigidBodies;
    

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<PhotonView>().RPC("GetRagdollBits", RpcTarget.AllBuffered, null);
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.enabled == false)
        {
            gameObject.GetComponent<PhotonView>().RPC("RagDollMove", RpcTarget.AllBuffered, null);
        }
    }

    [PunRPC]
    public void GetRagdollBits()
    {
        ragDollColliders = rig.GetComponentsInChildren<Collider>();
        limbsRigidBodies = rig.GetComponentsInChildren<Rigidbody>();
    }

    [PunRPC]
    public void RagDollMove()
    {
        if(isGrabbed)
        {
            foreach (Rigidbody rigid in limbsRigidBodies)
            {
                rigid.isKinematic = true;
                rigid.useGravity = false;
            }
        }
        hips.MovePosition(cameraRoot.transform.position);
        hips.MoveRotation(cameraRoot.transform.rotation);
    }

    [PunRPC]
    public void RagdollModeOn()
    {
        animator.enabled = false;
        foreach(Collider col in ragDollColliders)
        {
            col.enabled = true;
        }
        foreach(Rigidbody rigid in limbsRigidBodies)
        {
            rigid.isKinematic = false;
        }

        controller.enabled = false;
        player.enabled = false;
    }

    [PunRPC]
    public void RagdollModeOff()
    {

        foreach (Collider col in ragDollColliders)
        {
            col.enabled = false;
        }
        foreach(Rigidbody rigid in limbsRigidBodies)
        {
            rigid.isKinematic = true;
        }


        animator.enabled = true;
        controller.enabled = true;
        player.enabled = true;
    }

}
