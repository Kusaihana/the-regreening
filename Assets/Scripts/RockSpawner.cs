using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockPrefab;
    public int numberOfRocks = 10;
    public float spawnRadius = 10f;
    public int currentRocks;

    void Start()
    {
        SpawnRocks();
        InvokeRepeating("SpawnOneRock", 60f, 60f);
    }

    void SpawnRocks()
    {
        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            
            randomPosition.y = 0f;
            
            Instantiate(rockPrefab, randomPosition, Quaternion.identity);
            currentRocks++;
        }
    }
    
    void SpawnOneRock()
    {
       if(currentRocks < 50)
       {
           Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            
           randomPosition.y = 0f;
            
           Instantiate(rockPrefab, randomPosition, Quaternion.identity);
           currentRocks++;
       }
    }
}
