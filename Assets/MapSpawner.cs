
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public static MapSpawner instance;
    
    [Header("Vật phẩm Map")]
    public GameObject[] mapPrefabs;
    
    [Header("Spawn Range 2D")]
    public float spawnRangeYmin = -1.5f;
    public float spawnRangeX = 12f;

    [Header("Spawn Settings")]
    private float startDelay = 0f;
    private float spawnInterval = 0.1f;
    public int count = 0;
    
    void Start()
    {
        instance = this;
        StartCoroutine(SpawnMapCoroutine());
    }

    void Update()
    {

    }

    IEnumerator SpawnMapCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        
        while (true)
        {
            count = GameObject.FindGameObjectsWithTag("Map").Length;
            if (count < 6)
            {
                SpawnMapItem();
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                yield return null;
            } 
        }
    }

    void SpawnMapItem()
    {
        if (mapPrefabs.Length == 0) return;

        int prefabIndex = Random.Range(0, mapPrefabs.Length);
        Vector3 spawnPosition = new Vector3(spawnRangeX,
                                            spawnRangeYmin,
                                            0f); 

        Instantiate(mapPrefabs[prefabIndex], spawnPosition, mapPrefabs[prefabIndex].transform.rotation);
        spawnRangeX += 30f; 
    }
    
    public void DeleteSpawnedItems()
    {
        GameObject[] spawnedItems = GameObject.FindGameObjectsWithTag("Map");
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        spawnRangeX = -12f; 
    }
}
