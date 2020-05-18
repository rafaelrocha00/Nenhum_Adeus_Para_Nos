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
}

public struct Resource
{
    public CompanyController.ResourceType type;
    public int quant;
}
