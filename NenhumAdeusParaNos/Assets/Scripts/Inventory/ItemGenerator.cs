using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGenerator : MonoBehaviour
{
    public Transform spawnPos;
    public GameObject itemPref;

    //public Item itemConfig;
    public List<Item> items = new List<Item>();

    public void GenRandomItem()
    {
        if (items.Count > 0)
        {
            Item itemAux = items[Random.Range(0, items.Count)];

            GenItem(itemAux);
            //GameObject newItemObj = Instantiate(itemPref, transform, false) as GameObject;
            //newItemObj.transform.position = this.transform.position;
            //RectTransform newItemRect = newItemObj.GetComponent<RectTransform>();
            //newItemRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newItemRect.sizeDelta.x * itemConfig.slotSize.x);
            //newItemRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newItemRect.sizeDelta.y * itemConfig.slotSize.y);
            //newItemObj.GetComponent<Image>().sprite = itemConfig.itemSprite;
            //newItemObj.GetComponent<ItemButton>().Item = itemConfig;
        }
    }

    public ItemButton GenItem(Item item)
    {
        GameObject newItemObj = Instantiate(itemPref, transform, false) as GameObject;
        newItemObj.transform.position = this.transform.position;
        RectTransform newItemRect = newItemObj.GetComponent<RectTransform>();
        newItemRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newItemRect.sizeDelta.x * item.slotSize.x);
        newItemRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newItemRect.sizeDelta.y * item.slotSize.y);
        newItemObj.GetComponent<Image>().sprite = item.itemSprite;
        newItemObj.GetComponent<ItemButton>().Item = item;

        return newItemObj.GetComponent<ItemButton>();
    }

}
