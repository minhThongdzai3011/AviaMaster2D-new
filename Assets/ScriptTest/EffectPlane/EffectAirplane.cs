using UnityEngine;


public class EffectAirplane : MonoBehaviour
{
    public static EffectAirplane instance;

    void Start()
    {
        instance = this;
        
    }

    // Hàm làm đen máy bay
    public void MakePlaneBlack()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material = Plane.instance.blackMaterial;
        }
    }
        public void MakePlaneGold()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material = Plane.instance.GoldMaterial;
        }
    }
}