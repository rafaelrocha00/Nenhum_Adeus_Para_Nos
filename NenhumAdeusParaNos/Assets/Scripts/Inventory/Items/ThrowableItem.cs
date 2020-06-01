using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Throwable Item", menuName = "Item/Throwable Item")]
public class ThrowableItem : QuickUseItem
{
    public GameObject itemToThrow;

    public override bool Effect()
    {
        //Começar a mirar
        MainCharacter.StartAiming(this);
        return false;
    }

    public void Throw(Vector3 force, Granade generatedG)
    {
        //Arremessar
        generatedG.ApplyForce(force);
        //generatedG.GetComponent<Rigidbody>().velocity *= 5;
    }
}
