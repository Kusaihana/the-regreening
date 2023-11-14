using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Plant : MonoBehaviour
{
    [SerializeField] private TMP_Text _stageLabel;
    [SerializeField] private PlantType _plantType;
    [SerializeField] private GrowthStage _currentStage;
    [SerializeField] private MeshRenderer _meshRenderer;

    private float _growthProgress;
    private float _currentHealth;
    private float _currentGrowthTime;
    private SoilTile _tileAssigned;
    private Material _visual;
    
    void Start()
    {
        _currentStage = GrowthStage.Seed;
        _growthProgress = 0;
        _currentHealth = _plantType.maxHealth;
        SetRandomGrowthTime();

        _visual = _meshRenderer.material;
    }
    
    void Update()
    {
        if (_currentStage != GrowthStage.Dead)
        {
            float elapsedTime = Time.deltaTime;
            UpdatePlantGrowth(elapsedTime);
            UseWater(elapsedTime);
        }
    }

    public void SetTile(SoilTile soilTile)
    {
        _tileAssigned = soilTile;
    }
    
    private void UseWater(float elapsedTime)
    {
        GrowthStageParameters stageParams = GetCurrentStageParameters();

        if (_tileAssigned != null && _tileAssigned.waterPercentage > 0)
        {
            _tileAssigned.UseWater(elapsedTime / stageParams.waterUsage);
        }
    }

    private void UpdatePlantGrowth(float elapsedTime)
    {
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
        _growthProgress += elapsedTime / _currentGrowthTime;

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
            _currentHealth = _plantType.maxHealth;
            _growthProgress = 0;
            SetRandomGrowthTime();
            
            UpdatePlantAppearance();
        }
        else
        {
            _currentStage = GrowthStage.Dead;
            
            UpdatePlantAppearance();
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
    
    private void SetRandomGrowthTime()
    {
        GrowthStageParameters stageParams = GetCurrentStageParameters();
        _currentGrowthTime = Random.Range(stageParams.growthTimeRange.min, stageParams.growthTimeRange.max);
    }
    
    private void UpdatePlantAppearance()
    {
        _visual.SetInt("_AtlasTile", (int)_currentStage);
        _stageLabel.text = _currentStage.ToString();
    }

    private void HandlePlantDeath()
    {
        // TODO
    }
}
