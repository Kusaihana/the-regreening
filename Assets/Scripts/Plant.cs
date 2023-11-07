using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Plant : MonoBehaviour
{
    [SerializeField] private TMP_Text _stageLabel;
    [SerializeField] private PlantType _plantType;
    [SerializeField] private GrowthStage _currentStage;
    
    private float _growthProgress;
    private float _currentHealth;

    
    void Start()
    {
        _currentStage = GrowthStage.Seed;
        _growthProgress = 0;
        _currentHealth = _plantType.maxHealth;
    }
    
    void Update()
    {
        if (_currentStage != GrowthStage.Dead)
        {
            float elapsedTime = Time.deltaTime;
            UpdatePlantGrowth(elapsedTime);
        }
    }

    private void UpdatePlantGrowth(float elapsedTime)
    {
        if (_currentStage == GrowthStage.Dead)
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
                if (_currentHealth > 0)
                {
                    _currentHealth -= (stageParams.waterRequirement - waterAvailable) * elapsedTime;
                    _currentHealth = Mathf.Clamp(_currentHealth, 0, _plantType.maxHealth);
                }
            }
        }

        // Update the growth progress based on elapsed time and stage-specific growth time.
        _growthProgress += elapsedTime / stageParams.growthTime;

        if (_growthProgress >= 1.0f)
        {
            TransitionToNextStage();
        }
        
    }
    
    private GrowthStageParameters GetCurrentStageParameters()
    {
        switch (_currentStage)
        {
            case GrowthStage.Seed:
                return _plantType.seedStage;
            case GrowthStage.Seedling:
                return _plantType.seedlingStage;
            case GrowthStage.Sapling:
                return _plantType.saplingStage;
            case GrowthStage.Adult:
                return _plantType.adultStage;
            default:
                return new GrowthStageParameters();
        }
    }
    
    private void TransitionToNextStage()
    {
        GrowthStage nextStage = GetNextGrowthStage(_currentStage);

        if (nextStage != GrowthStage.Dead)
        {
            _currentStage = nextStage;
            _currentHealth = 0.0f;
            
            UpdatePlantAppearance();
        }
        else
        {
            _currentStage = GrowthStage.Dead;
            
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
