using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveProjectile : Bullet
{
    public float explosionRadius = 5.0f;

    protected override void OnContact(Collider col)
    {
        Debug.Log("Colliding");
        Explode();        
    }

    protected override void OnLifeTimeOver()
    {
        Debug.Log("Destroyng bullet by lifetime");
        Explode();
    }

    void Explode()
    {
        Debug.Log("Exploding");
        Collider[] cols = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var col in cols)
        {
            Debug.Log(col.name);
            //if (!col.gameObject.layer.Equals(this.gameObject.layer))
            //{

            try
            {
                BattleUnit bu = col.GetComponent<BattleUnit>();
                bu.ReceiveDamage(damage);
            }
            catch { Debug.Log("could not explode"); }
            //}
        }

        Destroy(this.gameObject);
    }
}
