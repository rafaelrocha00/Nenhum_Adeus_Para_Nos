using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public Vector3 distanceToPlayer = new Vector3(-12, 21, -12);
    public float cameraSmooth = 1.2f;
    public Transform pPosition;

    //public void Move(Vector3 pLocation)
    //{
    //    transform.position = Vector3.Slerp(transform.position, pLocation + distanceToPlayer, Time.deltaTime);
    //}

    private void Update()
    {
        transform.position = Vector3.Slerp(transform.position, pPosition.position + distanceToPlayer, Time.deltaTime * cameraSmooth);
    }
}
