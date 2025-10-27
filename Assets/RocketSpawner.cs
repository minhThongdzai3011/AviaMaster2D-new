
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSpawner : MonoBehaviour
{
    public static RocketSpawner instance;
    
    [Header("Vật phẩm Rocket")]
    public GameObject[] rocketPrefabs;
    
    [Header("Spawn Range 2D")]
    public float spawnRangeYmin = -2f;
    public float spawnRangeYmax = 2f;
    public float spawnRangeX = -14f;

    [Header("Spawn Settings")]
    public float startDelay = 0f;
    public float spawnInterval = 0.2f;
    public int count = 0;
    public int maxRocketItems = 20;
    public float rangeXmin = 5f;
    public float rangeXmax = 10f;
    public float startRangeX = -14f;

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
            count = GameObject.FindGameObjectsWithTag("Coin").Length;
            if (count < maxRocketItems)
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
        spawnRangeX += Random.Range(rangeXmin, rangeXmax);
    }
    
    public void DeleteSpawnedItems()
    {
        GameObject[] spawnedItems = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        spawnRangeX = startRangeX; 
    }
}
