using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public PlantType plantType;
    public GrowthStage currentStage;
    public float growthProgress;
    public float currentHealth;

    
    void Start()
    {
        currentStage = GrowthStage.Seed;
        growthProgress = 0;
        currentHealth = plantType.maxHealth;
    }
    
    void Update()
    {
        if (currentStage != GrowthStage.Dead)
        {
            float elapsedTime = Time.deltaTime;
            UpdatePlantGrowth(elapsedTime);
        }
    }

    private void UpdatePlantGrowth(float elapsedTime)
    {
        if (currentStage == GrowthStage.Dead)
        {
            // Don't update a dead plant :)
            return;
        }
        
        GrowthStageParameters stageParams = GetCurrentStageParameters();
        
        // Check if the plant has enough water to continue growing
        if (stageParams.waterRequirement > 0)
        {
            float waterAvailable = 50;// TODO Get water availability from nearby tiles or other sources
            if (waterAvailable < stageParams.waterRequirement)
            {
                // Reduce health if there's not enough water
                if (currentHealth > 0)
                {
                    currentHealth -= (stageParams.waterRequirement - waterAvailable) * elapsedTime;
                    currentHealth = Mathf.Clamp(currentHealth, 0, plantType.maxHealth);
                }
            }
        }

        // Update the growth progress based on elapsed time and stage-specific growth time.
        growthProgress += elapsedTime / stageParams.growthTime;

        if (growthProgress >= 1.0f)
        {
            TransitionToNextStage();
        }
        
    }
    
    private GrowthStageParameters GetCurrentStageParameters()
    {
        switch (currentStage)
        {
            case GrowthStage.Seed:
                return plantType.seedStage;
            case GrowthStage.Seedling:
                return plantType.seedlingStage;
            case GrowthStage.Sapling:
                return plantType.saplingStage;
            case GrowthStage.Adult:
                return plantType.adultStage;
            default:
                return new GrowthStageParameters();
        }
    }
    
    private void TransitionToNextStage()
    {
        GrowthStage nextStage = GetNextGrowthStage(currentStage);

        if (nextStage != GrowthStage.Dead)
        {
            currentStage = nextStage;
            growthProgress = 0.0f;
            
            UpdatePlantAppearance();
        }
        else
        {
            currentStage = GrowthStage.Dead;
            
            HandlePlantDeath();
        }
    }
    
    private GrowthStage GetNextGrowthStage(GrowthStage currentStage)
    {
        switch (currentStage)
        {
            case GrowthStage.Seed:
                return GrowthStage.Seedling;
            case GrowthStage.Seedling:
                return GrowthStage.Sapling;
            case GrowthStage.Sapling:
                return GrowthStage.Adult;
            case GrowthStage.Adult:
                return GrowthStage.Dead;
            default:
                return GrowthStage.Dead;
        }
    }
    
    private void UpdatePlantAppearance()
    {
        // TODO
    }

    private void HandlePlantDeath()
    {
        // TODO
    }
}
