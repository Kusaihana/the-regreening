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
    public float waterRetention;
    public LandType landType;

    private int _seedAmount;

    public void SetSoilProperties(LandType landType)
    {
        this.landType = landType;

        if (landType == LandType.Dessert)
        {
            waterPercentage = 100;
        }

        _tileLabel.color = GetColorByWaterPercentage(waterPercentage);
        _tileLabel.text = waterPercentage.ToString();
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

    private void CheckIfStable()
    {

    }
}
