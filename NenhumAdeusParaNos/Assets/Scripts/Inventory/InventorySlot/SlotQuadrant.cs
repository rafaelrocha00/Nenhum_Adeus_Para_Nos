using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotQuadrant : MonoBehaviour, IPointerEnterHandler
{
    InvenSlot mySlot;

    public int xRed;
    public int yRed;

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    Debug.Log("PointerEnter");
    //    mySlot.SetSlotQuadrant(this);
    //}

    private void Start()
    {
        mySlot = transform.parent.GetComponent<InvenSlot>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mySlot.SetSlotQuadrant(this);
    }
}
