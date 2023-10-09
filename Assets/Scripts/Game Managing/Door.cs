using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Door : MonoBehaviour
{
    public GameObject parent;
    public bool needsKey;
    public bool isFinalDoor;
    public bool isOpen = false;
    public bool isRotatingDoor = true;
    public float speed = 1f;
    public float rotationAmount = 90f;

    public float forwardDirection = 0;

    private Vector3 StartRotation;
    private Vector3 forward;

    private Coroutine animationCoroutine;

    [PunRPC]
    public void OpenNetwork(Vector3 userNetworkPosition)
    {
        Open(userNetworkPosition);
    }

    [PunRPC]
    public void CloseNetwork()
    {
        Close();
    }

    private void Awake()
    {
        StartRotation = parent.transform.rotation.eulerAngles;

        forward = parent.transform.right;
    }

    public void Open(Vector3 userPosition)
    {
        if(!isOpen)
        {
            if(animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            if(isRotatingDoor)
            {
                float dot = Vector3.Dot(forward, (userPosition - parent.transform.position).normalized);
                Debug.Log($"Dot: {dot.ToString("N3")}");
                animationCoroutine = StartCoroutine(DoRotationOpen(dot));
            }
        }
    }

    private IEnumerator DoRotationOpen(float forwardAmount)
    {
        Quaternion startRotation = parent.transform.rotation;
        Quaternion endRotation;

        if (forwardAmount >= forwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y - rotationAmount, 0));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y + rotationAmount, 0));
        }

        isOpen = true;

        float time = 0;
        while(time < 1)
        {
            parent.transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }

    public void Close()
    {
        if(isOpen)
        {
            if(animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            if(isRotatingDoor)
            {
                animationCoroutine = StartCoroutine(DoRotationClose());
            }
        }
    }

    private IEnumerator DoRotationClose()
    {
        Quaternion startRotation = parent.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(StartRotation);

        isOpen = false;

        float time = 0;
        while (time < 1)
        {
            parent.transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }
    }
    
    
}
