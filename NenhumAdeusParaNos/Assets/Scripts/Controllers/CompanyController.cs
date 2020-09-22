using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyController : MonoBehaviour
{
    public enum ResourceType { Tool, Food, Med, Feedstock, None }

    [SerializeField] float money;
    public float Money { get { return money; } set { money = value; } }

    Resource[] resources = new Resource[4];

    public Queue<Quest> quests_onPC = new Queue<Quest>();
    public Queue<Item> itemsToAlocate = new Queue<Item>();

    private void Start()
    {
        for (int i = 0; i < resources.Length; i++)
        {
            resources[i].type = (ResourceType)i;
            resources[i].quant = 0;
            Debug.Log(resources[i].type);
        }
    }

    public void AddResource(ResourceType type, int quant)
    {
        if (type == ResourceType.None) return;
        resources[(int)type].quant += quant;
    }
    public int GetResourceQuant(int id)
    {
        return resources[id].quant;
    }

    public bool CheckIfQuestExist(string objectName)
    {
        Quest[] aux = new Quest[quests_onPC.Count];
        //Debug.Log("Checando na Compania");
        quests_onPC.CopyTo(aux, 0);

        for (int i = 0; i < aux.Length; i++)
        {
            //Debug.Log("Analisando quests que estão no pc: " + aux[i].Name);
            if (aux[i] is RepairQuest)
            {
                RepairQuest rq = (RepairQuest)aux[i];
                return rq.ObjectToRepair.Equals(objectName);
            }
        }
        return false;
    }
}

public struct Resource
{
    public CompanyController.ResourceType type;
    public int quant;
}
