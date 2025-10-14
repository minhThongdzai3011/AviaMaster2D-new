using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    public static ShipSpawner instance;
    [Header("Vật phẩm Tàu")]
    public GameObject[] shipPrefabs;
    private float spawnRangeY = -5f;
    private float spawnRangeX = 25f;

    [Header("Spawn Settings")]
    private float startDelay = 0f;
    private float spawnInterval = 0.2f;

    public int count = 0;
    void Start()
    {
        instance = this;
        StartCoroutine(SpawnShipCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator SpawnShipCoroutine()
    {
        
        yield return new WaitForSeconds(startDelay);
        Debug.Log("count: " + count);

        while (true)
        {
            count = GameObject.FindGameObjectsWithTag("Ship").Length;
            if (count < 10)
            {
                SpawnShipItem();
                yield return new WaitForSeconds(spawnInterval);
            }
            else
            {
                yield return null;
            } 
        }
    }
    void SpawnShipItem()
    {
        int prefabIndex = Random.Range(0, shipPrefabs.Length);
        Vector3 spawnPosition = new Vector3(spawnRangeX,
                                            spawnRangeY,
                                            0);
        Instantiate(shipPrefabs[prefabIndex], spawnPosition, shipPrefabs[prefabIndex].transform.rotation);
        
        spawnRangeX += 25f;
    }

    public void DeleteSpawnedItems()
    {
        GameObject[] spawnedItems = GameObject.FindGameObjectsWithTag("Ship");
        if (spawnedItems == null || spawnedItems.Length == 0)
        {
            Debug.Log("No spawned items found with tag 'Ship'.");
            return; // No items to delete
        }
        Debug.Log("spawnedItems.Length: " + (spawnedItems?.Length ?? 0));
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        spawnRangeX = 25; // Reset spawnRangeX to initial value
    }

    
}