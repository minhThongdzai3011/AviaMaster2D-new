
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySpawner : MonoBehaviour
{
    public static SkySpawner instance;
    
    [Header("Vật phẩm Cloud")]
    public GameObject[] cloudPrefabs;
    
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
        StartCoroutine(SpawnSkyCoroutine());
    }

    void Update()
    {

    }

    IEnumerator SpawnSkyCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        
        while (true)
        {
            count = GameObject.FindGameObjectsWithTag("sky").Length;
            if (count < maxRocketItems)
            {
                SpawnSkyItem();
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                yield return null;
            } 
        }
    }
    
    void SpawnSkyItem()
    {
        if (cloudPrefabs.Length == 0) return;
        
        int prefabIndex = Random.Range(0, cloudPrefabs.Length);
        Vector3 spawnPosition = new Vector3(spawnRangeX,
                                            Random.Range(spawnRangeYmin, spawnRangeYmax),
                                            0f); 
        
        Instantiate(cloudPrefabs[prefabIndex], spawnPosition, cloudPrefabs[prefabIndex].transform.rotation);
        spawnRangeX += Random.Range(rangeXmin, rangeXmax);
    }
    
    public void DeleteSkyItems()
    {
        GameObject[] spawnedItems = GameObject.FindGameObjectsWithTag("sky");
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        spawnRangeX = startRangeX; 
    }
}
