using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBonusMap : MonoBehaviour
{
    public static ChangeBonusMap instance;
    [Header("Bonus Map City")]
    public GameObject[] bonusMapCity;
    [Header("Bonus Map Beach")]
    public GameObject[] bonusMapBeach;
    [Header("Bonus Map Desert")]
    public GameObject[] bonusMapDesert;
    [Header("Bonus Map Field")]
    public GameObject[] bonusMapField;
    [Header("Bonus Map Ice")]
    public GameObject[] bonusMapIce;
    [Header("Bonus Map Lava")]
    public GameObject[] bonusMapLava;
    public  bool isChangeBonusMap = false;

    public List<int> mapList = new List<int>();
    public bool isBonusMapCity = false;
    public bool isBonusMapBeach = false;
    public bool isBonusMapDesert = false;
    public bool isBonusMapField = false;
    public bool isBonusMapIce = false;
    public bool isBonusMapLava = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isChangeBonusMap)
        {
            Debug.Log("Chuyển đổi Bonus Map: " + string.Join(",", mapList));
            switch (mapList[0])
            {
                case 0:
                    BonusSpawner.instance.rocketPrefabs = bonusMapCity;
                    isBonusMapCity = true;
                    isChangeBonusMap = false;
                    break;
                case 1:
                    BonusSpawner.instance.rocketPrefabs = bonusMapBeach;
                    isBonusMapBeach = true;
                    isChangeBonusMap = false;
                    break;
                case 2:
                    BonusSpawner.instance.rocketPrefabs = bonusMapDesert;
                    isBonusMapDesert = true;
                    isChangeBonusMap = false;
                    break;
                case 3:
                    BonusSpawner.instance.rocketPrefabs = bonusMapField;
                    isBonusMapField = true;
                    isChangeBonusMap = false;
                    break;
                case 4:
                    BonusSpawner.instance.rocketPrefabs = bonusMapIce;
                    isBonusMapIce = true;
                    isChangeBonusMap = false;
                    break;
                case 5:
                    BonusSpawner.instance.rocketPrefabs = bonusMapLava;
                    isBonusMapLava = true;
                    isChangeBonusMap = false;
                    break;
            }
        }
    }
}
