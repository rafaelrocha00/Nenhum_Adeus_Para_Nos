using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooteableSpot : Interactives
{
    public ResourceItem[] possibleItens = new ResourceItem[1];

    public Vector2Int quantRange;

    public override void Interact(Player player)
    {
        int randItem = Random.Range(0, possibleItens.Length);

        int aux = Random.Range(quantRange.x, quantRange.y + 1);

        GameManager.gameManager.MainHud.OpenCloseInventory(true);

        StartCoroutine(GenItens(aux, randItem, player));
    }

    IEnumerator GenItens(int quant, int item, Player p)
    {
        yield return new WaitForSeconds(0.05f);
        for (int i = 0; i < quant; i++)
        {
            GameManager.gameManager.inventoryController.Inventory.AddItem(possibleItens[item]);
        }

        OnExit(p);
        this.gameObject.SetActive(false);
    }
}
