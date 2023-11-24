using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private TMP_Text _waterText;
    [SerializeField] private TMP_Text _dandelionText;
    [SerializeField] private TMP_Text _commonOakText;
    [SerializeField] private TMP_Text _catGrassText;
    [SerializeField] private TMP_Text _wildRoseText;
    
    public Dictionary<string, int> seeds = new Dictionary<string, int>();
    public int wateringCanFillAmount = 100;
    public int pebbleAmount;

    public List<PlantSpecs> seedTypes;

    void Start()
    {
        foreach (var seedType in seedTypes)
        {
            AddSeed(seedType, 5);
        }
    }
    
    public void AddSeed(PlantSpecs seed, int quantity)
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
            _dandelionText.text = seeds[seedName].ToString();
        }
        else if (seedName == "Wild Rose")
        {
            _wildRoseText.text = seeds[seedName].ToString();
        }
        else if (seedName == "Common Oak")
        {
            _commonOakText.text = seeds[seedName].ToString();
        }
        else if (seedName == "Cat Grass")
        {
            _catGrassText.text = seeds[seedName].ToString();
        }
    }

    private void UpdateWaterText()
    {
        _waterText.text = $"{wateringCanFillAmount}%";
    }

    public bool PlantSeed(PlantSpecs seed)
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

    public void FillWateringCan()
    {
        wateringCanFillAmount = 100;
        UpdateWaterText();
    }

    public void WaterTile(int amount)
    {
        if (wateringCanFillAmount > 0)
        {
            wateringCanFillAmount -= amount;
            UpdateWaterText();
        }
        else
        {
            Debug.Log("Watering can is empty.");
        }
    }
}