using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonToPress : MonoBehaviour
{

    private void Start()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward + Vector3.up);
    }


    private void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time) * 0.01f, transform.position.z);
    }
}
