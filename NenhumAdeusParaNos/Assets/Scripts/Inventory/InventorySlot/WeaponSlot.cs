using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSlot : DropSlot
{
    [HideInInspector] Player mainCharacter;
    public Player MainCharacter { get { if (mainCharacter == null) mainCharacter = GameManager.gameManager.battleController.MainCharacter; return mainCharacter; } set { mainCharacter = value; } }

    public bool isRanged = false;
    WeaponItem weaponItem;
    //ItemButton thisItemButton;

    public override bool OnDrop(ItemButton itemButton)
    {
        if (itemButton.Item is WeaponItem)
        {
            weaponItem = (WeaponItem)itemButton.Item;
            if ((weaponItem.thisWeapon is RangedConfig && isRanged) || (weaponItem.thisWeapon is MeleeConfig && !isRanged))
            {
                if (thisItemB != null)
                {
                    thisItemB.OriginDropSlot = null;
                    itemButton.OriginSlots[0, 0].MyGridManager.AlocateBigItem(thisItemB, itemButton.OriginSlots);
                }

                itemButton.transform.SetParent(transform);
                itemButton.OriginDropSlot = this;
                thisItemB = itemButton;
                itemButton.transform.position = this.transform.position;
                itemButton.ClearOrigin();
                MainCharacter.EquipWeapon(weaponItem.thisWeapon);
                return true;
            }
            else return false;
        }
        else return false;
    }

    public override void OnRemove()
    {
        base.OnRemove();
        MainCharacter.EquipOriginalWeapon(isRanged);
        thisItemB = null;
    }

    //public override void OnPointerEnter(PointerEventData eventData)
    //{
    //    base.OnPointerEnter(eventData);
    //}

    //public override void OnPointerExit(PointerEventData eventData)
    //{
    //    base.OnPointerExit(eventData);
    //    if (GameManager.gameManager.inventoryController.Dragging) MainCharacter.EquipOriginalWeapon(isRanged);
    //}
}
