using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSpawner : MonoBehaviour
{
    public static BonusSpawner instance;
    [Header("Vật phẩm Bonus")]
    public GameObject[] bonusPrefabs;
    public float spawnRangeYmin = -2f;
    public float spawnRangeYmax = 2f;
    public float spawnRangeX = -18f;

    [Header("Spawn Settings")]
    private float startDelay = 0f;
    private float spawnInterval = 0.2f;
    public int count = 0;
    void Start()
    {
        instance = this;
        StartCoroutine(SpawnBonusCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator SpawnBonusCoroutine()
    {
        yield return new WaitForSeconds(startDelay);
        Debug.Log("count: " + count);

        while (true)
        {
            count = GameObject.FindGameObjectsWithTag("Bonus").Length;
            if (count < 20)
            {
                SpawnBonusItem();
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                yield return null;
            } 
        }
    }
    void SpawnBonusItem()
    {
        int prefabIndex = Random.Range(0, bonusPrefabs.Length);
        Vector3 spawnPosition = new Vector3(spawnRangeX,
                                            Random.Range(spawnRangeYmin, spawnRangeYmax),
                                            0);
        Instantiate(bonusPrefabs[prefabIndex], spawnPosition, bonusPrefabs[prefabIndex].transform.rotation);
        spawnRangeX += Random.Range(5f, 10f);
    }

    public void DeleteSpawnedItems()
    {
        GameObject[] spawnedItems = GameObject.FindGameObjectsWithTag("Bonus");
        if (spawnedItems == null || spawnedItems.Length == 0)
        {
            Debug.Log("No spawned items found with tag 'Bonus'.");
            return; // No items to delete
        }
        Debug.Log("spawnedItems.Length: " + (spawnedItems?.Length ?? 0));
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        spawnRangeX = -18f; // Reset spawnRangeX to initial value
    }

    
}