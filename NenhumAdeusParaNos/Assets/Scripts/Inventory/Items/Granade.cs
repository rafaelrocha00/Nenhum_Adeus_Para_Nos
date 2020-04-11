using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Granade : MonoBehaviour
{
    Rigidbody rb;
    //public Transform origin;
    bool collided;

    public float areaOfEffect = 5.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.J)) ApplyForce((origin.up + origin.forward) * 20);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collided)
        {
            OnLand();
            collided = true;
        }
    }

    protected abstract void OnLand();

    public void ApplyForce(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Impulse);
    }

    public void Lock()
    {
        rb.useGravity = false;
    }
    public void Unlock()
    {
        rb.useGravity = true;
    }
}
