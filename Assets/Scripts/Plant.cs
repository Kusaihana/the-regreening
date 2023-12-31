using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Plant : MonoBehaviour
{
    public PlantSpecs plantSpec;
    
    public GrowthStage currentStage;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] public GameObject _growthParticlesPrefab; 

    private float _growthProgress;
    private float _currentHealth;
    private float _currentGrowthTime;
    private SoilTile _tileAssigned;
    private Material _visual;
    
    void Start()
    {
        currentStage = GrowthStage.Seed;
        _growthProgress = 0;
        _currentHealth = plantSpec.maxHealth;
        SetRandomGrowthTime(_tileAssigned.landType);

        _visual = _meshRenderer.material;
    }
    
    void Update()
    {
        if (currentStage != GrowthStage.Dead)
        {
            float elapsedTime = Time.deltaTime / 60f;
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

        if (_tileAssigned != null && _tileAssigned.waterPercentage > 0 && currentStage != GrowthStage.Dead)
        {
            _tileAssigned.UpdateWater(-elapsedTime * stageParams.waterUsage);
        }
    }

    private void UpdatePlantGrowth(float elapsedTime)
    {
        GrowthStageParameters stageParams = GetCurrentStageParameters();
        
        // Check if the plant has enough water to continue growing
        if (stageParams.waterRequirement > 0)
        {
            float waterAvailable = _tileAssigned.waterPercentage;
            if (waterAvailable < stageParams.waterRequirement)
            {
                // Reduce health if there's not enough water
                if (_currentHealth > 0)
                {
                    _currentHealth -= (stageParams.waterRequirement - waterAvailable) * elapsedTime;
                    _currentHealth = Mathf.Clamp(_currentHealth, 0, plantSpec.maxHealth);
                }

                return;
            }
        }

        // Update the growth progress based on elapsed time and stage-specific growth time.
        _growthProgress += elapsedTime / _currentGrowthTime;

        if (_growthProgress >= 1.0f)
        {
            TransitionToNextStage();
        }
        
    }
    
    public GrowthStageParameters GetCurrentStageParameters()
    {
        switch (currentStage)
        {
            case GrowthStage.Seed:
                return plantSpec.seedStage;
            case GrowthStage.Seedling:
                return plantSpec.seedlingStage;
            case GrowthStage.Sapling:
                return plantSpec.saplingStage;
            case GrowthStage.Adult:
                return plantSpec.adultStage;
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
            _currentHealth = plantSpec.maxHealth;
            _growthProgress = 0;
            SetRandomGrowthTime(_tileAssigned.landType);
            SpawnGrowthParticles();
            
            UpdatePlantAppearance();
        }
        else
        {
            currentStage = GrowthStage.Dead;
            
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
    
    private void SetRandomGrowthTime(LandType landType)
    {
        GrowthStageParameters stageParams = GetCurrentStageParameters();

        var growthTimeMin = stageParams.growthTimeRangeDesert.min;
        var growthTimeMax = stageParams.growthTimeRangeDesert.max;
        
        if (landType == LandType.Alive)
        {
            growthTimeMin = stageParams.growthTimeRangeAlive.min;
            growthTimeMax = stageParams.growthTimeRangeAlive.max;
        }
        else if (landType == LandType.Vivid)
        {
            growthTimeMin = stageParams.growthTimeRangeVivid.min;
            growthTimeMax = stageParams.growthTimeRangeVivid.max;
        }
        else if (landType == LandType.Lush)
        {
            growthTimeMin = stageParams.growthTimeRangeLush.min;
            growthTimeMax = stageParams.growthTimeRangeLush.max;
        }

        _currentGrowthTime = Random.Range(growthTimeMin, growthTimeMax);
    }
    
    private void UpdatePlantAppearance()
    {
        _visual.SetInt("_AtlasTile", (int)currentStage);
    }
    
    void SpawnGrowthParticles()
    {
        if (_growthParticlesPrefab != null)
        {
            GameObject particles = Instantiate(_growthParticlesPrefab, transform.position, Quaternion.identity);
            
            particles.transform.parent = transform;
            
            particles.GetComponent<ParticleSystem>().Play();
            
            var particleDuration = particles.GetComponent<ParticleSystem>().main.duration;

            Destroy(particles, particleDuration);
        }
    }

    private void HandlePlantDeath()
    {
        // TODO
    }
}
