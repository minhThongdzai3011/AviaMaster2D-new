using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuperPlaneManager : MonoBehaviour
{
    public static SuperPlaneManager instance;
    public bool isSuperPlane1 = false;
    public bool isSuperPlane2 = false;
    public bool isSuperPlane3 = false;
    public bool isSuperPlane4 = false;
    public bool isSuperPlane5 = false;

    public Image imageSkillSuperPlane2;
    public GameObject gameObjectBulletPlane;
    public Image imageSkillSuperPlane5;



    public bool skillPlane5 = true;
    

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuffBoostandFuelSuperPlane1()
    {
        if(isSuperPlane1)
        {
            float temp =  GManager.instance.durationFuel ;
            float temp2 = GManager.instance.boostedAltitude ;
            GManager.instance.durationFuel = GManager.instance.durationFuel * 1.05f;
            GManager.instance.boostedAltitude = GManager.instance.boostedAltitude * 1.2f;
            Debug.Log("Buff Boost and Fuel Super Plane 1 Active: " + temp + " - " + temp2 + " -> " + GManager.instance.durationFuel + " - " + GManager.instance.boostedAltitude);
        }
    }

    public void ResetBuffBoostandFuelSuperPlane1()
    {
        if (isSuperPlane1)
        {
            float temp = GManager.instance.durationFuel;
            float temp2 = GManager.instance.boostedAltitude;
            GManager.instance.durationFuel = GManager.instance.durationFuel / 1.05f;
            GManager.instance.boostedAltitude = GManager.instance.boostedAltitude / 1.2f;
            Debug.Log("Reset Buff Boost and Fuel Super Plane 1 Active: " + temp + " - " + temp2 + " -> " + GManager.instance.durationFuel + " - " + GManager.instance.boostedAltitude);
        }
    }

    public void BuffBoostandFuelSuperPlane2()
    {
        if (isSuperPlane2)
        {
            float temp = GManager.instance.durationFuel;
            float temp2 = GManager.instance.boostedAltitude;
            GManager.instance.durationFuel = GManager.instance.durationFuel * 1.05f;
            GManager.instance.boostedAltitude = GManager.instance.boostedAltitude * 1.05f;
            Debug.Log("Buff Boost and Fuel Super Plane 5 Active: " + temp + " - " + temp2 + " -> " + GManager.instance.durationFuel + " - " + GManager.instance.boostedAltitude);
        }
    }

    public void ResetBuffBoostandFuelSuperPlane2()
    {
        if (isSuperPlane2)
        {
            float temp = GManager.instance.durationFuel;
            float temp2 = GManager.instance.boostedAltitude;
            GManager.instance.durationFuel = GManager.instance.durationFuel / 1.05f;
            GManager.instance.boostedAltitude = GManager.instance.boostedAltitude / 1.05f;
            Debug.Log("Reset Buff Boost and Fuel Super Plane 5 Active: " + temp + " - " + temp2 + " -> " + GManager.instance.durationFuel + " - " + GManager.instance.boostedAltitude);
        }
    }

    public void BuffBoostandFuelSuperPlane3()
    {
        if (isSuperPlane3)
        {
            float temp = GManager.instance.durationFuel;
            float temp2 = GManager.instance.boostedAltitude;
            GManager.instance.durationFuel = GManager.instance.durationFuel * 1.05f;
            GManager.instance.boostedAltitude = GManager.instance.boostedAltitude * 1.05f;
            Debug.Log("Buff Boost and Fuel Super Plane 5 Active: " + temp + " - " + temp2 + " -> " + GManager.instance.durationFuel + " - " + GManager.instance.boostedAltitude);
        }
    }
    public void ResetBuffBoostandFuelSuperPlane3()
    {
        if (isSuperPlane3)
        {
            float temp = GManager.instance.durationFuel;
            float temp2 = GManager.instance.boostedAltitude;
            GManager.instance.durationFuel = GManager.instance.durationFuel / 1.05f;
            GManager.instance.boostedAltitude = GManager.instance.boostedAltitude / 1.05f;
            Debug.Log("Reset Buff Boost and Fuel Super Plane 5 Active: " + temp + " - " + temp2 + " -> " + GManager.instance.durationFuel + " - " + GManager.instance.boostedAltitude);
        }
    }

    public void BuffBoostandFuelSuperPlane4()
    {
        if (isSuperPlane4)
        {
            float temp = GManager.instance.durationFuel;
            float temp2 = GManager.instance.boostedAltitude;
            GManager.instance.durationFuel = GManager.instance.durationFuel * 1.2f;
            GManager.instance.boostedAltitude = GManager.instance.boostedAltitude * 1.05f;
            Debug.Log("Buff Boost and Fuel Super Plane 5 Active: " + temp + " - " + temp2 + " -> " + GManager.instance.durationFuel + " - " + GManager.instance.boostedAltitude);
        }
    }

    public void ResetBuffBoostandFuelSuperPlane4()
    {
        if (isSuperPlane4)
        {
            float temp = GManager.instance.durationFuel;
            float temp2 = GManager.instance.boostedAltitude;
            GManager.instance.durationFuel = GManager.instance.durationFuel / 1.2f;
            GManager.instance.boostedAltitude = GManager.instance.boostedAltitude / 1.05f;
            Debug.Log("Reset Buff Boost and Fuel Super Plane 5 Active: " + temp + " - " + temp2 + " -> " + GManager.instance.durationFuel + " - " + GManager.instance.boostedAltitude);
        }
    }

    public void BuffBoostandFuelSuperPlane5()
    {
        if (isSuperPlane5)
        {
            float temp = GManager.instance.durationFuel;
            float temp2 = GManager.instance.boostedAltitude;
            GManager.instance.durationFuel = GManager.instance.durationFuel * 1.05f;
            GManager.instance.boostedAltitude = GManager.instance.boostedAltitude * 1.05f;
            Debug.Log("Buff Boost and Fuel Super Plane 5 Active: " + temp + " - " + temp2 + " -> " + GManager.instance.durationFuel + " - " + GManager.instance.boostedAltitude);
        }
    }

    public void ResetBuffBoostandFuelSuperPlane5()
    {
        if (isSuperPlane5)
        {
            float temp = GManager.instance.durationFuel;
            float temp2 = GManager.instance.boostedAltitude;
            GManager.instance.durationFuel = GManager.instance.durationFuel / 1.05f;
            GManager.instance.boostedAltitude = GManager.instance.boostedAltitude / 1.05f;
            Debug.Log("Reset Buff Boost and Fuel Super Plane 5 Active: " + temp + " - " + temp2 + " -> " + GManager.instance.durationFuel + " - " + GManager.instance.boostedAltitude);
        }
    }

    
}
