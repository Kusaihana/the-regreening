using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

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
    public float waterPercentage;
    public float qualityLand;
    public float evaporationVariable;
    public LandType landType;
    public List<Plant> plantsOnTile = new List<Plant>();

    private TileColorSetter _tileColorSetter;
    private int _seedAmount;

    private void Awake()
    {
        _tileColorSetter = FindObjectOfType<TileColorSetter>();
        InvokeRepeating("RainRainRain", 60f,60f);
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
        //TODO Jason make it rain please :)
        UpdateWater(100);
    }
    
    public void SetSoilProperties(LandType landType)
    {
        this.landType = landType;

        if (landType == LandType.Desert)
        {
            waterPercentage = 100;
            evaporationVariable = 1;
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
    }
    
    private void CheckUpgrade()
    {
        if (landType == LandType.Desert)
        {
            if (CountPlantsOfType(PlantType.Grass) >= 3 && waterPercentage >= 150)
            {
                SetTileType(LandType.Alive);
            }
        }
        else if (landType == LandType.Alive)
        {
            if (CountPlantsOfType(PlantType.Grass) >= 1 &&
                CountPlantsOfType(PlantType.Dandelion) >= 1 && CountPlantsOfType(PlantType.Bush) >= 2 &&
                waterPercentage >= 300)
            {
                SetTileType(LandType.Vivid);
            }
            else if (CountPlantsOfType(PlantType.Grass) < 3 && waterPercentage < 150)
            {
                SetTileType(LandType.Desert);
            }
        }
        else if (landType == LandType.Vivid)
        {
            if (CountPlantsOfType(PlantType.Bush) >= 1 &&
                CountPlantsOfType(PlantType.Tree) >= 3 && waterPercentage >= 500)
            {
                SetTileType(LandType.Lush);
            }
            else if (CountPlantsOfType(PlantType.Bush) < 2 && waterPercentage < 300)
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
        return plantsOnTile.FindAll(plant => plant.plantSpec.plantType == plantType && plant.currentStage == GrowthStage.Adult).Count;
    }
}
