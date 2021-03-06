﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSlot : DropSlot
{
    public GameObject plusIcon;
    ResourceItem resource;

    public bool empty = true;

    [SerializeField] CraftingSection craftingSection = null;
    public CraftingSection CraftingSection { get { return craftingSection; } set { craftingSection = value; } }

    public override bool OnDrop(ItemButton itemButton)
    {
        //throw new System.NotImplementedException();
        if (itemButton.Item is ResourceItem && empty)
        {
            EnablePlusIcon(false);
            thisItemB = itemButton;
            thisItemB.OriginDropSlot = this;
            itemButton.ClearOrigin();
            itemButton.transform.position = transform.position;
            itemButton.WasInMaterialSlot = true;
            empty = false;
            itemButton.transform.SetParent(transform);
            resource = (ResourceItem)itemButton.Item;
            craftingSection.AddResource(resource.Value, resource.BonusTime, resource.MatType.Equals(craftingSection.brokenObject.baseMaterialType));//thisItemB.Item.itemName.Equals(craftingSection.brokenObject.baseMaterial.itemName));
            return true;
        }        
        else return false;
    }

    public void EnablePlusIcon(bool v)
    {
        plusIcon.SetActive(v);
    }

    public override void OnRemove()
    {
        if (thisItemB != null)
        {
            craftingSection.AddResource(-resource.Value, -resource.BonusTime, resource.MatType.Equals(craftingSection.brokenObject.baseMaterialType));
            thisItemB.OriginDropSlot = null;
            thisItemB = null;
        }
        Destroy(gameObject);
    }
}
