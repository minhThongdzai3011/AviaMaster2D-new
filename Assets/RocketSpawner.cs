using System.Collections;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class RocketSpawner : MonoBehaviour
{
    public static RocketSpawner instance;

    [Header("Vật phẩm Rocket")]
    public GameObject[] rocketPrefabs;
    public GameObject[] diamondPrefabs;

    [Header("Spawn Range 2D")]
    public float spawnRangeYmin = -2f;
    public float spawnRangeYmax = 2f;

    [Header("Spawn Settings")]
    public float startDelay = 0f;
    public float spawnInterval = 0.2f;
    public int count = 0;   
    public int maxRocketItems = 20;
    public float rangeXmin = 60f;
    public float rangeXmax = 120f;

    private int deletedCoinCount = 0;
    private int coinSpawnCount = 0;


    private CinemachineVirtualCamera mainCamera;
    private Vector3 cameraPos;

    void Start()
    {
        instance = this;
        mainCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        cameraPos = mainCamera.transform.position;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnRocketCoroutine());
        Debug.Log("Rocket Spawning Started");
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
        if (!GManager.instance.isControllable) return;

        int prefabIndex = Random.Range(0, rocketPrefabs.Length);

        float offsetX = Random.Range(rangeXmin, rangeXmax);
        float offsetY = Random.Range(spawnRangeYmin, spawnRangeYmax);

        Vector3 spawnPosition = new Vector3(
            cameraPos.x + offsetX,
            cameraPos.y + offsetY,
            cameraPos.z
        );

        Instantiate(
            rocketPrefabs[prefabIndex],
            spawnPosition,
            rocketPrefabs[prefabIndex].transform.rotation
        );

        coinSpawnCount++;

        if (coinSpawnCount >= 3)
        {
            coinSpawnCount = 0;
            SpawnDiamond();
        }
    }


    public void DeleteSpawnedItems()
    {
        GameObject[] spawnedItems = GameObject.FindGameObjectsWithTag("Coin");

        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);

            deletedCoinCount++;

            // Cứ 5 coin bị xóa thì spawn 1 diamond
            if (deletedCoinCount >= 5)
            {
                deletedCoinCount = 0;
                SpawnDiamond();
            }
        }
    }


    void SpawnDiamond()
    {
        if (diamondPrefabs == null || diamondPrefabs.Length == 0) return;
        if (!GManager.instance.isControllable) return;

        int prefabIndex = Random.Range(0, diamondPrefabs.Length);

        float offsetX = Random.Range(rangeXmin, rangeXmax);
        float offsetY = Random.Range(spawnRangeYmin, spawnRangeYmax);

        Vector3 spawnPosition = new Vector3(
            cameraPos.x + offsetX,
            cameraPos.y + offsetY,
            cameraPos.z
        );

        Instantiate(
            diamondPrefabs[prefabIndex],
            spawnPosition,
            diamondPrefabs[prefabIndex].transform.rotation
        );

        Debug.Log("Diamond Spawned (after 5 coins)");
    }


}