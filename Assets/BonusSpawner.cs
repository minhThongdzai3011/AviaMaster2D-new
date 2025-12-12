using System.Collections;
using UnityEngine;
using Cinemachine;

public class BonusSpawner : MonoBehaviour
{
    public static BonusSpawner instance;

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
    public Vector3 someVector;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (Plane.instance != null)
            someVector = Plane.instance.transform.position;
        spawnRangeX = Mathf.Max(spawnRangeX, someVector.x + 100f);
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
            count = GameObject.FindGameObjectsWithTag("bird").Length;

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
        if (GManager.instance.isControllable)
        {
            int prefabIndex = GetControlledRandomIndex();
            Vector3 spawnPosition = new Vector3(
                someVector.x + spawnRangeX,
                Random.Range(someVector.y - 5, someVector.y + 5),
                someVector.z
            );

            Instantiate(rocketPrefabs[prefabIndex], spawnPosition, rocketPrefabs[prefabIndex].transform.rotation);
            spawnRangeX += Random.Range(rangeXmin, rangeXmax);
        }
    }

    public void DeleteSpawnedItems()
    {
        GameObject[] spawnedItems = GameObject.FindGameObjectsWithTag("bird");
        foreach (GameObject item in spawnedItems)
            Destroy(item);

        spawnRangeX = startRangeX;
    }
    int GetControlledRandomIndex()
    {
        // Tỉ lệ % 
        float bonusRate = 0.6f;   // 60%
        float fogRate = 0.25f;    // 25%
        //float bombRate = 0.15f;   // 15%

        float r = Random.value;   // từ 0 → 1

        if (r < bonusRate)
            return 0;            // Bonus

        if (r < bonusRate + fogRate)
            return 1;            // Sương mù

        return 2;                // Bom
    }

}
