using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Farmer : MonoBehaviour
{
    [SerializeField] private List<PlantSpecs> _seedTypes;
    [SerializeField] private List<Image> _items;
    [SerializeField] private GameObject _bermPrefab;
    
    public Inventory inventory;
    public float moveSpeed = 5f;
    public LayerMask tileLayer;
    public LayerMask plantLayer; 
    public LayerMask waterLayer; 
    public LayerMask pebbleLayer; 

    private const int MaxNumOfPlants = 5;
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
        if (Input.GetMouseButtonDown(0))
        {
            PickUpPebble();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnItemClick(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OnItemClick(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            OnItemClick(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            OnItemClick(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            OnItemClick(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            OnItemClick(5);
        }
        
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
                    if (inventory.pebbleAmount >= 10)
                    {
                        PlaceWaterBerm();
                    }
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
    
    public void OnItemClick(int itemIndex)
    {
        _selectedItemIndex = itemIndex;
        UpdateSelectedItem();
    }

    private void PlaceWaterBerm()
    {
        var ray = new Ray(transform.position + new Vector3(0, 2, 0), -transform.up);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
        {
            SoilTile soilTile = hit.transform.GetComponent<SoilTile>();

            if (soilTile != null)
            {
                var bermPos = transform.position + transform.forward * 1f;
                Instantiate(_bermPrefab, bermPos, Quaternion.identity);

                inventory.UsePebble();
                soilTile.AddBerm();
            }
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
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, waterLayer))
        {
            inventory.FillWateringCan();
            return;
        }
        
        if (inventory.wateringCanFillAmount > 0)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
            {
                SoilTile soilTile = hit.transform.GetComponent<SoilTile>();

                if (soilTile != null)
                {
                    soilTile.UpdateWater(10);
                    inventory.WaterTile(10);
                }
            }        
        }
    }

    void SeedPlant(PlantSpecs seedType)
    {
        var ray = new Ray(transform.position + new Vector3(0, 2, 0), -transform.up);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
        {
            SoilTile soilTile = hit.transform.GetComponent<SoilTile>();
            var plantPos = transform.position + transform.forward * 1f;

            if (!IsPlantAtPosition(plantPos))
            {
                if (soilTile != null && soilTile.plantsOnTile.Count < MaxNumOfPlants)
                {
                    var plantGo = Instantiate(seedType.plantPrefab, plantPos, Quaternion.identity);
                    var plant = plantGo.GetComponent<Plant>();
                    plant.SetTile(soilTile);
                    soilTile.plantsOnTile.Add(plant);

                    var evaCoef = plant.GetCurrentStageParameters().evaporationEffect;
                    soilTile.UpdateEvaporationCoefficient(evaCoef);
                    
                    inventory.seeds[seedType.commonName]--;
                    inventory.UpdateSeedText(seedType.commonName);
                }
            }
            else
            {
                Debug.Log("Cannot plant on top of an existing plant!");
            }
        }
    }
    
    private bool IsPlantAtPosition(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.5f, plantLayer);
        return colliders.Length > 0;
    }

    private void RemovePlant()
    {
        var ray = new Ray(transform.position + new Vector3(0, 2, 0), -transform.up);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
        {
            SoilTile soilTile = hit.transform.GetComponent<SoilTile>();

            if (soilTile != null)
            {
                var closestPlant = FindClosestPlant(soilTile);

                if (closestPlant != null)
                {
                    soilTile.plantsOnTile.Remove(closestPlant);
                    Destroy(closestPlant.gameObject);
                    
                    var numOfSeeds = Random.Range(0, 5);
                    inventory.AddSeed(closestPlant.plantSpec, numOfSeeds);
                }
            }
        }
    }
    
    private void PickUpPebble()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 1f, transform.forward, Mathf.Infinity, pebbleLayer);
        
        Pebble closestPebble = FindClosestPebble(hits);

        if(closestPebble != null)
        {
            Destroy(closestPebble.gameObject);
            inventory.AddPebble();
        }
    }
    
    private Pebble FindClosestPebble(RaycastHit[] hits)
    {
        Pebble closestRock = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            Pebble rock = hit.transform.GetComponent<Pebble>();

            if (rock != null)
            {
                float distance = Vector3.Distance(transform.position, rock.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestRock = rock;
                }
            }
        }

        return closestRock;
    }

    private Plant FindClosestPlant(SoilTile soilTile)
    {
        Plant closestPlant = null;
        float closestDistance = float.MaxValue;

        foreach (var plant in soilTile.plantsOnTile)
        {
            float distance = Vector3.Distance(transform.position, plant.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlant = plant;
            }
        }

        return closestPlant;
    }
    
    private Plant FindClosestPebble(SoilTile soilTile)
    {
        Plant closestPlant = null;
        float closestDistance = float.MaxValue;

        foreach (var plant in soilTile.plantsOnTile)
        {
            float distance = Vector3.Distance(transform.position, plant.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlant = plant;
            }
        }

        return closestPlant;
    }
    
    private bool IsValidPlantingPosition()
    {
        var ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, plantLayer);
        
        return hits.Length == 0;
    }
}
