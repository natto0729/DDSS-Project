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
    public CharacterController controller;
    public Rigidbody rigidBody;
    public GameObject rig;
    public Animator animator;
    public PhotonRigidbodyView rigidView;
    public PhotonAnimatorView animView;

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

    private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		
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
        animView.enabled = false;
        foreach(Collider col in ragDollColliders)
        {
            col.enabled = true;
        }
        foreach(Rigidbody rigid in limbsRigidBodies)
        {
            rigid.isKinematic = false;
        }

        Physics.SyncTransforms();
        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rigidView.enabled = true;
        controller.enabled = false;
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
        animView.enabled = true;
        controller.enabled = true;
        player.enabled = true;
        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
        rigidBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rigidView.enabled = false;
    }

}
