using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum LandType
{
    Dessert,
    Alive,
    Vivid,
    Lush,
    Swamp,
    Water
}

public class SoilTile : MonoBehaviour
{
    [SerializeField] private TMP_Text _tileLabel;
    public float waterPercentage;
    public float evaporationVariable;
    public LandType landType;
    public int numOfPlants;
    public List<Plant> plantsOnTile = new List<Plant>();

    private int _seedAmount;

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

        if (landType == LandType.Dessert)
        {
            waterPercentage = 100;
            evaporationVariable = 1;
        }

        _tileLabel.color = GetColorByWaterPercentage(waterPercentage);
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
        _tileLabel.color = GetColorByWaterPercentage(waterPercentage);
        _tileLabel.text = ((int)waterPercentage).ToString();
    }
    
    private Color GetColorByWaterPercentage(float waterPercentage)
    {
        return waterPercentage switch
        {
            <= 100 => Color.red,
            <= 200 => Color.yellow,
            <= 300 => Color.blue,
            <= 400 => Color.green,
            <= 500 => Color.cyan,
            _ => Color.blue
        };
    }

    private void Evaporate(float elapsedTime)
    {
        UpdateWater(-elapsedTime / evaporationVariable);
    }
}
