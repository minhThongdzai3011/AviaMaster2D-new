using System.Collections;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class RocketSpawner : MonoBehaviour
{
    public static RocketSpawner instance;

    [Header("Vật phẩm Rocket")]
    public GameObject[] rocketPrefabs;

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
    }

    public void DeleteSpawnedItems()
    {
        GameObject[] spawnedItems = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject item in spawnedItems)
            Destroy(item);
    }
}