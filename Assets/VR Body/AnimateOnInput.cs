using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateOnInput : MonoBehaviour
{
    public Animator animator;

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Left Pinch", OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger));
        animator.SetFloat("Left Grab", OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger));
        animator.SetFloat("Right Pinch", OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger));
        animator.SetFloat("Right Grab", OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger));
    }
}
