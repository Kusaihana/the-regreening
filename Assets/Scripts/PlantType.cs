using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public string plantName;
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
    public float nutritionRequirement;
}

[System.Serializable]
public class Range
{
    public float min;
    public float max;
}
