using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DropSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] protected ItemButton thisItemB;
    public ItemButton ThisItemB { get { return thisItemB; } set { thisItemB = value; } }

    public abstract bool OnDrop(ItemButton itemButton);

    public virtual void OnRemove()
    {
        Debug.Log("Removing");
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("OnPointerEnter");
        GameManager.gameManager.inventoryController.ActualDropSlot = this;
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("OnPointerExit");
        GameManager.gameManager.inventoryController.ActualDropSlot = null;
    }
}
