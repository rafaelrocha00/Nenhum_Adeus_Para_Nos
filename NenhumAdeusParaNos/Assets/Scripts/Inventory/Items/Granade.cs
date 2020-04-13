using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Granade : MonoBehaviour
{
    Rigidbody rb;
    //public Transform origin;
    bool collided;

    public float areaOfEffect = 5.0f;
    //LayerMask layerMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    //private void Start()
    //{
    //    layerMask = LayerMask.GetMask("Interactives");
    //}

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
    protected abstract void GranadeEffect();

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

    //protected INPC[] GetEnemies()
    //{
    //    Collider[] colliders = Physics.OverlapSphere(transform.position, areaOfEffect, layerMask);
    //    List<INPC> enemies = new List<INPC>();
    //    for (int i = 0; i < colliders.Length; i++)
    //    {
    //        try
    //        {
    //            if (!colliders[i].isTrigger)
    //            {
    //                enemies.Add(colliders[i].GetComponent<INPC>());
    //            }
    //        }
    //        catch { Debug.Log("Não é NPC"); }
    //    }
    //    return enemies.ToArray();
    //}
}
