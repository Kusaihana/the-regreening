using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    }

    void Update()
    {
        if (waterPercentage > 0)
        {
            float elapsedTime = Time.deltaTime;
            Evaporate(elapsedTime);
        }
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
        CheckUpgrade();
    }
    
    private void CheckUpgrade()
    {
        if (landType == LandType.Desert && CountPlantsOfType(PlantType.Grass) > 0 && waterPercentage >= 100)
        {
            UpgradeTile(LandType.Alive);
        }
        else if (landType == LandType.Alive && CountPlantsOfType(PlantType.Grass) > 1 && CountPlantsOfType(PlantType.Bush) > 0 && waterPercentage >= 300)
        {
            UpgradeTile(LandType.Vivid);
        }
        else if (landType == LandType.Vivid && CountPlantsOfType(PlantType.Grass) > 0 && CountPlantsOfType(PlantType.Bush) > 0 && CountPlantsOfType(PlantType.Tree) > 1 && waterPercentage >= 500)
        {
            UpgradeTile(LandType.Lush);
        }
    }
    
    private void UpgradeTile(LandType newType)
    {
        landType = newType;
        _tileColorSetter.UpdateTileColors();
    }
    
    private int CountPlantsOfType(PlantType plantType)
    {
        return plantsOnTile.FindAll(plant => plant.plantSpec.plantType == plantType).Count;
    }
}
