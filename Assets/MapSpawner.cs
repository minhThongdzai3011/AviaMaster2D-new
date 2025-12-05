
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

    void Start()
    {
        

        if (isMapCityUnlocked && !isMapBeachUnlocked)
        {
            mapPrefabs = mapCityPrefabs;
            isCityMap = true;
            mapStart[0].SetActive(true);
            checkMapStartCitySpawned = true;  // *** SỬA: true vì City đã được chọn ***
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
    public int countMapSpawned = 0;
    public bool stopCheckSpawnRate01 = true;
    public bool stopCheckSpawnRate12 = true;
    public bool stopCheckSpawnRate23 = true;
    public bool stopCheckSpawnRate34 = true;
    public bool stopCheckSpawnRate45 = true;

    void Update()
    {
        countMapSpawned = GameObject.FindGameObjectsWithTag("Map").Length;
        if (countMapSpawned <= 20)
        {
            stopCheckSpawnRate01 = false;
        }
        else if (countMapSpawned > 20 && countMapSpawned <= 40)
        {
            stopCheckSpawnRate12 = false;
        }
        else if (countMapSpawned > 40 && countMapSpawned <= 60)
        {
            stopCheckSpawnRate23 = false;
        }
        else if (countMapSpawned > 60 && countMapSpawned <= 80)
        {
            stopCheckSpawnRate34 = false;
        }
        else if (countMapSpawned > 80 && countMapSpawned <= 100)
        {
            stopCheckSpawnRate45 = false;
        }
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

        bool needSpawnTransition = false;
        
        if (isCityMap && !checkMapStartCitySpawned)
        {
            // Spawn Beach transition và start cho City cycle
            CheckSpawnChangeMap();
            checkMapStartCitySpawned = true;
            needSpawnTransition = true;
            Debug.Log("Spawned City->Beach transition");
        }
        else if (isBeachMap && !checkMapStartBeachSpawned)
        {
            // Spawn Desert transition và start cho Beach cycle  
            CheckSpawnChangeMap();
            checkMapStartBeachSpawned = true;
            needSpawnTransition = true;
            Debug.Log("Spawned Beach->Desert transition");
        }
        else if (isDesertMap && !checkMapStartDesertSpawned)
        {
            // Spawn Field transition và start cho Desert cycle
            CheckSpawnChangeMap();
            checkMapStartDesertSpawned = true;
            needSpawnTransition = true;
            Debug.Log("Spawned Desert->Field transition");
        }
        else if (isFieldMap && !checkMapStartFieldSpawned)
        {
            // Spawn Ice transition và start cho Field cycle
            CheckSpawnChangeMap();
            checkMapStartFieldSpawned = true;
            needSpawnTransition = true;
            Debug.Log("Spawned Field->Ice transition");
        }
        else if (isIceMap && !checkMapStartIceSpawned)
        {
            // Spawn Lava transition và start cho Ice cycle
            CheckSpawnChangeMap();
            checkMapStartIceSpawned = true;
            needSpawnTransition = true;
            Debug.Log("Spawned Ice->Lava transition");
        }
        else if (isLavaMap && !checkMapStartLavaSpawned)
        {
            // Lava không có transition tiếp theo, chỉ đánh dấu
            CheckMap45();
            checkMapStartLavaSpawned = true;
            needSpawnTransition = true;
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
            spawnRangeYmin = spawnRangeYCityMin; 
        }
        else if (isBeachMap)
        {
            spawnRangeYmin = spawnRangeYBeachMin;
        }
        else if (isDesertMap)
        {
            spawnRangeYmin = spawnRangeYDesertMin; 
        }
        else if (isFieldMap)
        {
            spawnRangeYmin = spawnRangeYFieldMin;
        }
        else if (isIceMap)
        {
            spawnRangeYmin = spawnRangeYIceMin;
        }
        else if (isLavaMap)
        {
            spawnRangeYmin = spawnRangeYLavaMin;
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
        int countSpawned = GameObject.FindGameObjectsWithTag("Map").Length;

        int index = countSpawned / 20;   // Mỗi 20 map → đổi map
        if (index >= tempSpawnList.Count)
            return;

        int nextMap = tempSpawnList[index];
        SwitchToNextMap(nextMap);
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
        Debug.Log("Kết quả cuối cùng: " + string.Join(",", tempSpawnList));

    }

    public void CheckSpawnChangeMap()
    {
        // *** THÊM: Kiểm tra an toàn - nếu chỉ có 1 map unlocked, không spawn transition ***
        if (tempSpawnList.Count == 0)
        {
            Debug.Log("tempSpawnList rỗng - không spawn ChangeMap");
            return;
        }

        if (!stopCheckSpawnRate01)
        {
            CheckMap01();
            stopCheckSpawnRate01 = true;
            return;
        }
        if (!stopCheckSpawnRate12)
        {
            CheckMap12();
            stopCheckSpawnRate12 = true;
            return;
        }
        if (!stopCheckSpawnRate23)
        {
            CheckMap23();
            stopCheckSpawnRate23 = true;
            return;
        }
        if (!stopCheckSpawnRate34)
        {
            CheckMap34();
            stopCheckSpawnRate34 = true;
            return;
        }
        if (!stopCheckSpawnRate45)
        {
            CheckMap45();
            stopCheckSpawnRate45 = true;
            return;
        }

    }

        void SwitchToNextMap(int mapType)
    {
        ResetAllMapFlags();

        switch (mapType)
        {
            case 0: mapPrefabs = mapCityPrefabs; isCityMap = true; break;
            case 1: mapPrefabs = mapBeachPrefabs; isBeachMap = true; break;
            case 2: mapPrefabs = mapDesertPrefabs; isDesertMap = true; break;
            case 3: mapPrefabs = mapFieldPrefabs; isFieldMap = true; break;
            case 4: mapPrefabs = mapIcePrefabs; isIceMap = true; break;
            case 5: mapPrefabs = mapLavaPrefabs; isLavaMap = true; break;
        }
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
