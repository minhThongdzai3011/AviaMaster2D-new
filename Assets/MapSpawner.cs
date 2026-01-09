
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

    [Header("Debug Spawn 5 Map")]
    private int spawnCountInCurrentMap = 0;
    private GameObject airPortMap;

    [Header("Airport Prefabs")]
    public GameObject[] mapCityAirportPrefabs;
    public GameObject[] mapBeachAirportPrefabs;
    public GameObject[] mapDesertAirportPrefabs;
    public GameObject[] mapFieldAirportPrefabs;
    public GameObject[] mapIceAirportPrefabs;
    public GameObject[] mapLavaAirportPrefabs;


    [Header("MapCity")]
    public int mapCityIndex = 0;
    public bool isCityMap = false;
    public bool isCityStartSpawned = false;
    public GameObject[] mapCityChangeAllPrefab;
    public GameObject mapCityStartPrefab;
    public GameObject[] mapCityPrefabs;

    [Header("MapBeach")]
    public int mapBeachIndex = 0;
    public bool isBeachMap = false;
    public bool isBeachStartSpawned = false;
    public GameObject[] mapBeachChangeAllPrefab;
    public GameObject mapBeachStartPrefab;
    public GameObject[] mapBeachPrefabs;

    [Header("MapDesert")]
    public int mapDesertIndex = 0;
    public bool isDesertMap = false;
    public bool isDesertStartSpawned = false;
    public GameObject[] mapDesertChangeAllPrefab;
    public GameObject mapDesertStartPrefab;
    public GameObject[] mapDesertPrefabs;

    [Header("MapField")]
    public int mapFieldIndex = 0;
    public bool isFieldMap = false;
    public bool isFieldStartSpawned = false;
    public GameObject[] mapFieldChangeAllPrefab;
    public GameObject mapFieldStartPrefab;
    public GameObject[] mapFieldPrefabs;

    [Header("MapIce")]
    public int mapIceIndex = 0;
    public bool isIceMap = false;
    public bool isIceStartSpawned = false;
    public GameObject[] mapIceChangeAllPrefab;
    public GameObject mapIceStartPrefab;
    public GameObject[] mapIcePrefabs;

    [Header("MapLava")]
    public int mapLavaIndex = 0;
    public bool isLavaMap = false;
    public bool isLavaStartSpawned = false;
    public GameObject[] mapLavaChangeAllPrefab;
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
    private float spawnInterval = 0.12f;
    public int count = 0;

    [Header("Kiem tra Map da Unlock")]
    public bool isMapCityUnlocked = true;
    public bool isMapBeachUnlocked = false;
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

    void Awake()
    {
        instance = this;
        
        isMapCityUnlocked = PlayerPrefs.GetInt("IsMapCityUnlocked", 1) == 1;
        isMapBeachUnlocked = PlayerPrefs.GetInt("IsMapBeachUnlocked", 0) == 1;
        isMapDesertUnlocked = PlayerPrefs.GetInt("IsMapDesertUnlocked", 0) == 1;
        isMapFieldUnlocked = PlayerPrefs.GetInt("IsMapFieldUnlocked", 0) == 1;
        isMapIceUnlocked = PlayerPrefs.GetInt("IsMapIceUnlocked", 0) == 1;
        isMapLavaUnlocked = PlayerPrefs.GetInt("IsMapLavaUnlocked", 0) == 1;
    }
    
    // *** THÊM HÀM ĐỂ TẮT TẤT CẢ MAPSTART ***
    void DeactivateAllMapStarts()
    {
        Debug.Log("[MAPSTART] Deactivating all mapStart objects");
        
        if (mapStart == null || mapStart.Length == 0)
        {
            Debug.LogWarning("[MAPSTART] mapStart array is null or empty!");
            return;
        }
        
        for (int i = 0; i < mapStart.Length; i++)
        {
            if (mapStart[i] != null)
            {
                bool wasActive = mapStart[i].activeInHierarchy;
                mapStart[i].SetActive(false);
                if (wasActive)
                {
                    Debug.Log($"[MAPSTART] Deactivated mapStart[{i}] ({GetMapNameByIndex(i)})");
                }
            }
        }
        
        // Reset các biến trạng thái
        isCityMap = false;
        isBeachMap = false;
        isDesertMap = false;
        isFieldMap = false;
        isIceMap = false;
        isLavaMap = false;
        
        checkMapStartCitySpawned = false;
        checkMapStartBeachSpawned = false;
        checkMapStartDesertSpawned = false;
        checkMapStartFieldSpawned = false;
        checkMapStartIceSpawned = false;
        checkMapStartLavaSpawned = false;
        
        Debug.Log("[MAPSTART] Reset all map flags and spawn states");
    }
    
    // *** HELPER ĐỂ DEBUG ***
    string GetMapNameByIndex(int index)
    {
        switch (index)
        {
            case 0: return "City";
            case 1: return "Beach";
            case 2: return "Desert";
            case 3: return "Field";
            case 4: return "Ice";
            case 5: return "Lava";
            default: return "Unknown";
        }
    }

    void Start()
    {
        // *** TẮT TẤT CẢ MAPSTART TRƯỚC KHI BẮT ĐẦU ***
        DeactivateAllMapStarts();
        
        // *** KIỂM TRA HẠ CÁNH AN TOÀN LẦN TRƯỚC ***
        int lastSafeLanding = PlayerPrefs.GetInt("LastSafeLandingAirport", -1);
        
        if (lastSafeLanding >= 0)
        {
            Debug.Log($"[START] Player landed safely at airport {lastSafeLanding} last game!");
            
            // Bắt đầu từ map đã hạ cánh an toàn
            switch (lastSafeLanding)
            {
                case 0: // City
                    mapPrefabs = mapCityPrefabs;
                    isCityMap = true;
                    mapStart[0].SetActive(true);
                    checkMapStartCitySpawned = true;
                    Debug.Log("[START] Starting at City map due to safe landing");
                    break;
                case 1: // Beach
                    mapPrefabs = mapBeachPrefabs;
                    isBeachMap = true;
                    mapStart[1].SetActive(true);
                    checkMapStartBeachSpawned = true;
                    Debug.Log("[START] Starting at Beach map due to safe landing");
                    break;
                case 2: // Desert
                    mapPrefabs = mapDesertPrefabs;
                    isDesertMap = true;
                    mapStart[2].SetActive(true);
                    checkMapStartDesertSpawned = true;
                    Debug.Log("[START] Starting at Desert map due to safe landing");
                    break;
                case 3: // Field
                    mapPrefabs = mapFieldPrefabs;
                    isFieldMap = true;
                    mapStart[3].SetActive(true);
                    checkMapStartFieldSpawned = true;
                    Debug.Log("[START] Starting at Field map due to safe landing");
                    break;
                case 4: // Ice
                    mapPrefabs = mapIcePrefabs;
                    isIceMap = true;
                    mapStart[4].SetActive(true);
                    checkMapStartIceSpawned = true;
                    Debug.Log("[START] Starting at Ice map due to safe landing");
                    break;
                case 5: // Lava
                    mapPrefabs = mapLavaPrefabs;
                    isLavaMap = true;
                    mapStart[5].SetActive(true);
                    checkMapStartLavaSpawned = true;
                    Debug.Log("[START] Starting at Lava map due to safe landing");
                    break;
            }
            
            // Clear safe landing flag
            PlayerPrefs.SetInt("LastSafeLandingAirport", -1);
            PlayerPrefs.Save();
        }
        else
        {
            // Logic cũ - random map dựa vào unlock status
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
        }
        int check = 0;
        if (checkMapStartCitySpawned)
        {
            check++;
            Debug.Log("[START] City mapStart is active");
        }
        if (checkMapStartBeachSpawned)
        {
            check++;
        Debug.Log("[START] Beach mapStart is active");
        }
        if (checkMapStartDesertSpawned)
        {
            check++;
        Debug.Log("[START] Desert mapStart is active");
        }   
        if (checkMapStartFieldSpawned)
        {
            check++;
        Debug.Log("[START] Field mapStart is active");
        }
        if (checkMapStartIceSpawned)
        {
            check++;
        Debug.Log("[START] Ice mapStart is active");
        }
        if (checkMapStartLavaSpawned)
        {
            check++;
        Debug.Log("[START] Lava mapStart is active");
        }
        
        if(check == 0)
        {
            // Mặc định nếu không có map nào được spawn
            mapPrefabs = mapCityPrefabs;
            isCityMap = true;
            mapStart[0].SetActive(true);
            checkMapStartCitySpawned = true;
        }
        else if (check > 1)
        {
            mapPrefabs = mapCityPrefabs;
            isCityMap = true;
            mapStart[0].SetActive(true);
            checkMapStartCitySpawned = true;

            isMapBeachUnlocked = false;
            isMapDesertUnlocked = false;
            isMapFieldUnlocked = false;
            isMapIceUnlocked = false;
            isMapLavaUnlocked = false;

            

            checkMapStartBeachSpawned = false;
            checkMapStartDesertSpawned = false;
            checkMapStartFieldSpawned = false;
            checkMapStartIceSpawned = false;
            checkMapStartLavaSpawned = false;


            mapStart[1].SetActive(false);
            mapStart[2].SetActive(false);
            mapStart[3].SetActive(false);
            mapStart[4].SetActive(false);
            mapStart[5].SetActive(false);
            
            Debug.Log("[START] Multiple mapStarts were active, defaulting to City map");

        }
        RandomMap();
        
        // *** KHỞI TẠO lastMapSwitchIndex để tránh switch liên tục ***
        int currentMapCount = GameObject.FindGameObjectsWithTag("Map").Length;
        lastMapSwitchIndex = currentMapCount / 20;
        Debug.Log($"[INIT] Current map count: {currentMapCount}, starting at index: {lastMapSwitchIndex}");
        
        StartCoroutine(SpawnMapCoroutine());
        
    }
    public int countMapSpawned = 0;
    public bool stopCheckSpawnRate01 = true;
    public bool stopCheckSpawnRate12 = true;
    public bool stopCheckSpawnRate23 = true;
    public bool stopCheckSpawnRate34 = true;
    public bool stopCheckSpawnRate45 = true;

    void Update()
    {
        // *** Chỉ đếm map tiles, không đếm airports ***
        int totalMapObjects = GameObject.FindGameObjectsWithTag("Map").Length;
        int airportCount = GameObject.FindGameObjectsWithTag("Airport").Length;
        countMapSpawned = totalMapObjects - airportCount; // Trừ airports ra khỏi count
        
        // *** KHÔNG SET FLAGS Ở ĐÂY - Để tránh override logic trong CheckSpawnChangeMap ***
    }

    IEnumerator SpawnMapCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        
        while (true)
        {
            count = GameObject.FindGameObjectsWithTag("Map").Length;
            if (count < 121)
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

        if (isCityMap && !checkMapStartCitySpawned)
        {
            CheckSpawnChangeMap();
            checkMapStartCitySpawned = true;
            return; 
        }
        else if (isBeachMap && !checkMapStartBeachSpawned)
        {
            CheckSpawnChangeMap();
            checkMapStartBeachSpawned = true;
            return;
        }
        else if (isDesertMap && !checkMapStartDesertSpawned)
        {
            CheckSpawnChangeMap();
            checkMapStartDesertSpawned = true;
            return;
        }
        else if (isFieldMap && !checkMapStartFieldSpawned)
        {
            CheckSpawnChangeMap();
            checkMapStartFieldSpawned = true;
            return;
        }
        else if (isIceMap && !checkMapStartIceSpawned)
        {
            CheckSpawnChangeMap();
            checkMapStartIceSpawned = true;
            return;
        }
        else if (isLavaMap && !checkMapStartLavaSpawned)
        {
            CheckMap45();
            checkMapStartLavaSpawned = true;
            return;
        }

        float currentSpawnY = spawnRangeYmin;
        if (isCityMap)
        {
            currentSpawnY = spawnRangeYCityMin;
        }
        else if (isBeachMap)
        {
            currentSpawnY = spawnRangeYBeachMin;
        }
        else if (isDesertMap)
        {
            currentSpawnY = spawnRangeYDesertMin;
        }
        else if (isFieldMap)
        {
            currentSpawnY = spawnRangeYFieldMin;
        }
        else if (isIceMap)
        {
            currentSpawnY = spawnRangeYIceMin;
        }
        else if (isLavaMap)
        {
            currentSpawnY = spawnRangeYLavaMin;
        }

        int prefabIndex = Random.Range(0, mapPrefabs.Length);
        Vector3 spawnPosition = new Vector3(spawnRangeX, currentSpawnY, 0f);
        
        GameObject spawnedMap = Instantiate(mapPrefabs[prefabIndex], spawnPosition, mapPrefabs[prefabIndex].transform.rotation);
        spawnedMap.tag = "Map"; 
        
        
        spawnRangeX += 30f;
        spawnCountInCurrentMap++;


        if (spawnCountInCurrentMap % 5 == 0 && spawnCountInCurrentMap <= 15)
        {
            SpawnAirportForCurrentBiome();
        }
    }
    
    void SpawnAirportForCurrentBiome()
    {
        GameObject airportToSpawn = null;
        int airportIndex = (spawnCountInCurrentMap / 5) - 1; // 5->0, 10->1, 15->2
        float airportY = -4.2f; // Y cố định cho airport
        
        Debug.Log($"[AIRPORT DEBUG] Count: {spawnCountInCurrentMap}, Index: {airportIndex}");
        Debug.Log($"[AIRPORT DEBUG] Current biome - City:{isCityMap}, Beach:{isBeachMap}, Desert:{isDesertMap}, Field:{isFieldMap}, Ice:{isIceMap}, Lava:{isLavaMap}");
        
        // *** SỬA: KIỂM TRA BIOME HIỆN TẠI CHÍNH XÁC ***
        if (isCityMap && mapCityAirportPrefabs != null && mapCityAirportPrefabs.Length > 0)
        {
            if (airportIndex < mapCityAirportPrefabs.Length)
            {
                airportToSpawn = mapCityAirportPrefabs[airportIndex];
            }
            else
            {
                Debug.LogError($"[AIRPORT] City airport index {airportIndex} out of range (length: {mapCityAirportPrefabs.Length})");
            }
        }
        else if (isBeachMap && mapBeachAirportPrefabs != null && mapBeachAirportPrefabs.Length > 0)
        {
            if (airportIndex < mapBeachAirportPrefabs.Length)
            {
                airportToSpawn = mapBeachAirportPrefabs[airportIndex];
            }
            else
            {
                Debug.LogError($"[AIRPORT] Beach airport index {airportIndex} out of range (length: {mapBeachAirportPrefabs.Length})");
            }
        }
        else if (isDesertMap && mapDesertAirportPrefabs != null && mapDesertAirportPrefabs.Length > 0)
        {
            if (airportIndex < mapDesertAirportPrefabs.Length)
            {
                airportToSpawn = mapDesertAirportPrefabs[airportIndex];
            }
            else
            {
                Debug.LogError($"[AIRPORT] Desert airport index {airportIndex} out of range (length: {mapDesertAirportPrefabs.Length})");
            }
        }
        else if (isFieldMap && mapFieldAirportPrefabs != null && mapFieldAirportPrefabs.Length > 0)
        {
            if (airportIndex < mapFieldAirportPrefabs.Length)
            {
                airportToSpawn = mapFieldAirportPrefabs[airportIndex];
            }
        }
        else if (isIceMap && mapIceAirportPrefabs != null && mapIceAirportPrefabs.Length > 0)
        {
            if (airportIndex < mapIceAirportPrefabs.Length)
            {
                airportToSpawn = mapIceAirportPrefabs[airportIndex];
            }
        }
        else if (isLavaMap && mapLavaAirportPrefabs != null && mapLavaAirportPrefabs.Length > 0)
        {
            if (airportIndex < mapLavaAirportPrefabs.Length)
            {
                airportToSpawn = mapLavaAirportPrefabs[airportIndex];
            }
        }
        else
        {
            Debug.LogError($"[AIRPORT] No valid airport array for current biome! City:{isCityMap}, Beach:{isBeachMap}, Desert:{isDesertMap}");
        }

        if (airportToSpawn != null)
        {
            Vector3 airportPosition = new Vector3(spawnRangeX, airportY, 0f);
            GameObject spawnedAirport = Instantiate(airportToSpawn, airportPosition, airportToSpawn.transform.rotation);
            spawnedAirport.name = $"AIRPORT_{GetCurrentBiomeName()}_{spawnCountInCurrentMap}";
            spawnedAirport.tag = "Airport";
            
            spawnRangeX += 30f;
        }
    }
    
    string GetCurrentBiomeName()
    {
        if (isCityMap) return "City";
        if (isBeachMap) return "Beach";
        if (isDesertMap) return "Desert";
        if (isFieldMap) return "Field";
        if (isIceMap) return "Ice";
        if (isLavaMap) return "Lava";
        return "Unknown";
    }
    public void DeleteSpawnedItems()
    {
        GameObject[] spawnedItems = GameObject.FindGameObjectsWithTag("Map");
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        
        // *** Xóa cả airports ***
        GameObject[] spawnedAirports = GameObject.FindGameObjectsWithTag("Airport");
        foreach (GameObject airport in spawnedAirports)
        {
            Destroy(airport);
        }
        
        spawnRangeX = -12f;
        lastMapSwitchIndex = -1; // *** RESET để bắt đầu từ đầu ***
        spawnCountInCurrentMap = 0; // *** RESET counter ***
        Reset();

    }

    private int lastMapSwitchIndex = -1; // Track khi nào đã switch map
    
    public void CheckMap()
    {
        // *** Chỉ đếm map tiles, không đếm airports ***
        int totalMapObjects = GameObject.FindGameObjectsWithTag("Map").Length;
        int airportCount = GameObject.FindGameObjectsWithTag("Airport").Length;
        int countSpawned = totalMapObjects - airportCount;

        int index = countSpawned / 20;   // Mỗi 20 map → đổi map
        if (index >= tempSpawnList.Count)
            return;

        // *** CHỈ SWITCH KHI INDEX THAY ĐỔI ***
        if (index != lastMapSwitchIndex)
        {
            int nextMap = tempSpawnList[index];
            SwitchToNextMap(nextMap);
            lastMapSwitchIndex = index;
            Debug.Log($"[MAP SWITCH] Switched at map count: {countSpawned}, index: {index}");
        }
    }

    void ResetAllMapFlags()
    {
        isCityMap = false;
        isBeachMap = false;
        isDesertMap = false;
        isFieldMap = false;
        isIceMap = false;
        isLavaMap = false;
        
        // *** RESET transition flags để spawn transition maps khi chuyển biome ***
        checkMapStartCitySpawned = false;
        checkMapStartBeachSpawned = false;
        checkMapStartDesertSpawned = false;
        checkMapStartFieldSpawned = false;
        checkMapStartIceSpawned = false;
        checkMapStartLavaSpawned = false;
        
        // *** RESET stopCheckSpawnRate flags để cho phép spawn transition ***
        // Dựa vào lastMapSwitchIndex để biết đang ở transition nào
        if (lastMapSwitchIndex == 0)
        {
            stopCheckSpawnRate01 = false;
            Debug.Log("[RESET] Enabled transition 0->1");
        }
        else if (lastMapSwitchIndex == 1)
        {
            stopCheckSpawnRate12 = false;
            Debug.Log("[RESET] Enabled transition 1->2");
        }
        else if (lastMapSwitchIndex == 2)
        {
            stopCheckSpawnRate23 = false;
            Debug.Log("[RESET] Enabled transition 2->3");
        }
        else if (lastMapSwitchIndex == 3)
        {
            stopCheckSpawnRate34 = false;
            Debug.Log("[RESET] Enabled transition 3->4");
        }
        else if (lastMapSwitchIndex == 4)
        {
            stopCheckSpawnRate45 = false;
            Debug.Log("[RESET] Enabled transition 4->5");
        }
        
        Debug.Log("[RESET] Reset all map transition flags");
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
        
        // *** SỬA: Nếu chỉ có 1 map unlocked, không cần shuffle ***
        if (randomValue == 0 && mapList.Count == 1)
        {
            tempSpawnList = new List<int>(); // List rỗng - không có map tiếp theo
            Debug.Log("Chỉ có 1 map unlocked - tempSpawnList rỗng");
            return;
        }
        
        // Nhóm trước a (0,1,2)
        List<int> beforeA = new List<int>();
        if (randomValue > 0)
        {
            beforeA = mapList.GetRange(0, randomValue);
        }

        // Shuffle nhóm trước a bằng Random.Range
        for (int i = 0; i < beforeA.Count; i++)
        {
            int randomIndex1 = Random.Range(i, beforeA.Count);
            int temp = beforeA[i];
            beforeA[i] = beforeA[randomIndex1];
            beforeA[randomIndex1] = temp;
        }

        List<int> afterA = new List<int>();
        if (randomValue + 1 < mapList.Count)
        {
            afterA = mapList.GetRange(randomValue + 1, mapList.Count - (randomValue + 1));
        }
        
        // Ghép lại
        List<int> resultMap = new List<int>();
        resultMap.AddRange(beforeA);
        resultMap.AddRange(afterA);

        Debug.Log("Kết quả: " + randomValue + " / " + string.Join(",", resultMap));
        tempSpawnList  = resultMap;
        tempSpawnList.Insert(0, randomValue);
        ChangeBonusMap.instance.mapList = tempSpawnList;
        ChangeBonusMap.instance.isChangeBonusMap = true;
        Debug.Log("Kết quả cuối cùng: " + string.Join(",", tempSpawnList));

    }

    public void CheckSpawnChangeMap()
    {
        if (tempSpawnList.Count == 0)
        {
            Debug.Log("[TRANSITION] tempSpawnList empty - no transition needed");
            return;
        }

        Debug.Log($"[TRANSITION] tempSpawnList: [{string.Join(", ", tempSpawnList)}]");
        Debug.Log($"[TRANSITION] stopCheckSpawnRate flags - 01:{stopCheckSpawnRate01}, 12:{stopCheckSpawnRate12}, 23:{stopCheckSpawnRate23}, 34:{stopCheckSpawnRate34}, 45:{stopCheckSpawnRate45}");
        
        if (!stopCheckSpawnRate01)
        {
            Debug.Log("[TRANSITION] ✅ Calling CheckMap01()");
            CheckMap01();
            stopCheckSpawnRate01 = true;
            return;
        }
        if (!stopCheckSpawnRate12)
        {
            Debug.Log("[TRANSITION] ✅ Calling CheckMap12()");
            CheckMap12();
            stopCheckSpawnRate12 = true;
            return;
        }
        if (!stopCheckSpawnRate23)
        {
            Debug.Log("[TRANSITION] ✅ Calling CheckMap23()");
            CheckMap23();
            stopCheckSpawnRate23 = true;
            return;
        }
        if (!stopCheckSpawnRate34)
        {
            Debug.Log("[TRANSITION] ✅ Calling CheckMap34()");
            CheckMap34();
            stopCheckSpawnRate34 = true;
            return;
        }
        if (!stopCheckSpawnRate45)
        {
            Debug.Log("[TRANSITION] ✅ Calling CheckMap45()");
            CheckMap45();
            stopCheckSpawnRate45 = true;
            return;
        }
        
        Debug.LogWarning("[TRANSITION] ❌ No transition method called - all flags are true!");
    }

    public void SwitchToNextMap(int mapType)
    {
        Debug.Log($"[MAP SWITCH] Switching from {GetCurrentBiomeName()} to {mapType}");
        
        ResetAllMapFlags();

        // *** SỬA: RESET spawnCountInCurrentMap NGAY KHI SWITCH ***
        spawnCountInCurrentMap = 0;
        Debug.Log($"[MAP SWITCH] Reset spawnCountInCurrentMap to 0");

        switch (mapType)
        {
            case 0: 
                mapPrefabs = mapCityPrefabs; 
                isCityMap = true;
                Debug.Log("[SWITCH] ✅ Switched to City");
                break;
            case 1: 
                mapPrefabs = mapBeachPrefabs; 
                isBeachMap = true;
                Debug.Log("[SWITCH] ✅ Switched to Beach");
                break;
            case 2: 
                mapPrefabs = mapDesertPrefabs; 
                isDesertMap = true;
                Debug.Log("[SWITCH] ✅ Switched to Desert");
                break;
            case 3: 
                mapPrefabs = mapFieldPrefabs; 
                isFieldMap = true;
                Debug.Log("[SWITCH] ✅ Switched to Field");
                break;
            case 4: 
                mapPrefabs = mapIcePrefabs; 
                isIceMap = true;
                Debug.Log("[SWITCH] ✅ Switched to Ice");
                break;
            case 5: 
                mapPrefabs = mapLavaPrefabs; 
                isLavaMap = true;
                Debug.Log("[SWITCH] ✅ Switched to Lava");
                break;
        }
        
        Debug.Log($"[MAP SWITCH] Current biome after switch - City:{isCityMap}, Beach:{isBeachMap}, Desert:{isDesertMap}");
    }


    

    public void CheckMap01(){
        
        if(tempSpawnList[0] == 1) // map beach toi moi map
        {
            if(tempSpawnList[1] == 0)
            {
                Instantiate(mapBeachChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->City transition and start");
            }
            if(tempSpawnList[1] == 2)
            {
                Instantiate(mapBeachChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Desert transition and start");
            }
            if(tempSpawnList[1] == 3)
            {
                Instantiate(mapBeachChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Field transition and start");
            }
            if(tempSpawnList[1] == 4)
            {
                Instantiate(mapBeachChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Ice transition and start");
            }
            if(tempSpawnList[1] == 5)
            {
                Instantiate(mapBeachChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Lava transition and start");
            }
        }
        if(tempSpawnList[0] == 0) // ban dau la city toi moi map
        {
            if(tempSpawnList[1] == 1)
            {
                Instantiate(mapCityChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Beach transition and start");
            }
            if(tempSpawnList[1] == 2)
            {
                Instantiate(mapCityChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Desert transition and start");
            }
            if(tempSpawnList[1] == 3)
            {
                Instantiate(mapCityChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Field transition and start");
            }
            if(tempSpawnList[1] == 4)
            {
                Instantiate(mapCityChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Ice transition and start");
            }
            if(tempSpawnList[1] == 5)
            {
                Instantiate(mapCityChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Lava transition and start");
            }
        }
        if(tempSpawnList[0] == 2) // ban dau la desert toi moi map
        {
            if(tempSpawnList[1] == 0)
            {
                Instantiate(mapDesertChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->City transition and start");
            }
            if(tempSpawnList[1] == 1)
            {
                Instantiate(mapDesertChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Beach transition and start");
            }
           if(tempSpawnList[1] == 3)
            {
                Instantiate(mapDesertChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Field transition and start");
            }
            if(tempSpawnList[1] == 4)
            {
                Instantiate(mapDesertChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Ice transition and start");
            }
            if(tempSpawnList[1] == 5)
            {
                Instantiate(mapDesertChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Lava transition and start");
            }

        }
        if(tempSpawnList[0] == 3)
        {
            if(tempSpawnList[1] == 0)
            {
                Instantiate(mapFieldChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->City transition and start");
            }
            if(tempSpawnList[1] == 1)
            {
                Instantiate(mapFieldChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Beach transition and start");
            }
            if(tempSpawnList[1] == 2)
            {
                Instantiate(mapFieldChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Desert transition and start");
            }
            if(tempSpawnList[1] == 4)
            {
                Instantiate(mapFieldChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Ice transition and start");
            }
            if(tempSpawnList[1] == 5)
            {
                Instantiate(mapFieldChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Lava transition and start");
            }
        }
        if(tempSpawnList[0] == 4)
        {
            if(tempSpawnList[1] == 0)
            {
                Instantiate(mapIceChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->City transition and start");
            }
            if(tempSpawnList[1] == 1)
            {
                Instantiate(mapIceChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Beach transition and start");
            }
            if(tempSpawnList[1] == 2)
            {
                Instantiate(mapIceChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Desert transition and start");
            }
            if(tempSpawnList[1] == 3)
            {
                Instantiate(mapIceChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Field transition and start");
            }
            if(tempSpawnList[1] == 5)
            {
                Instantiate(mapIceChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Lava transition and start");
            }
            
        }
        if(tempSpawnList[0] == 5)
        {
            if(tempSpawnList[1] == 0)
            {
                Instantiate(mapLavaChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->City transition and start");
            }
            if(tempSpawnList[1] == 1)
            {
                Instantiate(mapLavaChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Beach transition and start");
            }
            if(tempSpawnList[1] == 2)
            {
                Instantiate(mapLavaChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Desert transition and start");
            }
            if(tempSpawnList[1] == 3)
            {
                Instantiate(mapLavaChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Field transition and start");
            }
            if(tempSpawnList[1] == 4)
            {
                Instantiate(mapLavaChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Ice transition and start");
            }
        }

    }

    public void CheckMap12(){
        
        if(tempSpawnList[1] == 1) // map beach toi moi map
        {
            if(tempSpawnList[2] == 0)
            {
                Instantiate(mapBeachChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->City transition and start");
            }
            if(tempSpawnList[2] == 2)
            {
                Instantiate(mapBeachChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Desert transition and start");
            }
            if(tempSpawnList[2] == 3)
            {
                Instantiate(mapBeachChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Field transition and start");
            }
            if(tempSpawnList[2] == 4)
            {
                Instantiate(mapBeachChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Ice transition and start");
            }
            if(tempSpawnList[2] == 5)
            {
                Instantiate(mapBeachChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Lava transition and start");
            }
        }
        if(tempSpawnList[1] == 0) // ban dau la city toi moi map
        {
            if(tempSpawnList[2] == 1)
            {
                Instantiate(mapCityChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Beach transition and start");
            }
            if(tempSpawnList[2] == 2)
            {
                Instantiate(mapCityChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Desert transition and start");
            }
            if(tempSpawnList[2] == 3)
            {
                Instantiate(mapCityChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Field transition and start");
            }
            if(tempSpawnList[2] == 4)
            {
                Instantiate(mapCityChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Ice transition and start");
            }
            if(tempSpawnList[2] == 5)
            {
                Instantiate(mapCityChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Lava transition and start");
            }
        }
        if(tempSpawnList[1] == 2) // ban dau la desert toi moi map
        {
            if(tempSpawnList[2] == 0)
            {
                Instantiate(mapDesertChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->City transition and start");
            }
            if(tempSpawnList[2] == 1)
            {
                Instantiate(mapDesertChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Beach transition and start");
            }
           if(tempSpawnList[2] == 3)
            {
                Instantiate(mapDesertChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Field transition and start");
            }
            if(tempSpawnList[2] == 4)
            {
                Instantiate(mapDesertChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Ice transition and start");
            }
            if(tempSpawnList[2] == 5)
            {
                Instantiate(mapDesertChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Lava transition and start");
            }

        }
        if(tempSpawnList[1] == 3)
        {
            if(tempSpawnList[2] == 0)
            {
                Instantiate(mapFieldChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->City transition and start");
            }
            if(tempSpawnList[2] == 1)
            {
                Instantiate(mapFieldChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Beach transition and start");
            }
            if(tempSpawnList[2] == 2)
            {
                Instantiate(mapFieldChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Desert transition and start");
            }
            if(tempSpawnList[2] == 4)
            {
                Instantiate(mapFieldChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Ice transition and start");
            }
            if(tempSpawnList[2] == 5)
            {
                Instantiate(mapFieldChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Lava transition and start");
            }
        }
        if(tempSpawnList[1] == 4)
        {
            if(tempSpawnList[2] == 0)
            {
                Instantiate(mapIceChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->City transition and start");
            }
            if(tempSpawnList[2] == 1)
            {
                Instantiate(mapIceChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Beach transition and start");
            }
            if(tempSpawnList[2] == 2)
            {
                Instantiate(mapIceChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Desert transition and start");
            }
            if(tempSpawnList[2] == 3)
            {
                Instantiate(mapIceChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Field transition and start");
            }
            if(tempSpawnList[2] == 5)
            {
                Instantiate(mapIceChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Lava transition and start");
            }
            
        }
        if(tempSpawnList[1] == 5)
        {
            if(tempSpawnList[2] == 0)
            {
                Instantiate(mapLavaChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->City transition and start");
            }
            if(tempSpawnList[2] == 1)
            {
                Instantiate(mapLavaChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Beach transition and start");
            }
            if(tempSpawnList[2] == 2)
            {
                Instantiate(mapLavaChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Desert transition and start");
            }
            if(tempSpawnList[2] == 3)
            {
                Instantiate(mapLavaChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Field transition and start");
            }
            if(tempSpawnList[2] == 4)
            {
                Instantiate(mapLavaChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Ice transition and start");
            }
        }
    
    }

    public void CheckMap23(){
        
        if(tempSpawnList[2] == 1) // map beach toi moi map
        {
            if(tempSpawnList[3] == 0)
            {
                Instantiate(mapBeachChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->City transition and start");
            }
            if(tempSpawnList[3] == 2)
            {
                Instantiate(mapBeachChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Desert transition and start");
            }
            if(tempSpawnList[3] == 3)
            {
                Instantiate(mapBeachChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Field transition and start");
            }
            if(tempSpawnList[3] == 4)
            {
                Instantiate(mapBeachChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Ice transition and start");
            }
            if(tempSpawnList[3] == 5)
            {
                Instantiate(mapBeachChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Lava transition and start");
            }
        }
        if(tempSpawnList[2] == 0) // ban dau la city toi moi map
        {
            if(tempSpawnList[3] == 1)
            {
                Instantiate(mapCityChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Beach transition and start");
            }
            if(tempSpawnList[3] == 2)
            {
                Instantiate(mapCityChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Desert transition and start");
            }
            if(tempSpawnList[3] == 3)
            {
                Instantiate(mapCityChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Field transition and start");
            }
            if(tempSpawnList[3] == 4)
            {
                Instantiate(mapCityChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Ice transition and start");
            }
            if(tempSpawnList[3] == 5)
            {
                Instantiate(mapCityChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Lava transition and start");
            }
        }
        if(tempSpawnList[2] == 2) // ban dau la desert toi moi map
        {
            if(tempSpawnList[3] == 0)
            {
                Instantiate(mapDesertChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->City transition and start");
            }
            if(tempSpawnList[3] == 1)
            {
                Instantiate(mapDesertChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Beach transition and start");
            }
           if(tempSpawnList[3] == 3)
            {
                Instantiate(mapDesertChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Field transition and start");
            }
            if(tempSpawnList[3] == 4)
            {
                Instantiate(mapDesertChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Ice transition and start");
            }
            if(tempSpawnList[3] == 5)
            {
                Instantiate(mapDesertChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Lava transition and start");
            }

        }
        if(tempSpawnList[2] == 3)
        {
            if(tempSpawnList[3] == 0)
            {
                Instantiate(mapFieldChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->City transition and start");
            }
            if(tempSpawnList[3] == 1)
            {
                Instantiate(mapFieldChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Beach transition and start");
            }
            if(tempSpawnList[3] == 2)
            {
                Instantiate(mapFieldChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Desert transition and start");
            }
            if(tempSpawnList[3] == 4)
            {
                Instantiate(mapFieldChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Ice transition and start");
            }
            if(tempSpawnList[3] == 5)
            {
                Instantiate(mapFieldChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Lava transition and start");
            }
        }
        if(tempSpawnList[2] == 4)
        {
            if(tempSpawnList[3] == 0)
            {
                Instantiate(mapIceChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->City transition and start");
            }
            if(tempSpawnList[3] == 1)
            {
                Instantiate(mapIceChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Beach transition and start");
            }
            if(tempSpawnList[3] == 2)
            {
                Instantiate(mapIceChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Desert transition and start");
            }
            if(tempSpawnList[3] == 3)
            {
                Instantiate(mapIceChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Field transition and start");
            }
            if(tempSpawnList[3] == 5)
            {
                Instantiate(mapIceChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Lava transition and start");
            }
            
        }
        if(tempSpawnList[2] == 5)
        {
            if(tempSpawnList[3] == 0)
            {
                Instantiate(mapLavaChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->City transition and start");
            }
            if(tempSpawnList[3] == 1)
            {
                Instantiate(mapLavaChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Beach transition and start");
            }
            if(tempSpawnList[3] == 2)
            {
                Instantiate(mapLavaChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Desert transition and start");
            }
            if(tempSpawnList[3] == 3)
            {
                Instantiate(mapLavaChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Field transition and start");
            }
            if(tempSpawnList[3] == 4)
            {
                Instantiate(mapLavaChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Ice transition and start");
            }
        }
    
    }

    public void CheckMap34(){
        
        if(tempSpawnList[3] == 1) // map beach toi moi map
        {
            if(tempSpawnList[4] == 0)
            {
                Instantiate(mapBeachChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->City transition and start");
            }
            if(tempSpawnList[4] == 2)
            {
                Instantiate(mapBeachChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Desert transition and start");
            }
            if(tempSpawnList[4] == 3)
            {
                Instantiate(mapBeachChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Field transition and start");
            }
            if(tempSpawnList[4] == 4)
            {
                Instantiate(mapBeachChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Ice transition and start");
            }
            if(tempSpawnList[4] == 5)
            {
                Instantiate(mapBeachChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Lava transition and start");
            }
        }
        if(tempSpawnList[3] == 0) // ban dau la city toi moi map
        {
            if(tempSpawnList[4] == 1)
            {
                Instantiate(mapCityChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Beach transition and start");
            }
            if(tempSpawnList[4] == 2)
            {
                Instantiate(mapCityChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Desert transition and start");
            }
            if(tempSpawnList[4] == 3)
            {
                Instantiate(mapCityChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Field transition and start");
            }
            if(tempSpawnList[4] == 4)
            {
                Instantiate(mapCityChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Ice transition and start");
            }
            if(tempSpawnList[4] == 5)
            {
                Instantiate(mapCityChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Lava transition and start");
            }
        }
        if(tempSpawnList[3] == 2) // ban dau la desert toi moi map
        {
            if(tempSpawnList[4] == 0)
            {
                Instantiate(mapDesertChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->City transition and start");
            }
            if(tempSpawnList[4] == 1)
            {
                Instantiate(mapDesertChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Beach transition and start");
            }
           if(tempSpawnList[4] == 3)
            {
                Instantiate(mapDesertChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Field transition and start");
            }
            if(tempSpawnList[4] == 4)
            {
                Instantiate(mapDesertChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Ice transition and start");
            }
            if(tempSpawnList[4] == 5)
            {
                Instantiate(mapDesertChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Lava transition and start");
            }

        }
        if(tempSpawnList[3] == 3)
        {
            if(tempSpawnList[4] == 0)
            {
                Instantiate(mapFieldChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->City transition and start");
            }
            if(tempSpawnList[4] == 1)
            {
                Instantiate(mapFieldChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Beach transition and start");
            }
            if(tempSpawnList[4] == 2)
            {
                Instantiate(mapFieldChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Desert transition and start");
            }
            if(tempSpawnList[4] == 4)
            {
                Instantiate(mapFieldChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Ice transition and start");
            }
            if(tempSpawnList[4] == 5)
            {
                Instantiate(mapFieldChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Lava transition and start");
            }
        }
        if(tempSpawnList[3] == 4)
        {
            if(tempSpawnList[4] == 0)
            {
                Instantiate(mapIceChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->City transition and start");
            }
            if(tempSpawnList[4] == 1)
            {
                Instantiate(mapIceChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Beach transition and start");
            }
            if(tempSpawnList[4] == 2)
            {
                Instantiate(mapIceChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Desert transition and start");
            }
            if(tempSpawnList[4] == 3)
            {
                Instantiate(mapIceChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Field transition and start");
            }
            if(tempSpawnList[4] == 5)
            {
                Instantiate(mapIceChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Lava transition and start");
            }
            
        }
        if(tempSpawnList[3] == 5)
        {
            if(tempSpawnList[4] == 0)
            {
                Instantiate(mapLavaChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->City transition and start");
            }
            if(tempSpawnList[4] == 1)
            {
                Instantiate(mapLavaChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Beach transition and start");
            }
            if(tempSpawnList[4] == 2)
            {
                Instantiate(mapLavaChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Desert transition and start");
            }
            if(tempSpawnList[4] == 3)
            {
                Instantiate(mapLavaChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Field transition and start");
            }
            if(tempSpawnList[4] == 4)
            {
                Instantiate(mapLavaChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Ice transition and start");
            }
        }
    
    }

    
    public void CheckMap45(){
        Debug.Log("Checking Beach to ..." + tempSpawnList[4] + " at index 5 and value " + tempSpawnList[5]);
        if(tempSpawnList[4] == 1) // map beach toi moi map
        {
            Debug.Log("Checking Beach to ...");
            if(tempSpawnList[5] == 0)
            {
                Instantiate(mapBeachChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->City transition and start");
            }
            if(tempSpawnList[5] == 2)
            {
                Instantiate(mapBeachChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Desert transition and start");
            }
            if(tempSpawnList[5] == 3)
            {
                Instantiate(mapBeachChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Field transition and start");
            }
            if(tempSpawnList[5] == 4)
            {
                Instantiate(mapBeachChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Ice transition and start");
            }
            if(tempSpawnList[5] == 5)
            {
                Instantiate(mapBeachChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapBeachChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Beach->Lava transition and start");
            }
        }
        if(tempSpawnList[4] == 0) // ban dau la city toi moi map
        {
            Debug.Log("Checking City to ...");
            if(tempSpawnList[5] == 1)
            {
                Instantiate(mapCityChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Beach transition and start");
            }
            if(tempSpawnList[5] == 2)
            {
                Instantiate(mapCityChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Desert transition and start");
            }
            if(tempSpawnList[5] == 3)
            {
                Instantiate(mapCityChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Field transition and start");
            }
            if(tempSpawnList[5] == 4)
            {
                Instantiate(mapCityChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Ice transition and start");
            }
            if(tempSpawnList[5] == 5)
            {
                Instantiate(mapCityChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapCityChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned City->Lava transition and start");
            }
        }
        if(tempSpawnList[4] == 2) // ban dau la desert toi moi map
        {
            Debug.Log("Checking Desert to ...");
            if(tempSpawnList[5] == 0)
            {
                Instantiate(mapDesertChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->City transition and start");
            }
            if(tempSpawnList[5] == 1)
            {
                Instantiate(mapDesertChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Beach transition and start");
            }
           if(tempSpawnList[5] == 3)
            {
                Instantiate(mapDesertChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Field transition and start");
            }
            if(tempSpawnList[5] == 4)
            {
                Instantiate(mapDesertChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Ice transition and start");
            }
            if(tempSpawnList[5] == 5)
            {
                Instantiate(mapDesertChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapDesertChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Desert->Lava transition and start");
            }

        }
        if(tempSpawnList[4] == 3)
        {
            Debug.Log("Checking Field to ...");
            if(tempSpawnList[5] == 0)
            {
                Instantiate(mapFieldChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->City transition and start");
            }
            if(tempSpawnList[5] == 1)
            {
                Instantiate(mapFieldChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Beach transition and start");
            }
            if(tempSpawnList[5] == 2)
            {
                Instantiate(mapFieldChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Desert transition and start");
            }
            if(tempSpawnList[5] == 4)
            {
                Instantiate(mapFieldChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Ice transition and start");
            }
            if(tempSpawnList[5] == 5)
            {
                Instantiate(mapFieldChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapFieldChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Field->Lava transition and start");
            }
        }
        if(tempSpawnList[4] == 4)
        {
            Debug.Log("Checking Ice to ...");
            if(tempSpawnList[5] == 0)
            {
                Instantiate(mapIceChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->City transition and start");
            }
            if(tempSpawnList[5] == 1)
            {
                Instantiate(mapIceChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Beach transition and start");
            }
            if(tempSpawnList[5] == 2)
            {
                Instantiate(mapIceChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Desert transition and start");
            }
            if(tempSpawnList[5] == 3)
            {
                Instantiate(mapIceChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Field transition and start");
            }
            if(tempSpawnList[5] == 5)
            {
                Instantiate(mapIceChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapIceChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapLavaStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapLavaStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Ice->Lava transition and start");
            }
            
        }
        if(tempSpawnList[4] == 5)
        {
            Debug.Log("Checking Lava to ...");
            if(tempSpawnList[5] == 0)
            {
                Instantiate(mapLavaChangeAllPrefab[0], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[0].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapCityStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapCityStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->City transition and start");
            }
            if(tempSpawnList[5] == 1)
            {
                Instantiate(mapLavaChangeAllPrefab[1], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[1].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapBeachStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapBeachStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Beach transition and start");
            }
            if(tempSpawnList[5] == 2)
            {
                Instantiate(mapLavaChangeAllPrefab[2], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[2].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapDesertStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapDesertStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Desert transition and start");
            }
            if(tempSpawnList[5] == 3)
            {
                Instantiate(mapLavaChangeAllPrefab[3], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[3].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapFieldStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapFieldStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Field transition and start");
            }
            if(tempSpawnList[5] == 4)
            {
                Instantiate(mapLavaChangeAllPrefab[4], new Vector3(spawnRangeX, -32.1f, 0f), mapLavaChangeAllPrefab[4].transform.rotation);
                spawnRangeX += 30f;
                Instantiate(mapIceStartPrefab, new Vector3(spawnRangeX, -4.195f, 0f), mapIceStartPrefab.transform.rotation);
                spawnRangeX += 30f;
                Debug.Log("Spawned Lava->Ice transition and start");
            }
        }
    
    }

}