using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBonusStart : MonoBehaviour
{
    public static RandomBonusStart instance;
    public GameObject[] bonusPrefabs;
    // Start is called before the first frame update
    void Start()
    {

        instance = this;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnRandomBonusAtLaunch()
    {
        int index = Random.Range(0, bonusPrefabs.Length);
        Instantiate(bonusPrefabs[index],
         new Vector3(60, GManager.instance.airplaneRigidbody2D.position.y , 0)
         , Quaternion.identity);
         Debug.Log("Spawned Random Bonus at Launch - position: " + new Vector3(60, GManager.instance.airplaneRigidbody2D.position.y, 0));
    }
}
