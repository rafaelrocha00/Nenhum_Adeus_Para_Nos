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
    [HideInInspector] bool targetingPlayer = true;
    public bool TargetingPlayer { set { targetingPlayer = value; } }
    bool inBattle = false;

    [HideInInspector] Quaternion defaultRotation;
    public Quaternion DefaultRotation { get { return defaultRotation; } }

    //float timeInTopBorders = 0.0f;
    //float timeInRigBorders = 0.0f;
    //float centralizedTime;

    //public void Move(Vector3 pLocation)
    //{
    //    transform.position = Vector3.Slerp(transform.position, pLocation + distanceToPlayer, Time.deltaTime);
    //}

    private void Start()
    {
        distanceToTarget = defaultDistance;
        defaultRotation = transform.rotation;

        //centralizedTime = GameManager.gameManager.battleController.WaitTime;
    }

    private void LateUpdate()
    {
        if (targetingPlayer) transform.position = Vector3.Slerp(transform.position, playerTransform.position + distanceToTarget, Time.deltaTime * cameraSmooth);
        else if (inBattle) transform.position = Vector3.Slerp(transform.position, targetPos + distanceToTarget, Time.deltaTime * cameraSmooth);

        if (targetingPlayer || inBattle)
        {
            float xDistance = Mathf.Abs(transform.position.x - playerTransform.position.x);
            //if (xDistance < 15.5) SetToWalkDown();
            //else if (xDistance > 26.5) SetDefaultDistance();
        }

        CameraMouseControl();
    }

    public void StartBattle(Vector3 newTarget)
    {
        targetingPlayer = false;
        inBattle = true;
        targetPos = newTarget;
        distanceToTarget = distanceInBattle;
        //StartCoroutine("ResetTarget");
    }

    public void SetTarget(Vector3 newTarget)
    {
        //Debug.Log(newTarget);
        targetPos = newTarget;
    }

    public void EndBattle()
    {
        SetDefaultDistance();
        targetingPlayer = true;
        inBattle = false;
    }

    public void CameraMouseControl()
    {
        if (Input.mousePosition.x >= Screen.width * 0.9f)
        {
            //timeInTopBorders += Time.deltaTime;
            //if (timeInTopBorders >= 1.0f) 
            SetCameraToRight(true);
        }
        else if (Input.mousePosition.x <= Screen.width * 0.1f)
        {
            //timeInTopBorders += Time.deltaTime;
            //if (timeInTopBorders >= 1.0f) 
            SetCameraToRight(false);
        }
        else SetDefaultZDist();

        if (Input.mousePosition.y >= Screen.height * 0.9f)
        {
            //timeInRigBorders += Time.deltaTime;
            //if (timeInRigBorders >= 1.0f) 
            SetCameraToUp(true);
        }
        else if (Input.mousePosition.y <= Screen.height * 0.1f)
        {
            //timeInRigBorders += Time.deltaTime;
            //if (timeInRigBorders >= 1.0f) 
            SetCameraToUp(false);
        }
        else SetDefaultXDist();
    }

    public void SetCameraToUp(bool v)
    {
        //if (!GameManager.gameManager.battleController.ActiveBattle) distanceToTarget.x = defaultDistance.x * 1.6f;
        //else distanceToTarget.x = distanceInBattle.x * 1.6f;
        if (v) distanceToTarget.x = defaultDistance.x *  0.7f;
        else   distanceToTarget.x = defaultDistance.x *  1.4f;
    }
    public void SetCameraToRight(bool v)
    {
        if (v) distanceToTarget.z = defaultDistance.z - 5f;
        else   distanceToTarget.z = defaultDistance.z + 5f;
    }

    public void SetDefaultDistance()
    {
        if (!GameManager.gameManager.battleController.ActiveBattle) distanceToTarget = defaultDistance;
        else distanceToTarget = distanceInBattle;
    }
    void SetDefaultXDist()
    {
        if (!GameManager.gameManager.battleController.ActiveBattle) distanceToTarget.x = defaultDistance.x;
        else distanceToTarget.x = distanceInBattle.x;

        //timeInRigBorders = 0.0f;
    }
    void SetDefaultZDist()
    {
        if (!GameManager.gameManager.battleController.ActiveBattle) distanceToTarget.z = defaultDistance.z;
        else distanceToTarget.z = distanceInBattle.z;

        //timeInTopBorders = 0.0f;
    }

    public void LerpLoc(Vector3 loc, float tMult)
    {
        StartCoroutine(LerpLocation(loc, tMult));
    }
    public void LerpRot(Quaternion rot, float tMult)
    {
        StartCoroutine(LerpRotation(rot, tMult));
    }

    IEnumerator LerpLocation(Vector3 loc, float tMult)
    {
        Vector3 oriPos = transform.position;
        float t = 0.0f;

        while (!transform.position.Equals(loc))
        {
            transform.position = Vector3.Lerp(oriPos, loc, t);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime * tMult;
        }
    }

    IEnumerator LerpRotation(Quaternion rot, float tMult)
    {
        Quaternion oriRot = transform.rotation;
        float t = 0.0f;

        while (t < 1)
        {
            transform.rotation = Quaternion.Lerp(oriRot, rot, t);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime * tMult;
        }
    }
    //IEnumerator ResetTarget()
    //{
    //    yield return new WaitForSeconds(centralizedTime);        
    //}
}
