using System.Collections;
using UnityEngine;
using Cinemachine;


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
    public CinemachineVirtualCamera mainCamera;
    public Vector3 someVector;

    void Start()
    {
        instance = this;
        mainCamera = Camera.main.GetComponent<CinemachineVirtualCamera>();
        
    }

    void Update()
    {
        
        someVector = mainCamera.transform.position;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnRocketCoroutine());
        Debug.Log("Rocket Spawning Started");
    }

    IEnumerator SpawnRocketCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        Debug.Log("Starting Rocket Spawn Coroutine");
        while (true)
        {
            count = GameObject.FindGameObjectsWithTag("Coin").Length;
            if (count < maxRocketItems)
            {
                SpawnRocketItem();
                Debug.Log("Rocket item spawned");
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
        if (GManager.instance.isControllable)
        {
            int prefabIndex = Random.Range(0, rocketPrefabs.Length);
            Vector3 spawnPosition = new Vector3(spawnRangeX,
                                                Random.Range(someVector.y-5, someVector.y+5),
                                                someVector.z); 
            
            Instantiate(rocketPrefabs[prefabIndex], spawnPosition, rocketPrefabs[prefabIndex].transform.rotation);
            spawnRangeX += Random.Range(rangeXmin, rangeXmax);
            Debug.Log("Rocket Spawned at X: " + spawnRangeX + " Y: " + spawnPosition.y + " | Prefab Index: " + prefabIndex);
            return;
        }
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
