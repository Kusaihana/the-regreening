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
    [SerializeField] private TMP_Text _pebbleText;
    
    public Dictionary<string, int> seeds = new Dictionary<string, int>();
    public int wateringCanFillAmount = 100;
    public int pebbleAmount;

    public List<PlantSpecs> seedTypes;

    void Start()
    {
        AddSeed(seedTypes[0], 10);
        AddSeed(seedTypes[1], 10);
    }

    public void AddPebble()
    {
        pebbleAmount++;
        _pebbleText.text = $"{pebbleAmount}/10";
    }
    
    public void UsePebble()
    {
        pebbleAmount -= 10;
        _pebbleText.text = $"{pebbleAmount}/10";
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

    public void UpdateSeedText(string seedName)
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