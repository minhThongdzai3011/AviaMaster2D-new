
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
    public int mapCityIndex = 10;
    public bool isCityMap = false;
    public GameObject mapCityBeachPrefab;
    public GameObject mapCityStartPrefab;
    public GameObject[] mapCityPrefabs;

    [Header("MapBeach")]
    public int mapBeachIndex = 20;
    public bool isBeachMap = false;
    public bool isBeachStartSpawned = false;
    public GameObject mapBeachDesertPrefab;
    public GameObject mapBeachStartPrefab;
    public GameObject[] mapBeachPrefabs;

    [Header("MapDesert")]
    public int mapDesertIndex = 30;
    public bool isDesertMap = false;
    public bool isDesertStartSpawned = false;
    public GameObject mapDesertFieldPrefab;
    public GameObject mapDesertStartPrefab;
    public GameObject[] mapDesertPrefabs;

    [Header("MapField")]
    public int mapFieldIndex = 40;
    public bool isFieldMap = false;
    public bool isFieldStartSpawned = false;
    public GameObject mapFieldIcePrefab;
    public GameObject mapFieldStartPrefab;
    public GameObject[] mapFieldPrefabs;

    [Header("MapIce")]
    public int mapIceIndex = 50;
    public bool isIceMap = false;
    public bool isIceStartSpawned = false;
    public GameObject mapIceLavaPrefab;
    public GameObject mapIceStartPrefab;
    public GameObject[] mapIcePrefabs;

    [Header("MapLava")]
    public int mapLavaIndex = 60;
    public bool isLavaMap = false;
    public bool isLavaStartSpawned = false;
    public GameObject mapLavaStartPrefab;
    public GameObject[] mapLavaPrefabs;

    [Header("Spawn Range 2D")]
    public float spawnRangeYmin = -1.5f;
    private float spawnRangeYBeachMin = 3783.31f;
    public float spawnRangeYDesertMin = 1165.4051f;
    public float spawnRangeYFieldMin = 1165.4051f;
    public float spawnRangeYIceMin = 1165.4051f;
    public float spawnRangeYLavaMin = 1165.4051f;
    public float spawnRangeX = 12f;

    [Header("Spawn Settings")]
    private float startDelay = 0f;
    private float spawnInterval = 0.05f;
    public int count = 0;

    [Header("Kiem tra Map da Unlock")]
    public bool isMapCityUnlocked = true;
    public bool isMapBeachUnlocked = true;
    public bool isMapDesertUnlocked = false;
    public bool isMapFieldUnlocked = false;
    public bool isMapIceUnlocked = false;
    public bool isMapLavaUnlocked = false;

    [Header("Spawned Map Start")]
    public GameObject[] mapStart;
    
    void Start()
    {
        instance = this;
        // mapPrefabs = mapCityPrefabs;
        // isCityMap = true;
        if (isMapCityUnlocked && !isMapBeachUnlocked)
        {
            mapPrefabs = mapBeachPrefabs;
            isBeachMap = true;
        }
        if (isMapBeachUnlocked && !isMapDesertUnlocked)
        {
            int isRandomIndexMapStart = Random.Range(0, 2);
            if (isRandomIndexMapStart == 0)
            {
                mapPrefabs = mapCityPrefabs;
                isCityMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else
            {
                mapPrefabs = mapBeachPrefabs;
                isBeachMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
        }
        if (isMapDesertUnlocked && !isMapFieldUnlocked)
        {
            int isRandomIndexMapStart = Random.Range(0, 3);
            if (isRandomIndexMapStart == 0)
            {
                mapPrefabs = mapCityPrefabs;
                isCityMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else if (isRandomIndexMapStart == 1)
            {
                mapPrefabs = mapBeachPrefabs;
                isBeachMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else
            {
                mapPrefabs = mapDesertPrefabs;
                isDesertMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
        }
        if (isMapFieldUnlocked && !isMapIceUnlocked)
        {
            int isRandomIndexMapStart = Random.Range(0, 4);
            if (isRandomIndexMapStart == 0)
            {
                mapPrefabs = mapCityPrefabs;
                isCityMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else if (isRandomIndexMapStart == 1)
            {
                mapPrefabs = mapBeachPrefabs;
                isBeachMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else if (isRandomIndexMapStart == 2)
            {
                mapPrefabs = mapDesertPrefabs;
                isDesertMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else
            {
                mapPrefabs = mapFieldPrefabs;
                isFieldMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
        }
        if (isMapIceUnlocked && !isMapLavaUnlocked)
        {
            int isRandomIndexMapStart = Random.Range(0, 5);
            if (isRandomIndexMapStart == 0)
            {
                mapPrefabs = mapCityPrefabs;
                isCityMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else if (isRandomIndexMapStart == 1)
            {
                mapPrefabs = mapBeachPrefabs;
                isBeachMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else if (isRandomIndexMapStart == 2)
            {
                mapPrefabs = mapDesertPrefabs;
                isDesertMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else if (isRandomIndexMapStart == 3)
            {
                mapPrefabs = mapFieldPrefabs;
                isFieldMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else
            {
                mapPrefabs = mapIcePrefabs;
                isIceMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
        }
        if (isMapLavaUnlocked)
        {
            int isRandomIndexMapStart = Random.Range(0, 6);
            if (isRandomIndexMapStart == 0)
            {
                mapPrefabs = mapCityPrefabs;
                isCityMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else if (isRandomIndexMapStart == 1)
            {
                mapPrefabs = mapBeachPrefabs;
                isBeachMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else if (isRandomIndexMapStart == 2)
            {
                mapPrefabs = mapDesertPrefabs;
                isDesertMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else if (isRandomIndexMapStart == 3)
            {
                mapPrefabs = mapFieldPrefabs;
                isFieldMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else if (isRandomIndexMapStart == 4)
            {
                mapPrefabs = mapIcePrefabs;
                isIceMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
            else
            {
                mapPrefabs = mapLavaPrefabs;
                isLavaMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
            }
        }
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
            Instantiate(mapCityBeachPrefab, new Vector3(spawnRangeX, -32.1f, 0f), mapBeachStartPrefab.transform.rotation);
            spawnRangeX += 30f;

            Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
            isBeachMap = false;
            isBeachStartSpawned = true;
            spawnRangeX += 30f;
            return;
        }
        else if (isDesertMap)
        {
            Instantiate(mapBeachDesertPrefab, new Vector3(spawnRangeX, -32.1f, 0f), mapBeachStartPrefab.transform.rotation);
            spawnRangeX += 30f;

            Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
            isDesertMap = false;
            isBeachStartSpawned = false;
            isDesertStartSpawned = true;
            spawnRangeX += 30f;
            return;
        }
        else if (isFieldMap)
        {
            Instantiate(mapDesertFieldPrefab, new Vector3(spawnRangeX, -32.1f, 0f), mapBeachStartPrefab.transform.rotation);
            spawnRangeX += 30f;
            
            Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
            isFieldMap = false;
            isDesertStartSpawned = false;
            isFieldStartSpawned = true;
            spawnRangeX += 30f;
            return;
        }
        else if (isIceMap)
        {
            Instantiate(mapFieldIcePrefab, new Vector3(spawnRangeX, -32.1f, 0f), mapBeachStartPrefab.transform.rotation);
            spawnRangeX += 30f;
            
            Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
            isIceMap = false;
            isFieldStartSpawned = false;
            isIceStartSpawned = true;
            spawnRangeX += 30f;
            return;
        }
        else if (isLavaMap)
        {
            Instantiate(mapIceLavaPrefab, new Vector3(spawnRangeX, -32.1f, 0f), mapBeachStartPrefab.transform.rotation);
            spawnRangeX += 30f;

            Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
            isLavaMap = false;
            isIceStartSpawned = false;
            isLavaStartSpawned = true;
            spawnRangeX += 30f;
            return;
        }

        int prefabIndex = Random.Range(0, mapPrefabs.Length);
        if (isBeachStartSpawned)
        {
            spawnRangeYmin = spawnRangeYBeachMin;
        }
        else if (isDesertStartSpawned)
        {
            spawnRangeYmin = spawnRangeYDesertMin;
        }
        else if (isFieldStartSpawned)
        {
            spawnRangeYmin = spawnRangeYFieldMin;
        }
        else if (isIceStartSpawned)
        {
            spawnRangeYmin = spawnRangeYIceMin;
        }
        else if (isLavaStartSpawned)
        {
            spawnRangeYmin = spawnRangeYLavaMin;
        }

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
