using System.Collections;
using UnityEngine;
using Cinemachine;

public class DiamondHigherSpawn : MonoBehaviour
{
    public static DiamondHigherSpawn instance;

    [Header("Vật phẩm Rocket")]
    public GameObject[] BonusPrefabs;

    [Header("Spawn Range 2D")]
    public float spawnRangeYmin = -2f;
    public float spawnRangeYmax = 2f;
    public float spawnRangeX = -14f;

    [Header("Spawn Settings")]
    public float startDelay = 0f;
    public float spawnInterval = 0.3f;
    public int count = 0;
    public int maxRocketItems = 20;
    public float rangeXmin = 5f;
    public float rangeXmax = 10f;
    public float startRangeX = -14f;
    public Vector3 someVector;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (Plane.instance != null && Plane.instance.transform.position.y > 200f)
            someVector = Plane.instance.transform.position;
    }

    public void StartSpawning() 
    {
        StartCoroutine(SpawnRocketCoroutine());
        Debug.Log("Diamond Spawning 1 Started");
    }

    IEnumerator SpawnRocketCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        while (true)
        {
            count = GameObject.FindGameObjectsWithTag("Diamond").Length;

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
        if (BonusPrefabs.Length == 0) return;
        if (!GManager.instance.isControllable) return;
        if (Plane.instance == null) return;

        int prefabIndex = Random.Range(0, BonusPrefabs.Length);

        Vector3 planePos = Plane.instance.transform.position;

        Vector3 spawnPosition = new Vector3(
            planePos.x + Random.Range(rangeXmin, rangeXmax), 
             Random.Range(spawnRangeYmin, spawnRangeYmax),    
            planePos.z
        );

        Instantiate(
            BonusPrefabs[prefabIndex],
            spawnPosition,
            Quaternion.identity
        );

        Debug.Log("Spawned Bonus at: " + spawnPosition);
    }


    public void DeleteSpawnedItems()
    {
        GameObject[] spawnedItems = GameObject.FindGameObjectsWithTag("Respawn");
        foreach (GameObject item in spawnedItems)
            Destroy(item);

        spawnRangeX = startRangeX;
    }


}
