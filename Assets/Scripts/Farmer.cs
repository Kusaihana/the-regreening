using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Farmer : MonoBehaviour
{
    [SerializeField] private List<PlantType> _seedTypes;
    
    public Inventory inventory;
    public float moveSpeed = 5f;
    public LayerMask tileLayer;
    public LayerMask plantLayer; 

    private const int MaxNumOfPlants = 4;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    void Update()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");
        
        if (horizontalInput > 0)
        {
            transform.GetChild(0).rotation = new Quaternion(0,180,0,0);
        }
        else if (horizontalInput != 0)
        {
            transform.GetChild(0).rotation = new Quaternion(0,0,0,0);
        }

        var movement = new Vector3(horizontalInput, verticalInput, 0f).normalized;

        MovePlayer(movement);
        
        CheckForInteraction();
    }

    private void MovePlayer(Vector3 movement)
    {
        var movementAmount = movement * moveSpeed * Time.deltaTime;
        transform.Translate(movementAmount, Space.World);
    }
    
    private void CheckForInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            WaterTile();
        }

        else if (Input.GetKeyDown(KeyCode.E))
        {
            RemovePlant();
        }
        
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // check if there are enough seeds in the inventory
            if (IsValidPlantingPosition() && inventory.PlantSeed(_seedTypes[0]))
            {
                SeedPlant(_seedTypes[0]);
            }
        }
        
        else if (IsValidPlantingPosition() && Input.GetKeyDown(KeyCode.Alpha2))
        {
            // check if there are enough seeds in the inventory
            if (inventory.PlantSeed(_seedTypes[1]))
            {
                SeedPlant(_seedTypes[1]);
            }
        }
        
        else if (IsValidPlantingPosition() && Input.GetKeyDown(KeyCode.Alpha3))
        {
            // check if there are enough seeds in the inventory
            if (inventory.PlantSeed(_seedTypes[2]))
            {
                SeedPlant(_seedTypes[2]);
            }
        }
        
        else if (IsValidPlantingPosition() && Input.GetKeyDown(KeyCode.Alpha4))
        {
            // check if there are enough seeds in the inventory
            if (inventory.PlantSeed(_seedTypes[3]))
            {
                SeedPlant(_seedTypes[3]);
            }
        }
    }

    void WaterTile()
    {
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
        {
            SoilTile soilTile = hit.transform.GetComponent<SoilTile>();

            if (soilTile != null)
            {
                soilTile.UpdateWater(10);
            }
        }
    }

    void SeedPlant(PlantType seedType)
    {
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
        {
            SoilTile soilTile = hit.transform.GetComponent<SoilTile>();

            if (soilTile != null && soilTile.plantsOnTile.Count < MaxNumOfPlants)
            {
                var plantGo = Instantiate(seedType.plantPrefab, transform.position, Quaternion.identity);
                var plant = plantGo.GetComponent<Plant>();
                plant.SetTile(soilTile);
                soilTile.plantsOnTile.Add(plant);

                var evaCoef = plant.GetCurrentStageParameters().evaporationEffect;
                soilTile.UpdateEvaporationCoefficient(evaCoef);
            }
        }
    }

    private void RemovePlant()
    {
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
        {
            SoilTile soilTile = hit.transform.GetComponent<SoilTile>();

            if (soilTile != null && soilTile.plantsOnTile.Count < MaxNumOfPlants)
            {
                var plantToRemove = soilTile.plantsOnTile.FirstOrDefault();

                if (plantToRemove != null)
                {
                    soilTile.plantsOnTile.Remove(plantToRemove);
                    Destroy(plantToRemove.gameObject);
                    
                    // removing plant drops some seeds
                    var numOfSeeds = Random.Range(0, 5);
                    inventory.AddSeed(plantToRemove.plantType, numOfSeeds);
                }
            }
        }
    }
    
    private bool IsValidPlantingPosition()
    {
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, plantLayer);
        
        return hits.Length == 0;
    }
}
