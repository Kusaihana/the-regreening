using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Farmer : MonoBehaviour
{
    [SerializeField] private List<PlantType> _seedTypes;
    [SerializeField] private List<Image> _items;
    
    public Inventory inventory;
    public float moveSpeed = 5f;
    public LayerMask tileLayer;
    public LayerMask plantLayer; 

    private const int MaxNumOfPlants = 4;
    private int _selectedItemIndex;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        UpdateSelectedItem();
    }

    void Update()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");
        
        var movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        MovePlayer(movement);
        
        CheckForInteraction();
    }

    private void MovePlayer(Vector3 movement)
    {
        var movementAmount = movement * moveSpeed * Time.deltaTime;
        transform.Translate(movementAmount, Space.World);
        
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
    }
    
    private void CheckForInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (_selectedItemIndex)
            {
                case 0:
                    WaterTile();
                    break;
                case 1:
                {
                    // check if there are enough seeds in the inventory
                    if (IsValidPlantingPosition() && inventory.PlantSeed(_seedTypes[0]))
                    {
                        SeedPlant(_seedTypes[0]);
                    }

                    break;
                }
                case 2:
                {
                    // check if there are enough seeds in the inventory
                    if (IsValidPlantingPosition() && inventory.PlantSeed(_seedTypes[1]))
                    {
                        SeedPlant(_seedTypes[1]);
                    }

                    break;
                }
                case 3:
                {
                    // check if there are enough seeds in the inventory
                    if (IsValidPlantingPosition() && inventory.PlantSeed(_seedTypes[2]))
                    {
                        SeedPlant(_seedTypes[2]);
                    }

                    break;
                }
                case 4:
                {
                    // check if there are enough seeds in the inventory
                    if (IsValidPlantingPosition() && inventory.PlantSeed(_seedTypes[3]))
                    {
                        SeedPlant(_seedTypes[3]);
                    }

                    break;
                }
                case 5:
                    //TODO water berm
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q)) //TODO
        {
            RemovePlant();
        }
        
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        if (scrollWheel > 0f)
        {
            // Scroll up
            SelectNextItem();
        }
        else if (scrollWheel < 0f)
        {
            // Scroll down
            SelectPreviousItem();
        }
    }
    
    private void SelectNextItem()
    {
        _selectedItemIndex = (_selectedItemIndex + 1) % _items.Count;

        UpdateSelectedItem();
    }

    private void SelectPreviousItem()
    {
        _selectedItemIndex = (_selectedItemIndex - 1 + 6) % _items.Count;

        UpdateSelectedItem();
    }

    private void UpdateSelectedItem()
    {
        foreach (var item in _items)
        {
            item.color = Color.white;
        }

        // Enable the selected item
        _items[_selectedItemIndex].color = Color.green;
    }

    void WaterTile()
    {
        var ray = new Ray(transform.position + new Vector3(0, 2, 0), -transform.up);
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
        var ray = new Ray(transform.position + new Vector3(0, 2, 0), -transform.up);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
        {
            SoilTile soilTile = hit.transform.GetComponent<SoilTile>();

            if (soilTile != null && soilTile.plantsOnTile.Count < MaxNumOfPlants)
            {
                var plantPos = transform.position + transform.forward * 1f;
                var plantGo = Instantiate(seedType.plantPrefab, plantPos, Quaternion.identity);
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
        var ray = new Ray(transform.position + new Vector3(0, 2, 0), -transform.up);
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
