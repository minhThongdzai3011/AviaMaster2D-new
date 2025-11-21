
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
    public int mapCityIndex = 0;
    public bool isCityMap = false;
    public bool isCityStartSpawned = false;
    public GameObject mapCityBeachPrefab;
    public GameObject mapCityStartPrefab;
    public GameObject[] mapCityPrefabs;

    [Header("MapBeach")]
    public int mapBeachIndex = 0;
    public bool isBeachMap = false;
    public bool isBeachStartSpawned = false;
    public GameObject mapBeachDesertPrefab;
    public GameObject mapBeachStartPrefab;
    public GameObject[] mapBeachPrefabs;

    [Header("MapDesert")]
    public int mapDesertIndex = 0;
    public bool isDesertMap = false;
    public bool isDesertStartSpawned = false;
    public GameObject mapDesertFieldPrefab;
    public GameObject mapDesertStartPrefab;
    public GameObject[] mapDesertPrefabs;

    [Header("MapField")]
    public int mapFieldIndex = 0;
    public bool isFieldMap = false;
    public bool isFieldStartSpawned = false;
    public GameObject mapFieldIcePrefab;
    public GameObject mapFieldStartPrefab;
    public GameObject[] mapFieldPrefabs;

    [Header("MapIce")]
    public int mapIceIndex = 0;
    public bool isIceMap = false;
    public bool isIceStartSpawned = false;
    public GameObject mapIceLavaPrefab;
    public GameObject mapIceStartPrefab;
    public GameObject[] mapIcePrefabs;

    [Header("MapLava")]
    public int mapLavaIndex = 0;
    public bool isLavaMap = false;
    public bool isLavaStartSpawned = false;
    public GameObject mapLavaStartPrefab;
    public GameObject[] mapLavaPrefabs;

    [Header("Spawn Range 2D")]
    public float spawnRangeYmin = -1.5f;
    public float spawnRangeYCityMin = -1.5f;
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
    public int[] mapSpawnIndex = {10,20,30,40,50,60};
    List<int> mapList = new List<int>() {0,1,2,3,4,5};

    [Header("Kiem tra MapStart")]
    public bool checkMapStartCitySpawned = false;
    public bool checkMapStartBeachSpawned = false;
    public bool checkMapStartDesertSpawned = false;
    public bool checkMapStartFieldSpawned = false;
    public bool checkMapStartIceSpawned = false;
    public bool checkMapStartLavaSpawned = false;
    
    void Start()
    {
        instance = this;
        
        if (isMapCityUnlocked && !isMapBeachUnlocked)
        {
            mapPrefabs = mapCityPrefabs;
            isCityMap = true;
            mapStart[0].SetActive(true);
            checkMapStartCitySpawned = true;
        }
        if (isMapBeachUnlocked && !isMapDesertUnlocked)
        {
            int isRandomIndexMapStart = Random.Range(0, 2);
            if (isRandomIndexMapStart == 0)
            {
                mapPrefabs = mapCityPrefabs;
                isCityMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartCitySpawned = true;
            }
            else
            {
                mapPrefabs = mapBeachPrefabs;
                isBeachMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartBeachSpawned = true;
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
                checkMapStartCitySpawned = true;
            }
            else if (isRandomIndexMapStart == 1)
            {
                mapPrefabs = mapBeachPrefabs;
                isBeachMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartBeachSpawned = true;
            }
            else
            {
                mapPrefabs = mapDesertPrefabs;
                isDesertMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartDesertSpawned = true;
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
                checkMapStartCitySpawned = true;
            }
            else if (isRandomIndexMapStart == 1)
            {
                mapPrefabs = mapBeachPrefabs;
                isBeachMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartBeachSpawned = true;
            }
            else if (isRandomIndexMapStart == 2)
            {
                mapPrefabs = mapDesertPrefabs;
                isDesertMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartDesertSpawned = true;
            }
            else
            {
                mapPrefabs = mapFieldPrefabs;
                isFieldMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartFieldSpawned = true;
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
                checkMapStartCitySpawned = true;
            }
            else if (isRandomIndexMapStart == 1)
            {
                mapPrefabs = mapBeachPrefabs;
                isBeachMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartBeachSpawned = true;
            }
            else if (isRandomIndexMapStart == 2)
            {
                mapPrefabs = mapDesertPrefabs;
                isDesertMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartDesertSpawned = true;
            }
            else if (isRandomIndexMapStart == 3)
            {
                mapPrefabs = mapFieldPrefabs;
                isFieldMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartFieldSpawned = true;
            }
            else
            {
                mapPrefabs = mapIcePrefabs;
                isIceMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartIceSpawned = true;
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
                checkMapStartCitySpawned = true;
            }
            else if (isRandomIndexMapStart == 1)
            {
                mapPrefabs = mapBeachPrefabs;
                isBeachMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartBeachSpawned = true;
            }
            else if (isRandomIndexMapStart == 2)
            {
                mapPrefabs = mapDesertPrefabs;
                isDesertMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartDesertSpawned = true;
            }
            else if (isRandomIndexMapStart == 3)
            {
                mapPrefabs = mapFieldPrefabs;
                isFieldMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartFieldSpawned = true;
            }
            else if (isRandomIndexMapStart == 4)
            {
                mapPrefabs = mapIcePrefabs;
                isIceMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartIceSpawned = true;
            }
            else
            {
                mapPrefabs = mapLavaPrefabs;
                isLavaMap = true;
                mapStart[isRandomIndexMapStart].SetActive(true);
                checkMapStartLavaSpawned = true;
            }
        }
        RandomMap();
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

        // *** SỬA: Logic transition spawning - KHÔNG tắt map flags ***
        bool needSpawnTransition = false;
        
        if (isCityMap && !checkMapStartCitySpawned)
        {
            // Spawn Beach transition và start cho City cycle
            Instantiate(mapCityBeachPrefab, new Vector3(spawnRangeX, -32.1f, 0f), mapCityBeachPrefab.transform.rotation);
            spawnRangeX += 30f;
            Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
            spawnRangeX += 30f;
            checkMapStartCitySpawned = true;
            needSpawnTransition = true;
            Debug.Log("Spawned City->Beach transition");
        }
        else if (isBeachMap && !checkMapStartBeachSpawned)
        {
            // Spawn Desert transition và start cho Beach cycle  
            Instantiate(mapBeachDesertPrefab, new Vector3(spawnRangeX, -32.1f, 0f), mapBeachDesertPrefab.transform.rotation);
            spawnRangeX += 30f;
            Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
            spawnRangeX += 30f;
            checkMapStartBeachSpawned = true;
            needSpawnTransition = true;
            Debug.Log("Spawned Beach->Desert transition");
        }
        else if (isDesertMap && !checkMapStartDesertSpawned)
        {
            // Spawn Field transition và start cho Desert cycle
            Instantiate(mapDesertFieldPrefab, new Vector3(spawnRangeX, -32.1f, 0f), mapDesertFieldPrefab.transform.rotation);
            spawnRangeX += 30f;
            Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
            spawnRangeX += 30f;
            checkMapStartDesertSpawned = true;
            needSpawnTransition = true;
            Debug.Log("Spawned Desert->Field transition");
        }
        else if (isFieldMap && !checkMapStartFieldSpawned)
        {
            // Spawn Ice transition và start cho Field cycle
            Instantiate(mapFieldIcePrefab, new Vector3(spawnRangeX, -32.1f, 0f), mapFieldIcePrefab.transform.rotation);
            spawnRangeX += 30f;
            Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
            spawnRangeX += 30f;
            checkMapStartFieldSpawned = true;
            needSpawnTransition = true;
            Debug.Log("Spawned Field->Ice transition");
        }
        else if (isIceMap && !checkMapStartIceSpawned)
        {
            // Spawn Lava transition và start cho Ice cycle
            Instantiate(mapIceLavaPrefab, new Vector3(spawnRangeX, -32.1f, 0f), mapIceLavaPrefab.transform.rotation);
            spawnRangeX += 30f;
            Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
            spawnRangeX += 30f;
            checkMapStartIceSpawned = true;
            needSpawnTransition = true;
            Debug.Log("Spawned Ice->Lava transition");
        }
        else if (isLavaMap && !checkMapStartLavaSpawned)
        {
            // Lava không có transition tiếp theo, chỉ đánh dấu
            checkMapStartLavaSpawned = true;
            Debug.Log("Lava map - no next transition");
        }
        
        // *** QUAN TRỌNG: Nếu vừa spawn transition, return để không spawn map ngay lập tức ***
        if (needSpawnTransition)
        {
            return;
        }

        // *** SỬA: Set Y range dựa trên map type hiện tại (KHÔNG dựa trên StartSpawned) ***
        if (isCityMap)
        {
            spawnRangeYmin = spawnRangeYCityMin; // -1.5f
            Debug.Log("City Map - Y: " + spawnRangeYmin);
        }
        else if (isBeachMap)
        {
            spawnRangeYmin = spawnRangeYBeachMin; // 3783.31f
            Debug.Log("Beach Map - Y: " + spawnRangeYmin);
        }
        else if (isDesertMap)
        {
            spawnRangeYmin = spawnRangeYDesertMin; // 1165.4051f
            Debug.Log("Desert Map - Y: " + spawnRangeYmin);
        }
        else if (isFieldMap)
        {
            spawnRangeYmin = spawnRangeYFieldMin;
            Debug.Log("Field Map - Y: " + spawnRangeYmin);
        }
        else if (isIceMap)
        {
            spawnRangeYmin = spawnRangeYIceMin;
            Debug.Log("Ice Map - Y: " + spawnRangeYmin);
        }
        else if (isLavaMap)
        {
            spawnRangeYmin = spawnRangeYLavaMin;
            Debug.Log("Lava Map - Y: " + spawnRangeYmin);
        }
        else
        {
            // Fallback: dựa trên StartSpawned flags
            if(isCityStartSpawned)
            {
                spawnRangeYmin = spawnRangeYCityMin;
                Debug.Log("City StartSpawned - Y: " + spawnRangeYmin);
            }
            else if (isBeachStartSpawned)
            {
                spawnRangeYmin = spawnRangeYBeachMin;
                Debug.Log("Beach StartSpawned - Y: " + spawnRangeYmin);
            }
            else if (isDesertStartSpawned)
            {
                spawnRangeYmin = spawnRangeYDesertMin;
                Debug.Log("Desert StartSpawned - Y: " + spawnRangeYmin);
            }
            else if (isFieldStartSpawned)
            {
                spawnRangeYmin = spawnRangeYFieldMin;
                Debug.Log("Field StartSpawned - Y: " + spawnRangeYmin);
            }
            else if (isIceStartSpawned)
            {
                spawnRangeYmin = spawnRangeYIceMin;
                Debug.Log("Ice StartSpawned - Y: " + spawnRangeYmin);
            }
            else if (isLavaStartSpawned)
            {
                spawnRangeYmin = spawnRangeYLavaMin;
                Debug.Log("Lava StartSpawned - Y: " + spawnRangeYmin);
            }
        }

        // *** SPAWN ACTUAL MAP ITEMS ***
        int prefabIndex = Random.Range(0, mapPrefabs.Length);
        Vector3 spawnPosition = new Vector3(spawnRangeX, spawnRangeYmin, 0f);
        
        GameObject spawnedMap = Instantiate(mapPrefabs[prefabIndex], spawnPosition, mapPrefabs[prefabIndex].transform.rotation);
        spawnRangeX += 30f;
        
        // Debug để kiểm tra
        string mapType = isCityMap ? "City" : isBeachMap ? "Beach" : isDesertMap ? "Desert" : 
                         isFieldMap ? "Field" : isIceMap ? "Ice" : isLavaMap ? "Lava" : "Unknown";
        Debug.Log($"Spawned {mapType} map: {spawnedMap.name} at Y: {spawnPosition.y}");
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
        if (mapIndex == mapSpawnIndex[1])
        {
            ResetAllMapFlags(); // Reset trước khi chuyển map mới
            switch (tempSpawnList[0])
            {
                case 0:
                    mapPrefabs = mapCityPrefabs;
                    isCityMap = true;               
                    break;
                case 1:
                    mapPrefabs = mapBeachPrefabs;
                    isBeachMap = true;
                    break;
                case 2:
                    mapPrefabs = mapDesertPrefabs;
                    isDesertMap = true;
                    break;
                case 3:
                    mapPrefabs = mapFieldPrefabs;
                    isFieldMap = true;
                    break;
                case 4:
                    mapPrefabs = mapIcePrefabs;
                    isIceMap = true;
                    break;
                case 5:
                    mapPrefabs = mapLavaPrefabs;
                    isLavaMap = true;
                    break;  
            }
            Debug.Log($"Map Index {mapIndex}: Switched to map type {tempSpawnList[0]}");
        }
        else if (mapIndex == mapSpawnIndex[2])
        {
            ResetAllMapFlags();
            switch (tempSpawnList[1])
            {
                case 0:
                    mapPrefabs = mapCityPrefabs;
                    isCityMap = true;               
                    break;
                case 1:
                    mapPrefabs = mapBeachPrefabs;
                    isBeachMap = true;
                    break;
                case 2:
                    mapPrefabs = mapDesertPrefabs;
                    isDesertMap = true;
                    break;
                case 3:
                    mapPrefabs = mapFieldPrefabs;
                    isFieldMap = true;
                    break;
                case 4:
                    mapPrefabs = mapIcePrefabs;
                    isIceMap = true;
                    break;
                case 5:
                    mapPrefabs = mapLavaPrefabs;
                    isLavaMap = true;
                    break;  
            }
            Debug.Log($"Map Index {mapIndex}: Switched to map type {tempSpawnList[1]}");
        }
        else if (mapIndex == mapSpawnIndex[3])
        {
            ResetAllMapFlags();
            switch (tempSpawnList[2])
            {
                case 0:
                    mapPrefabs = mapCityPrefabs;
                    isCityMap = true;               
                    break;
                case 1:
                    mapPrefabs = mapBeachPrefabs;
                    isBeachMap = true;
                    break;
                case 2:
                    mapPrefabs = mapDesertPrefabs;
                    isDesertMap = true;
                    break;
                case 3:
                    mapPrefabs = mapFieldPrefabs;
                    isFieldMap = true;
                    break;
                case 4:
                    mapPrefabs = mapIcePrefabs;
                    isIceMap = true;
                    break;
                case 5:
                    mapPrefabs = mapLavaPrefabs;
                    isLavaMap = true;
                    break;  
            }
            Debug.Log($"Map Index {mapIndex}: Switched to map type {tempSpawnList[2]}");
        }
        else if (mapIndex == mapSpawnIndex[4])
        {
            ResetAllMapFlags();
            switch (tempSpawnList[3])
            {
                case 0:
                    mapPrefabs = mapCityPrefabs;
                    isCityMap = true;               
                    break;
                case 1:
                    mapPrefabs = mapBeachPrefabs;
                    isBeachMap = true;
                    break;
                case 2:
                    mapPrefabs = mapDesertPrefabs;
                    isDesertMap = true;
                    break;
                case 3:
                    mapPrefabs = mapFieldPrefabs;
                    isFieldMap = true;
                    break;
                case 4:
                    mapPrefabs = mapIcePrefabs;
                    isIceMap = true;
                    break;
                case 5:
                    mapPrefabs = mapLavaPrefabs;
                    isLavaMap = true;
                    break;  
            }
            Debug.Log($"Map Index {mapIndex}: Switched to map type {tempSpawnList[3]}");
        }
        else if (mapIndex == mapSpawnIndex[5])
        {
            ResetAllMapFlags();
            switch (tempSpawnList[4])
            {
                case 0:
                    mapPrefabs = mapCityPrefabs;
                    isCityMap = true;               
                    break;
                case 1:
                    mapPrefabs = mapBeachPrefabs;
                    isBeachMap = true;
                    break;
                case 2:
                    mapPrefabs = mapDesertPrefabs;
                    isDesertMap = true;
                    break;
                case 3:
                    mapPrefabs = mapFieldPrefabs;
                    isFieldMap = true;
                    break;
                case 4:
                    mapPrefabs = mapIcePrefabs;
                    isIceMap = true;
                    break;
                case 5:
                    mapPrefabs = mapLavaPrefabs;
                    isLavaMap = true;
                    break;  
            }
            Debug.Log($"Map Index {mapIndex}: Switched to map type {tempSpawnList[4]}");
        }
        
    }

    // THÊM: Method để reset tất cả map flags
    void ResetAllMapFlags()
    {
        isCityMap = false;
        isBeachMap = false;
        isDesertMap = false;
        isFieldMap = false;
        isIceMap = false;
        isLavaMap = false;
    }


    public void Reset()
    {
        mapIndex = 0;
        mapPrefabs = mapCityPrefabs;
    }

    List<int> tempSpawnList = new List<int>() { };

    public void RandomMap()
    {
        int randomValue = 0;
        if (checkMapStartCitySpawned)
        {
            randomValue = 0;
        }
        if (checkMapStartBeachSpawned)
        {
            randomValue = 1;
        }
        if (checkMapStartDesertSpawned)
        {
            randomValue = 2;
        }
        if (checkMapStartFieldSpawned)
        {
            randomValue = 3;
        }
        if (checkMapStartIceSpawned)
        {
            randomValue = 4;
        }
        if (checkMapStartLavaSpawned)
        {
            randomValue = 5;
        }
        Debug.Log("Giá trị randomValue: " + randomValue);
        // Nhóm trước a (0,1,2)
        List<int> beforeA = mapList.GetRange(0, randomValue);

        // Shuffle nhóm trước a bằng Random.Range
        for (int i = 0; i < beforeA.Count; i++)
        {
            int randomIndex1 = Random.Range(i, beforeA.Count);
            int temp = beforeA[i];
            beforeA[i] = beforeA[randomIndex1];
            beforeA[randomIndex1] = temp;
        }

        List<int> afterA = mapList.GetRange(randomValue + 1, mapList.Count - (randomValue + 1));
        // Ghép lại
        List<int> resultMap = new List<int>();
        resultMap.AddRange(beforeA);
        resultMap.AddRange(afterA);

        Debug.Log("Kết quả: " + string.Join(",", resultMap));
        tempSpawnList  = resultMap;
    }

    // public void SetTempSpawnList(List<int> newList)
    // {
    //     tempSpawnList = newList;
    // }
}
