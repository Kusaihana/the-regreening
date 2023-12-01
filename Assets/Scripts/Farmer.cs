using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Farmer : MonoBehaviour
{
    [SerializeField] private List<PlantSpecs> _seedTypes;
    [SerializeField] private List<TMP_Text> _items;
    [SerializeField] private GameObject _bermPrefab;
    [SerializeField] private GameObject _scaleCursor;
    [SerializeField] private Animator _animator;
    [SerializeField] public GameObject _waterParticlesPrefab; 

    public Inventory inventory;
    public float moveSpeed = 5f;
    public LayerMask tileLayer;
    public LayerMask plantLayer; 
    public LayerMask waterLayer; 
    public LayerMask pebbleLayer;

    private const int MaxNumOfPlants = 8;
    private int _selectedItemIndex;
    private bool _waterDialogShown;
    private LandType _currentLandType = LandType.Desert;
    private DialogManager _dialogManager;
    
    public float minX = -5f;
    public float maxX = 5f;
    public float minZ = -5f;
    public float maxZ = 5f;


    private void Awake()
    {
        inventory = GetComponent<Inventory>();
        _dialogManager = FindObjectOfType<DialogManager>();
        UpdateSelectedItem();
    }

    void Update()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");
        
        var movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        MovePlayer(movement);
        
        CheckForInteraction();
        
        var ray = new Ray(transform.position + new Vector3(0, 2, 0), -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
        {
            SoilTile soilTile = hit.transform.GetComponent<SoilTile>();

            if (soilTile != null)
            {
                var newLandType = soilTile.landType;

                if (_currentLandType != newLandType)
                {
                    var newRotation = newLandType switch
                    {
                        LandType.Alive => -45f,
                        LandType.Vivid => -90f,
                        LandType.Lush => -135f,
                        _ => 0f
                    };

                    _currentLandType = newLandType;
                    
                    StartCoroutine(RotateCursorOverTime(newRotation));
                }
            }
        }
    }

    private void MovePlayer(Vector3 movement)
    {
        var movementAmount = movement * moveSpeed * Time.deltaTime;
        var newX = Mathf.Clamp(transform.position.x + movementAmount.x, minX, maxX);
        var newZ = Mathf.Clamp(transform.position.z + movementAmount.z, minZ, maxZ);

        transform.position = new Vector3(newX, transform.position.y, newZ);

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
            _animator.SetFloat("Walking", 1f);
        }
        else
        {
            _animator.SetFloat("Walking", 0f);
        }
    }
    
    private void CheckForInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PickUpPebble();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RemovePlant();
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
            item.color = new Color(243/255f,231/255f,187/255f);
        }

        // Enable the selected item
        if (_selectedItemIndex >= 0 && _selectedItemIndex < _items.Count)
        {
            _items[_selectedItemIndex].color = new Color(100/255f,200/255f,100/255f);
        }
    }

    void WaterTile()
    {
        var ray = new Ray(transform.position + new Vector3(0, 2, 0), -transform.up);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, waterLayer))
        {
            if (!_waterDialogShown)
            {
                _dialogManager.StartDialogue("Narrative/Dialog1_2.txt");
                _waterDialogShown = true;
            }
            inventory.FillWateringCan();
            SpawnWaterParticles(transform.position + new Vector3(1.8f, 0f, 1f));
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
                    SpawnWaterParticles(transform.position + new Vector3(1.8f, 0f, 0f));
                }
            }        
        }
    }
    
    private void SpawnWaterParticles(Vector3 pos)
    {
        if (_waterParticlesPrefab != null)
        {
            GameObject particles = Instantiate(_waterParticlesPrefab, pos, Quaternion.identity);
            
            particles.transform.parent = transform;
            
            particles.GetComponent<ParticleSystem>().Play();
            
            var particleDuration = particles.GetComponent<ParticleSystem>().main.duration;

            Destroy(particles, particleDuration);
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
    
    private IEnumerator RotateCursorOverTime(float newRotation)
    {
        var elapsedTime = 0f;

        var startAngle = _scaleCursor.transform.rotation.z;

        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            
            float t = Mathf.Clamp01(elapsedTime / 0.5f);
            
            Quaternion startRotation = Quaternion.Euler(0f, 0f, startAngle);
            Quaternion endRotation = Quaternion.Euler(0f, 0f, newRotation);
            Quaternion interpolatedRotation = Quaternion.Lerp(startRotation, endRotation, t);
            
            _scaleCursor.transform.rotation = interpolatedRotation;

            yield return null;
        }
    }
}
