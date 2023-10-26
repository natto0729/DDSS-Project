using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using StarterAssets;
using Oculus.Platform;

public class RagdollController : MonoBehaviour
{
    public ThirdPersonController player;
    public GameObject rig;
    public Animator animator;

    public bool mode = false;
    public bool canSwitch = true;

    Collider[] ragDollColliders;
    Rigidbody[] limbsRigidBodies;
    

    // Start is called before the first frame update
    void Start()
    {
        GetRagdollBits();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!mode && canSwitch)
        {
            canSwitch = false;
            RagdollModeOff();
        }
        else if(mode && !canSwitch)
        {
            canSwitch = true;
            RagdollModeOn();
        }
    }

    void GetRagdollBits()
    {
        ragDollColliders = rig.GetComponentsInChildren<Collider>();
        limbsRigidBodies = rig.GetComponentsInChildren<Rigidbody>();
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

        player.enabled = false;
    }

    [PunRPC]
    public void RagdollModeOff()
    {      

        foreach(Collider col in ragDollColliders)
        {
            col.enabled = false;
        }
        foreach(Rigidbody rigid in limbsRigidBodies)
        {
            rigid.isKinematic = true;
        }


        animator.enabled = true;
        player.enabled = true;
    }

}
