using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class EffectExplosionBonus : MonoBehaviour
{
   // public static EffectExplosionBonus instance;
    public GameObject Obj;
    public GameObject Obj1;

    public void Awake()
    {
      //  instance = this;
    }
    [Button]
    public void ExplosionEffect()
    {
        Debug.Log("Explosion Effect Triggered");
        Obj.gameObject.SetActive(true);
    }

    public void ExplosionEffect1()
    {
        Debug.Log("Explosion Effect1 Triggered");
        Obj1.gameObject.SetActive(false);
    }
}