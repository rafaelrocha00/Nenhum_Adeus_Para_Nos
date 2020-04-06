using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePopUpFollow : MonoBehaviour
{
    Transform attachedTransform;
    Camera mainCam;
    //Vector3 fixedDistance;

    private void Awake()
    {
        attachedTransform = transform;
        //fixedDistance = Vector3.zero;

        Debug.Log("Start");
    }

    private void Start()
    {
        mainCam = GameManager.gameManager.MainCamera.GetComponent<Camera>();
    }

    public void SetTransform(Transform atTransform)
    {
        attachedTransform = atTransform;
        //float xDis;
        //float yDis;
        //float zDis;
        //xDis = transform.position.x - attachedTransform.position.x;
        //yDis = transform.position.y - attachedTransform.position.y;
        //zDis = transform.position.z - attachedTransform.position.z;
        //fixedDistance = new Vector3(xDis, yDis, zDis);

        Debug.Log("Setando Transf");
    }

    private void Update()
    {
        transform.position = mainCam.WorldToScreenPoint(attachedTransform.position);// + fixedDistance;   
    }
}
