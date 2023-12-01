using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public enum LandType
{
    Desert, //R
    Alive, //G
    Vivid, //B
    Lush, //A
}


public class SoilTile : MonoBehaviour
{
    [SerializeField] private TMP_Text _tileLabel;
    public List<PlantSpecs> seedTypes;

    public float waterPercentage;
    public float qualityLand;
    public float evaporationVariable;
    public LandType landType;
    public List<Plant> plantsOnTile = new List<Plant>();
    public int bermCount;

    private TileColorSetter _tileColorSetter;
    private int _seedAmount;
    private Inventory _inventory;

    private void Awake()
    {
        _tileColorSetter = FindObjectOfType<TileColorSetter>();
        _inventory = FindObjectOfType<Inventory>();
        InvokeRepeating("RainRainRain", 10f,60f);
    }

    void Update()
    {
        if (waterPercentage > 0)
        {
            float elapsedTime = Time.deltaTime;
            Evaporate(elapsedTime);
        }
    }
    
    void RainRainRain()
    {
        UpdateWater(100);
        _tileColorSetter.ShowRain();
    }

    public void SetSoilProperties(LandType landType)
    {
        this.landType = landType;

        if (landType == LandType.Desert)
        {
            waterPercentage = 100;
            evaporationVariable = 2f;
        }

        _tileLabel.color = GetColorByTileType();
        _tileLabel.text = waterPercentage.ToString();
    }

    public void UpdateEvaporationCoefficient(float coefficient)
    {
        evaporationVariable *= coefficient;
    }
    
    public void UpdateWater(float waterAmount)
    {
        waterPercentage += waterAmount;
        waterPercentage = Math.Clamp(waterPercentage, 0, 1000);
        _tileLabel.color = GetColorByTileType();
        _tileLabel.text = ((int)waterPercentage).ToString();
        CheckUpgrade();
    }

    public void AddBerm()
    {
        bermCount++;
    }
    
    private Color GetColorByTileType()
    {
        return landType switch
        {
            LandType.Desert => Color.red,
            LandType.Alive => Color.yellow,
            LandType.Vivid => Color.cyan,
            _ => Color.green
        };
    }

    private void Evaporate(float elapsedTime)
    {
        UpdateWater(-elapsedTime / evaporationVariable);

        for (var i = 0; i < bermCount; i++)
        {
            UpdateWater(elapsedTime * 25/60f); // water berm adds 25 water to the tile per minute (in game day)
        }
    }
    
    private void CheckUpgrade()
    {
        if (landType == LandType.Desert)
        {
            if (CountPlantsOfType(PlantType.Grass) >= 1 && waterPercentage >= 150)
            {
                SetTileType(LandType.Alive);
            }
        }
        else if (landType == LandType.Alive)
        {
            if (CountPlantsOfType(PlantType.Grass) >= 1 &&
                CountPlantsOfType(PlantType.Dandelion) >= 1 &&
                waterPercentage >= 300)
            {
                SetTileType(LandType.Vivid);
                _inventory.AddSeed(seedTypes[2], 5);
            }
            else if (waterPercentage < 150)
            {
                SetTileType(LandType.Desert);
            }
        }
        else if (landType == LandType.Vivid)
        {
            if (CountPlantsOfType(PlantType.Bush) >= 1 &&
                waterPercentage >= 500)
            {
                SetTileType(LandType.Lush);
                _inventory.AddSeed(seedTypes[3], 5);
            }
            else if (waterPercentage < 300)
            {
                SetTileType(LandType.Alive);
            }
        }
    }
    
    private void SetTileType(LandType newType)
    {
        landType = newType;
        _tileColorSetter.UpdateTileColors();
    }
    
    private int CountPlantsOfType(PlantType plantType)
    {
        return plantsOnTile.Count(plant => plant.plantSpec.plantType == plantType && plant.currentStage == GrowthStage.Adult);
    }
}
