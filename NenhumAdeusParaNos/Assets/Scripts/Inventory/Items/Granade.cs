using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Granade : MonoBehaviour
{
    Rigidbody rb;
    //public Transform origin;
    bool collided;

    public float areaOfEffect = 7.5f;
    public GameObject effect;

    public bool onPlayer = false;
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
        UnlockAll();
        StopCoroutine("ThrowCurve");
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
        StartCoroutine(ThrowCurve(force));
        //rb.AddForce(force, ForceMode.Impulse);
        //rb.
        //rb.velocity = force;
        //rb.AddForce(force * 25, ForceMode.Acceleration);
    }
    IEnumerator ThrowCurve(Vector3 dest)
    {
        Vector3 oriPos = transform.position;
        float timer = 0.0f;
        while (timer < 1)
        {
            transform.position = LauchTragectory.Hermite(oriPos, dest, timer);
            timer += Time.deltaTime * 1.5f;
            yield return new WaitForEndOfFrame();
        }
    }

    public void Lock()
    {
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        GetComponent<Collider>().enabled = false;
    }
    public void UnlockCol()
    {
        //rb.useGravity = true;
        //rb.constraints = RigidbodyConstraints.None;
        GetComponent<Collider>().enabled = true;
    }
    public void UnlockAll()
    {
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
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
