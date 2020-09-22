using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Delivery Quest")]
public class DeliveryQuest : Quest
{
    [SerializeField] string depositName = "";
    public string DepositName { get { return depositName; } set { depositName = value; } }

    public Item[] itemsToDelivery;
    public int[] itemsQuant;

    public GameObject toInstantiate;

    public override void CheckComplete<T>(T thing)
    {
        try
        {
            Storage sto = thing as Storage;
            if (sto.Name.Equals(depositName))
            {
                for (int i = 0; i < itemsToDelivery.Length; i++)
                {
                    int aux = sto.CheckQuestItems(itemsToDelivery[i].itemName);
                    if (aux < itemsQuant[i]) return;
                }
                TryComplete();
            }            
        }
        catch { Debug.Log("Not a storage"); }
    }

    public override void InstantiateObjs(ObjectInstancer oi)
    {
        oi.SpawnChest(depositName, toInstantiate);
    }
}
