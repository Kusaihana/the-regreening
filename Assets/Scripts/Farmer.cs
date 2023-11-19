using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    [SerializeField] private GameObject _plantPrefab;
    [SerializeField] private PlantType _plantType;
    
    public Inventory inventory;
    public float moveSpeed = 5f;
    public LayerMask tileLayer;

    private const int MaxNumOfPlants = 4;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    void Update()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");
        
        var movement = new Vector3(horizontalInput, verticalInput, 0f).normalized;
        MovePlayer(movement);
        
        CheckForInteraction();
    }

    private void MovePlayer(Vector3 direction)
    {
        var movementAmount = direction * moveSpeed * Time.deltaTime;
        transform.Translate(movementAmount, Space.World);
    }
    
    private void CheckForInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WaterTile();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // check if there are enough seeds in the inventory
            if (inventory.PlantSeed(_plantType))
            {
                SeedPlant();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RemovePlant();
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

    void SeedPlant()
    {
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
        {
            SoilTile soilTile = hit.transform.GetComponent<SoilTile>();

            if (soilTile != null && soilTile.plantsOnTile.Count < MaxNumOfPlants)
            {
                var plantGo = Instantiate(_plantPrefab, transform.position, Quaternion.identity);
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
                }
            }
        }
    }
}
