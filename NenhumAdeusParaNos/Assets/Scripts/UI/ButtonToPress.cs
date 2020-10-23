using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonToPress : MonoBehaviour
{
    Camera main;

    public SpriteAnimator sa;

    private void Start()
    {
        main = Camera.main;
        //transform.rotation = Quaternion.LookRotation(main.transform.forward + Vector3.up);
    }

    private void OnEnable()
    {
        sa.Play(true);
    }

    private void OnDisable()
    {
        sa.Stop();
    }

    private void FixedUpdate()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time) * 0.01f, transform.position.z);
        transform.rotation = Quaternion.LookRotation(/*main.transform.forward + Vector3.up*/Vector3.Cross(main.transform.forward, main.transform.right));
    }
}
