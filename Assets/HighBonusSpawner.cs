using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighBonusSpawner : MonoBehaviour
{
    public static HighBonusSpawner instance;
    [Header("Vật phẩm Bonus")]
    public GameObject[] bonusPrefabs;
    public float spawnRangeYmin = 5f;
    public float spawnRangeYmax = 10f;
    public float spawnRangeX = -12f;

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
            count = GameObject.FindGameObjectsWithTag("Bonus2").Length + GameObject.FindGameObjectsWithTag("Bonus3").Length + GameObject.FindGameObjectsWithTag("Bonus4").Length + GameObject.FindGameObjectsWithTag("Bonus5").Length + GameObject.FindGameObjectsWithTag("Bonus10").Length;
            if (count < 30)
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
        List<GameObject> allBonusItems = new List<GameObject>();
        
        // Danh sách các tag bonus cần xóa
        string[] bonusTags = { "Bonus2", "Bonus3", "Bonus4", "Bonus5", "Bonus10" };
        
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
            return; // No items to delete
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
        
        spawnRangeX = -12f; // Reset spawnRangeX to initial value
        Debug.Log("Đã xóa tất cả bonus items và reset spawnRangeX");
    }

    
}