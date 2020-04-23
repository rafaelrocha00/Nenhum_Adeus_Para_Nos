using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    GameObject objectToPlace;
    GameObject objectToPlaceMeshOnly;
    GameObject auxObj;
    LayerMask mask;

    bool placing;

    public void StartPlacingItem(GameObject obj, GameObject objMeshonly, LayerMask mk)
    {
        objectToPlace = obj;
        objectToPlaceMeshOnly = objMeshonly;
        mask = mk;
        StartCoroutine("PlacingItem");
    }

    IEnumerator PlacingItem()
    {
        placing = true;
        Ray ray;
        RaycastHit hit;

        auxObj = Instantiate(objectToPlaceMeshOnly, GameManager.gameManager.battleController.MainCharacter.transform) as GameObject;

        while (placing)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000, mask))
            {
                auxObj.transform.position = hit.point;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void ComfirmPlace(PlaceableItem pI)
    {
        Debug.Log("Placing");
        //GameObject aux = Instantiate(objectToPlace, auxObj.transform.position, auxObj.transform.rotation) as GameObject;
        //Destroy(aux, 5);
        pI.Place(auxObj.transform.position, auxObj.transform.rotation);
        CancelPlacing();
    }

    public void CancelPlacing()
    {
        if (placing)
        {
            placing = false;
            Destroy(auxObj);
        }
    }
}
