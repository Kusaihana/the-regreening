using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private TMP_Text _dandelionText;
    [SerializeField] private TMP_Text _commonOakText;
    [SerializeField] private TMP_Text _catGrassText;
    [SerializeField] private TMP_Text _wildRoseText;
    
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

        UpdateSeedText(seedName);
    }

    private void UpdateSeedText(string seedName)
    {
        if (seedName == "Dandelion")
        {
            _dandelionText.text = $"Dandelion: {seeds[seedName]}";
        }
        else if (seedName == "Wild Rose")
        {
            _wildRoseText.text = $"Wild Rose: {seeds[seedName]}";
        }
        else if (seedName == "Common Oak")
        {
            _commonOakText.text = $"Common Oak: {seeds[seedName]}";
        }
        else if (seedName == "Cat Grass")
        {
            _catGrassText.text = $"Cat Grass: {seeds[seedName]}";
        }
    }

    public bool PlantSeed(PlantType seed)
    {
        var seedName = seed.commonName;
        
        if (seeds.ContainsKey(seedName) && seeds[seedName] > 0)
        {
            seeds[seedName]--;
            UpdateSeedText(seed.commonName);
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