using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockPrefab;
    public int numberOfRocks = 10;
    public float spawnRadius = 10f;

    void Start()
    {
        SpawnRocks();
    }

    void SpawnRocks()
    {
        for (int i = 0; i < numberOfRocks; i++)
        {
            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            
            randomPosition.y = 0f;
            
            Instantiate(rockPrefab, randomPosition, Quaternion.identity);
        }
    }
}
