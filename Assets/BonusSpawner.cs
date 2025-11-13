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
    public float startDelay = 0f;
    public float spawnInterval = 0.2f;
    public int count = 0;
    public int maxSpawnedItems = 10;
    public float rangeXmin = 5f;
    public float rangeXmax = 10f;
    public float startRangeX = -18f;

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
            count = GameObject.FindGameObjectsWithTag("bird").Length; 
            if (count < maxSpawnedItems)
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
        // Debug.Log("Spawned Bonus Item at X: " + spawnRangeX + " Y: " + spawnPosition.y + " | Prefab Index: " + prefabIndex);
        spawnRangeX += Random.Range(rangeXmin, rangeXmax);
    }

    public void DeleteSpawnedItems()
    {
        List<GameObject> allBonusItems = new List<GameObject>();
        
        // Danh sách các tag bonus cần xóa
        string[] bonusTags = { "bird" };
        
        // Tìm và thêm tất cả các object có tag bonus vào list
        foreach (string tag in bonusTags)
        {
            GameObject[] itemsWithTag = GameObject.FindGameObjectsWithTag(tag);
            if (itemsWithTag != null && itemsWithTag.Length > 0)
            {
                allBonusItems.AddRange(itemsWithTag);
                Debug.Log($"Tìm thấy {itemsWithTag.Length} items với tag '{tag}'");
            }
        }
        
        if (allBonusItems.Count == 0)
        {
            Debug.Log("Không tìm thấy bonus items nào để xóa.");
            // return; // No items to delete
        }
        
        Debug.Log($"Tổng cộng {allBonusItems.Count} bonus items sẽ bị xóa");
        
        // Xóa tất cả bonus items
        foreach (GameObject item in allBonusItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }

        spawnRangeX = startRangeX; // Reset spawnRangeX to initial value
        Debug.Log("GameObj" + gameObject.name);

        Debug.Log("Đã xóa tất cả bonus items và reset spawnRangeX");
    }

    
}