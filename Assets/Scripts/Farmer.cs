using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    [SerializeField] private GameObject _plantPrefab;
    
    public float moveSpeed = 5f;
    public LayerMask tileLayer;

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
            SeedPlant();
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
                //TODO
                Debug.Log("Watering the tile!");
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

            if (soilTile != null)
            {
                var plantGo = Instantiate(_plantPrefab, transform.position, Quaternion.identity);
                var plant = plantGo.GetComponent<Plant>();
                plant.SetTile(soilTile);
            }
        }
    }
}
