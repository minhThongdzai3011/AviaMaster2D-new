
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    public static MapSpawner instance;

    [Header("Vật phẩm Map Chung")]
    public int mapIndex = 0;
    public GameObject mapStartPrefab;
    public GameObject[] mapPrefabs;

    [Header("MapCity")]
    public int mapCityIndex = 5;
    public bool isCityMap = false;
    public GameObject mapCityStartPrefab;
    public GameObject[] mapCityPrefabs;

    [Header("MapBeach")]
    public int mapBeachIndex = 10;
    public bool isBeachMap = false;
    public GameObject mapBeachStartPrefab;
    public GameObject[] mapBeachPrefabs;

    [Header("MapDesert")]
    public int mapDesertIndex = 15;
    public bool isDesertMap = false;
    public GameObject mapDesertStartPrefab;
    public GameObject[] mapDesertPrefabs;

    [Header("MapField")]
    public int mapFieldIndex = 20;
    public bool isFieldMap = false;
    public GameObject mapFieldStartPrefab;
    public GameObject[] mapFieldPrefabs;

    [Header("MapIce")]
    public int mapIceIndex = 25;
    public bool isIceMap = false;
    public GameObject mapIceStartPrefab;
    public GameObject[] mapIcePrefabs;

    [Header("MapLava")]
    public int mapLavaIndex = 30;
    public bool isLavaMap = false;
    public GameObject mapLavaStartPrefab;
    public GameObject[] mapLavaPrefabs;

    [Header("Spawn Range 2D")]
    public float spawnRangeYmin = -1.5f;
    public float spawnRangeX = 12f;

    [Header("Spawn Settings")]
    private float startDelay = 0f;
    private float spawnInterval = 0.05f;
    public int count = 0;
    
    void Start()
    {
        instance = this;
        StartCoroutine(SpawnMapCoroutine());
        mapPrefabs = mapCityPrefabs;
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
                CheckMap();
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

        if (isBeachMap)
        {
            Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, spawnRangeYmin + (-2.7f), 0f), mapBeachStartPrefab.transform.rotation);
            isBeachMap = false;
            spawnRangeX += 30f;
            return;
        }
        else if (isDesertMap)
        {
            Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, spawnRangeYmin + (-2.7f), 0f), mapDesertStartPrefab.transform.rotation);
            isDesertMap = false;
            spawnRangeX += 30f;
            return;
        }
        else if (isFieldMap)
        {
            Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, spawnRangeYmin + (-2.7f), 0f), mapFieldStartPrefab.transform.rotation);
            isFieldMap = false;
            spawnRangeX += 30f;
            return;
        }
        else if (isIceMap)
        {
            Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, spawnRangeYmin + (-2.7f), 0f), mapIceStartPrefab.transform.rotation);
            isIceMap = false;
            spawnRangeX += 30f;
            return;
        }
        else if (isLavaMap)
        {
            Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, spawnRangeYmin + (-2.7f), 0f), mapLavaStartPrefab.transform.rotation);
            isLavaMap = false;
            spawnRangeX += 30f;
            return;
        }

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
        Reset();

    }

    public void CheckMap()
    {
        mapIndex++;
        if (mapIndex == mapCityIndex)
        {
            mapPrefabs = mapBeachPrefabs;
            isBeachMap = true;
        }
        else if (mapIndex == mapBeachIndex)
        {
            mapPrefabs = mapDesertPrefabs;
            isDesertMap = true;
        }
        else if (mapIndex == mapDesertIndex)
        {
            mapPrefabs = mapFieldPrefabs;
            isFieldMap = true;
        }
        else if (mapIndex == mapFieldIndex)
        {
            mapPrefabs = mapIcePrefabs;
            isIceMap = true;
        }
        else if (mapIndex == mapIceIndex)
        {
            mapPrefabs = mapLavaPrefabs;
            isLavaMap = true;
        }
    }


    public void Reset()
    {
        mapIndex = 0;
        mapPrefabs = mapCityPrefabs;
    }

}
