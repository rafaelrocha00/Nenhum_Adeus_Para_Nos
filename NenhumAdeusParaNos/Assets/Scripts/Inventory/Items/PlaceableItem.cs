using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Placeable Item", menuName = "Item/Placeable Item")]
public class PlaceableItem : QuickUseItem
{
    public GameObject itemToPlace;
    public GameObject itemToPlaceMeshonly;

    public override bool Effect()
    {
        MainCharacter.StartPlaceItem(this);
        return false;
    }

    public void Place(Vector3 pos, Quaternion rot)
    {
        GameObject aux = Instantiate(itemToPlace, pos, rot) as GameObject;
        //Destroy(aux, 5);
    }
}
