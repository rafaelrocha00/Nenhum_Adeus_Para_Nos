using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackingBullet : Bullet
{
    public float knockbackDistance = 2.5f;

    protected override void OnContact(Collider col)
    {
        bool aux = false;
        try
        {
            aux = col.GetComponent<BattleUnit>().ReceiveDamage(damage);
            col.GetComponent<BattleUnit>().Knockback(knockbackDistance);
        }
        catch { }

        if (!aux) Destroy(this.gameObject);
    }
}
