using UnityEngine;


public class EffectAirplane : MonoBehaviour
{
    public static EffectAirplane instance;
    private Material originalMaterial;
    private Renderer planeRenderer;

    void Start()
    {
        instance = this;
        planeRenderer = GetComponentInChildren<Renderer>();
        if (planeRenderer != null)
        {
            originalMaterial = planeRenderer.material;
        }
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

    public void RestoreOriginalMaterial()
    {
        if (planeRenderer != null && originalMaterial != null)
        {
            planeRenderer.material = originalMaterial;
        }
    }
}