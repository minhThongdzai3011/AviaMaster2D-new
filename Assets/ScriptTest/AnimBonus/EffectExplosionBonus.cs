using UnityEngine;
using System.Collections.Generic;

public class EffectExplosionBonus : MonoBehaviour
{
    public static EffectExplosionBonus instance;
    public GameObject Obj;
    public GameObject Obj1;

    public void Awake()
    {
        instance = this;
    }
    public void ExplosionEffect()
    {
        Debug.Log("Explosion Effect Triggered");
        Obj.gameObject.SetActive(true);
    }

    public void ExplosionEffect1()
    {
        Debug.Log("Explosion Effect1 Triggered");
        Obj1.gameObject.SetActive(true);
    }


}