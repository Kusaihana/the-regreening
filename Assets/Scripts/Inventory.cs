using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, int> seeds = new Dictionary<string, int>();
    public int wateringCanFillAmount = 0;

    public List<PlantType> seedTypes;

    void Start()
    {
        foreach (var seedType in seedTypes)
        {
            AddSeed(seedType, 5);
        }
    }
    
    public void AddSeed(PlantType seed, int quantity)
    {
        var seedName = seed.commonName;
        
        if (seeds.ContainsKey(seedName))
        {
            seeds[seedName] += quantity;
        }
        else
        {
            seeds.Add(seedName, quantity);
        }
    }

    public bool PlantSeed(PlantType seed)
    {
        var seedName = seed.commonName;
        
        if (seeds.ContainsKey(seedName) && seeds[seedName] > 0)
        {
            seeds[seedName]--;
            return true;
        }

        return false;
    }

    public void FillWateringCan(int amount)
    {
        wateringCanFillAmount += amount;
    }

    public void WaterTile()
    {
        if (wateringCanFillAmount > 0)
        {
            wateringCanFillAmount--;
        }
        else
        {
            Debug.Log("Watering can is empty.");
        }
    }
}