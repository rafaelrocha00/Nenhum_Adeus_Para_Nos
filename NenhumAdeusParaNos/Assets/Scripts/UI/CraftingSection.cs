﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSection : MonoBehaviour
{
    public Image brokenObjectIcon;
    public BrokenObject brokenObject;

    public int maxMaterials;

    public Image combinedProgressionBar;
    public Image baseProgressionBar;

    public GameObject materialSlotPref;

    public int totalBonusTime = 0;

    public int totalCombinedValue = 0;
    public int totalBaseValue = 0;

    int minCombinedValue = 0;
    bool repair_enabled = false;

    public Transform materialSlotTab;
    public List<MaterialSlot> materialSlots = new List<MaterialSlot>();

    public Color c_repair_enabled;
    public Color c_repair_disabled;

    public AudioClip clip_craft;

    private void Start()
    {
        MaterialSlot ms = materialSlotTab.GetComponentInChildren<MaterialSlot>();
        materialSlots.Add(ms);
    }

    public void SetBrokenObject(BrokenObject bo)
    {
        brokenObject = bo;
        minCombinedValue = bo.MinCombinedValue;
        brokenObjectIcon.sprite = bo.objIcon;

        totalBonusTime = 0;
        totalCombinedValue = 0;
        totalBaseValue = 0;
    }

    public void AddResource(int cv, int bt, bool base_mat = false)
    {
        totalCombinedValue += cv;
        if (base_mat) totalBaseValue += cv;

        totalBonusTime += bt;

        if (cv > 0) AddNewMaterialSlot();
        else Invoke("RemoveMaterialSlot", 0.01f);//();
        
        if (totalCombinedValue >= minCombinedValue && totalBaseValue >= minCombinedValue / 2) repair_enabled = true;
        else repair_enabled = false;

        UpdateProgressionBar();
    }

    void UpdateProgressionBar()
    {
        float combinedValueRate = (float)totalCombinedValue / minCombinedValue * 0.75f;
        float baseValueRate = (float)totalBaseValue / totalCombinedValue * combinedValueRate;
        baseValueRate = (totalCombinedValue == 0) ? 0 : baseValueRate;

        if (repair_enabled)
        {
            combinedProgressionBar.color = c_repair_enabled;
        }
        else combinedProgressionBar.color = c_repair_disabled;

        combinedProgressionBar.fillAmount = combinedValueRate;
        baseProgressionBar.fillAmount = baseValueRate;

        //if (combinedValueRate <= 1.34f) combinedProgressionBar.transform.localScale = new Vector3(combinedValueRate, 1, 1);
        //else combinedProgressionBar.transform.localScale = new Vector3(1.34f, 1, 1);

        //if (baseValueRate <= 1.34f) baseProgressionBar.transform.localScale = new Vector3(baseValueRate, 1, 1);
        //else baseProgressionBar.transform.localScale = new Vector3(1.34f, 1, 1);
    }

    public void AddNewMaterialSlot()
    {
        if (materialSlots.Count < maxMaterials + 1)
        {
            GameObject go = Instantiate(materialSlotPref, materialSlotTab, false) as GameObject;
            MaterialSlot ms = go.GetComponent<MaterialSlot>();
            ms.CraftingSection = this;
            materialSlots.Add(ms);
        }
    }
    public void RemoveMaterialSlot()
    {
        //materialSlots[idx].gameObject.SetActive(false);
        //if (idx + 1 == maxMaterials) materialSlots[idx - 1].EnablePlusIcon(true);
        for (int i = 0; i < materialSlots.Count; i++)
        {
            if (materialSlots[i] == null)
            {
                materialSlots.RemoveAt(i);
            }
        }
    }

    public void DropOnLastSlot(ItemButton ib)
    {
        materialSlots[materialSlots.Count - 1].OnDrop(ib);
    }

    public void Confirm()
    {
        if (repair_enabled)
        {
            int bt = totalBonusTime;
            for (int i = 0; i < materialSlots.Count; i++)
            {
                if (materialSlots[i].ThisItemB != null)
                {
                    materialSlots[i].ThisItemB.RemoveAndDestroy();
                    materialSlots[i].OnRemove();
                }
            }
            RemoveMaterialSlot();
            brokenObject.FinishRepair(bt);
            GameManager.gameManager.audioController.PlayEffect(clip_craft);
        }
    }

    public void Cancel()
    {
        GameManager.gameManager.MainHud.OpenCloseInventory(false);       
    }

    public void ReturnItems()
    {
        for (int i = 0; i < materialSlots.Count; i++)
        {
            if (materialSlots[i].ThisItemB != null)
            {
                GameManager.gameManager.inventoryController.Inventory.myGrid.TryAlocateItem(materialSlots[i].ThisItemB);
                materialSlots[i].OnRemove();
            }
        }
        RemoveMaterialSlot();
    }

    ///////////////////////////////////////////////////////////////
    /////Bugzin ao sair do bau e tentar interagir com o objeto consertavel
    /////////////////////////
}
