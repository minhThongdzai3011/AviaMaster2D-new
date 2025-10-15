
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSpawner : MonoBehaviour
{
    public static RocketSpawner instance;
    
    [Header("Vật phẩm Rocket")]
    public GameObject[] rocketPrefabs;
    
    [Header("Spawn Range 2D")]
    private float spawnRangeYmin = -2f;
    private float spawnRangeYmax = 2f;
    private float spawnRangeX = 12f;
    
    [Header("Spawn Settings")]
    private float startDelay = 0f;
    private float spawnInterval = 0.2f;
    public int count = 0;
    
    void Start()
    {
        instance = this;
        StartCoroutine(SpawnRocketCoroutine());
    }

    void Update()
    {

    }

    IEnumerator SpawnRocketCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        
        while (true)
        {
            count = GameObject.FindGameObjectsWithTag("Rocket").Length;
            if (count < 20)
            {
                SpawnRocketItem();
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                yield return null;
            } 
        }
    }
    
    void SpawnRocketItem()
    {
        if (rocketPrefabs.Length == 0) return;
        
        int prefabIndex = Random.Range(0, rocketPrefabs.Length);
        Vector3 spawnPosition = new Vector3(spawnRangeX,
                                            Random.Range(spawnRangeYmin, spawnRangeYmax),
                                            0f); 
        
        Instantiate(rocketPrefabs[prefabIndex], spawnPosition, rocketPrefabs[prefabIndex].transform.rotation);
        spawnRangeX += 5f; 
    }
    
    public void DeleteSpawnedItems()
    {
        GameObject[] spawnedItems = GameObject.FindGameObjectsWithTag("Rocket");
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        spawnRangeX = 12f; 
    }
}
