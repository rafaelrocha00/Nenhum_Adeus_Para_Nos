using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    /*[HideInInspector]*/Vector3 distanceToTarget;
    //public Vector3 DistanceToTaget { set { distanceToTarget = value; } }

    [SerializeField] Vector3 defaultDistance = new Vector3(-14, 21, 0);
    public Vector3 DefaultDistance { set { defaultDistance = value; } }
    public Vector3 distanceInBattle = new Vector3(-16, 26, 0);

    public float cameraSmooth = 1.2f;

    Vector3 targetPos;
    public Transform playerTransform;
    bool targetingPlayer = true;

    //float centralizedTime;

    //public void Move(Vector3 pLocation)
    //{
    //    transform.position = Vector3.Slerp(transform.position, pLocation + distanceToPlayer, Time.deltaTime);
    //}

    private void Start()
    {
        distanceToTarget = defaultDistance;

        //centralizedTime = GameManager.gameManager.battleController.WaitTime;
    }

    private void LateUpdate()
    {
        if (targetingPlayer) transform.position = Vector3.Slerp(transform.position, playerTransform.position + distanceToTarget, Time.deltaTime * cameraSmooth);
        else transform.position = Vector3.Slerp(transform.position, targetPos + distanceToTarget, Time.deltaTime * cameraSmooth);

        float xDistance = Mathf.Abs(transform.position.x - playerTransform.position.x);
        if (xDistance < 11.5) SetToWalkDown();
        else if (xDistance > 17) SetDefaultDistance();
    }

    public void StartBattle(Vector3 newTarget)
    {
        targetingPlayer = false;
        targetPos = newTarget;
        distanceToTarget = distanceInBattle;
        //StartCoroutine("ResetTarget");
    }

    public void SetTarget(Vector3 newTarget)
    {
        targetPos = newTarget;
    }

    public void EndBattle()
    {
        SetDefaultDistance();
        targetingPlayer = true;
    }

    public void SetToWalkDown()
    {
        distanceToTarget.x = defaultDistance.x * 1.5f;
    }
    public void SetDefaultDistance()
    {
        distanceToTarget = defaultDistance;
    }
    //IEnumerator ResetTarget()
    //{
    //    yield return new WaitForSeconds(centralizedTime);        
    //}
}
