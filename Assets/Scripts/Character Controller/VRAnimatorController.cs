using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAnimatorController : MonoBehaviour
{
    public float speedThreshold;
    private Animator animator;
    private Vector3 previousPos;
    private VRRig vrRig;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        vrRig = GetComponent<VRRig>();
        previousPos = vrRig.head.vrTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 headSpeed = (vrRig.head.vrTarget.position - previousPos) / Time.deltaTime;
        headSpeed.y = 0;

        Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headSpeed);
        previousPos = vrRig.head.vrTarget.position;

        animator.SetBool("isMoving", headsetLocalSpeed.magnitude > speedThreshold);
        animator.SetFloat("Speed", headsetLocalSpeed.magnitude);
    }
}
