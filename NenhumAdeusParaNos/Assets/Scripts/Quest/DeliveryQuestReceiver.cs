using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Delivery Receiver", fileName = "New Receiver")]
public class DeliveryQuestReceiver : ScriptableObject
{
    [SerializeField] string receiver_location = "";
    [SerializeField] Item[] receiver_possible_deliveries = null;
    [SerializeField] Vector2Int receiver_deliveries_quant = Vector2Int.one;
    [SerializeField] Item[] receiver_possible_rewards = null;
    [SerializeField] Vector2Int receiver_rewards_quant = Vector2Int.one;

    public string Receiver_Location { get { return receiver_location; } }

    public Item GetDelivery()
    {
        return receiver_possible_deliveries[Random.Range(0, receiver_possible_deliveries.Length)];
    }

    public int GetDeliveryQuant()
    {
        return Random.Range(receiver_deliveries_quant.x, receiver_deliveries_quant.y + 1);
    }

    public Item[] GetReward()
    {
        Item[] items = new Item[1];
        items[0] = receiver_possible_rewards[Random.Range(0, receiver_possible_rewards.Length)];

        return items;//receiver_possible_rewards[Random.Range(0, receiver_possible_rewards.Length)];
    }

    public int[] GetRewardQuant(Item item)
    {
        int[] quants = new int[1];
        quants[0] = Random.Range(receiver_rewards_quant.x, receiver_rewards_quant.y + 1);

        if (item is WeaponItem) quants[0] = 1;

        return quants;//Random.Range(receiver_rewards_quant.x, receiver_rewards_quant.y + 1);
    }
}
