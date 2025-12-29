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

    public Image imageSkillSuperPlane1;
    public Image imageSkillSuperPlane5;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
