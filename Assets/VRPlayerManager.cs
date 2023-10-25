using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPlayerManager : MonoBehaviour
{

public List<GameObject> VRObjects = new List<GameObject>();
    //public List<OVRGrabbable> grabObjs = new List<OVRGrabbable>();


    // Start is called before the first frame update
    void Awake()
    {
        foreach(GameObject obj in VRObjects)
        {
            obj.GetComponent<OVRGrabbable>().enabled = true;
            obj.GetComponent<Rigidbody>().isKinematic = false;
        }       
    }

}
