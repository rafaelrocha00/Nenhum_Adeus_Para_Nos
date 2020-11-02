using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Delivery Quest")]
public class DeliveryQuest : Quest
{
    [Header("Delivery Quest")]
    [SerializeField] string depositName = "";
    public string DepositName { get { return depositName; } set { depositName = value; } }

    public Item[] itemsToDelivery;
    public int[] itemsQuant;

    public GameObject toInstantiate;

    public override void AcceptQuest()
    {
        base.AcceptQuest();
        if (generated)
        {
            for (int i = 0; i < itemsToDelivery.Length; i++)
            {
                for (int j = 0; j < itemsQuant[i]; j++)
                {
                    GameManager.gameManager.itemsSaver.itemsToDelivery.Enqueue(itemsToDelivery[i]);
                }
            }
        }
    }

    public override void CheckComplete<T>(T thing)
    {
        try
        {
            Storage sto = thing as Storage;
            Debug.Log("Tentando compeltar a quest");
            if (sto.Name.Equals(depositName))
            {
                for (int i = 0; i < itemsToDelivery.Length; i++)
                {
                    int aux = sto.CheckQuestItems(itemsToDelivery[i].itemName);
                    if (aux < itemsQuant[i]) return;
                }
                Debug.Log("Completando");

                for (int i = 0; i < itemsToDelivery.Length; i++)
                {
                    sto.RemoveItems(itemsToDelivery[i].itemName, itemsQuant[i]);
                }
                TryComplete();
                sto.CheckQuestMarker();
            }            
        }
        catch { Debug.Log("Not a storage"); }
    }

    public override void InstantiateObjs(ObjectInstancer oi)
    {
        oi.SpawnChest(depositName, toInstantiate);
    }
}
