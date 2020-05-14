using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanyPC : Interactives
{
    public Transform camT;
    //bool interacted = false;
    CamMove cam;
    public GameObject pcScreen;
    public GameObject pcLight;

    public override void Interact(Player player)
    {
        DesactiveBtp();
        //interacted = true;
        if (cam == null) cam = GameManager.gameManager.MainCamera;

        cam.TargetingPlayer = false;
        cam.LerpLoc(camT.position, 1);
        cam.LerpRot(camT.rotation, 1f);
        pcScreen.SetActive(true);
        pcLight.SetActive(true);
    }

    //public override void OnExit()
    //{
    //    //base.OnExit();
    //    //if (interacted)
    //    //{
    //    //    cam.TargetingPlayer = true;
    //    //    cam.LerpRot(cam.DefaultRotation, 1);
    //    //}
    //    //interacted = false;
    //}

    public void ExitPC()
    {
        base.OnExit();
        cam.TargetingPlayer = true;
        cam.LerpRot(cam.DefaultRotation, 1.0f);
        pcScreen.SetActive(false);
        pcLight.SetActive(false);
    }
}
