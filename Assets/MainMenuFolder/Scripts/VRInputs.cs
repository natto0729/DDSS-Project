using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRInputs : MonoBehaviour
{
    public static Controls controls;

    public VRController rightController;
    public VRController leftController;


    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.VR.LeftTrigger.performed += LeftPressed;
        controls.VR.RightTrigger.performed += RightPressed;

        controls.Enable();
    }

    private void LeftPressed(InputAction.CallbackContext obj)
    {
        leftController.TriggerPressed();
    }

    void RightPressed(InputAction.CallbackContext obj)
    {
        rightController.TriggerPressed();
    }

}
