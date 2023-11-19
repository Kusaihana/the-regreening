using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GrowthStage
{
    Seed,
    Seedling,
    Sapling,
    Adult,
    Dead
}

[CreateAssetMenu(menuName = "Plant Type")]
public class PlantType : ScriptableObject
{
    public string commonName;
    public string latinName;
    public string description;
    public string distribution;
    public Image seedIcon;
    public int maxHealth;
    
    public GrowthStageParameters seedStage;
    public GrowthStageParameters seedlingStage;
    public GrowthStageParameters saplingStage;
    public GrowthStageParameters adultStage;
}

[System.Serializable]
public class GrowthStageParameters
{
    public Range growthTimeRange; //in game days
    public float waterRequirement;
    public float evaporationEffect;
    public float waterUsage;
}

[System.Serializable]
public class Range
{
    public float min;
    public float max;
}
