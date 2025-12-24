using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneManage : MonoBehaviour
{
    [Header ("Plane Materials")]
    public static PlaneManage instance;
    public Material planeMaterial1;
    public Material planeMaterial2;
    public Material planeMaterial3;
    public Material planeMaterial4;
    public Material planeMaterial5;
    public Material planeMaterial6;
    public Material planeMaterial7;
    public Material planeMaterial8;
    public Material planeMaterial9;
    public Material planeMaterial10;
    public Material planeMaterial11;
    public Material planeMaterial12;
    public Material planeMaterial13;
    public Material planeMaterial14;



    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMaterial(int index)
    {
        switch (index)
        {
            case 0:
                Plane.instance.GoldMaterial = planeMaterial1;
                Debug.Log("Changed to Material 1");
                break;
            case 1:
                Plane.instance.GoldMaterial = planeMaterial2;
                Debug.Log("Changed to Material 2");
                break;
            case 2:
                Plane.instance.GoldMaterial = planeMaterial3;
                Debug.Log("Changed to Material 3");
                break;
            case 3:
                Plane.instance.GoldMaterial = planeMaterial4;
                Debug.Log("Changed to Material 4");
                break;
            case 4:
                Plane.instance.GoldMaterial = planeMaterial5;
                Debug.Log("Changed to Material 5");
                break;
            case 5:
                Plane.instance.GoldMaterial = planeMaterial6;
                Debug.Log("Changed to Material 6");
                break;
            case 6:
                Plane.instance.GoldMaterial = planeMaterial7;
                Debug.Log("Changed to Material 7");
                break;
            case 7:
                Plane.instance.GoldMaterial = planeMaterial8;
                Debug.Log("Changed to Material 8");
                break;
            case 8:
                Plane.instance.GoldMaterial = planeMaterial9;
                Debug.Log("Changed to Material 9");
                break;
            case 9:
                Plane.instance.GoldMaterial = planeMaterial10;
                Debug.Log("Changed to Material 10");
                break;
            case 10:
                Plane.instance.GoldMaterial = planeMaterial11;
                Debug.Log("Changed to Material 11");
                break;
            case 11:
                Plane.instance.GoldMaterial = planeMaterial12;
                Debug.Log("Changed to Material 12");
                break;
            case 12:
                Plane.instance.GoldMaterial = planeMaterial13;
                Debug.Log("Changed to Material 13");
                break;
            case 13:
                Plane.instance.GoldMaterial = planeMaterial14;
                Debug.Log("Changed to Material 14");
                break;
            default:
                break;
        }
    // }
    // public void ChangeColorTrail(int index)
    // {
    //     switch (index)
    //     {
    //         case 0:
    //             Gradient g0 = new Gradient();
    //             g0.SetKeys(
    //                 new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.green, 0.5f), new GradientColorKey(Color.cyan, 1.0f) },
    //                 new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g0;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g0;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 1 (Yellow → Green → Cyan).");
    //             break;

    //         case 1:
    //             Gradient g1 = new Gradient();
    //             g1.SetKeys(
    //                 new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.magenta, 0.5f), new GradientColorKey(Color.blue, 1.0f) },
    //                 new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.6f, 0.5f), new GradientAlphaKey(0f, 1f) }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g1;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g1;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 2 (Red → Magenta → Blue).");
    //             break;

    //         case 2:
    //             Gradient g2 = new Gradient();
    //             g2.SetKeys(
    //                 new GradientColorKey[] { new GradientColorKey(Color.cyan, 0.0f), new GradientColorKey(Color.blue, 0.5f), new GradientColorKey(Color.white, 1.0f) },
    //                 new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.7f, 0.5f), new GradientAlphaKey(0f, 1f) }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g2;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g2;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 3 (Cyan → Blue → White).");
    //             break;

    //         case 3:
    //             Gradient g3 = new Gradient();
    //             g3.SetKeys(
    //                 new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.5f, 0f), 0.0f), new GradientColorKey(Color.red, 0.5f), new GradientColorKey(Color.black, 1.0f) },
    //                 new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g3;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g3;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 4 (Orange → Red → Black).");
    //             break;

    //         case 4:
    //             Gradient g4 = new Gradient();
    //             g4.SetKeys(
    //                 new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(new Color(0.5f, 1f, 0.5f), 0.5f), new GradientColorKey(Color.white, 1.0f) },
    //                 new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.6f, 0.5f), new GradientAlphaKey(0f, 1f) }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g4;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g4;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 5 (Green → Light Green → White).");
    //             break;

    //         case 5:
    //             Gradient g5 = new Gradient();
    //             g5.SetKeys(
    //                 new GradientColorKey[] { new GradientColorKey(Color.magenta, 0.0f), new GradientColorKey(new Color(1f, 0.5f, 0.8f), 0.5f), new GradientColorKey(Color.yellow, 1.0f) },
    //                 new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g5;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g5;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 6 (Magenta → Pink → Yellow).");
    //             break;

    //         case 6:
    //             Gradient g6 = new Gradient();
    //             g6.SetKeys(
    //                 new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(new Color(0.3f, 0.7f, 1f), 0.5f), new GradientColorKey(Color.cyan, 1.0f) },
    //                 new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.6f, 0.5f), new GradientAlphaKey(0f, 1f) }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g6;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g6;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 7 (Blue → Sky Blue → Cyan).");
    //             break;

    //         case 7:
    //             Gradient g7 = new Gradient();
    //             g7.SetKeys(
    //                 new GradientColorKey[] { new GradientColorKey(new Color(0.8f, 0.2f, 0.2f), 0.0f), new GradientColorKey(new Color(1f, 0.6f, 0.2f), 0.5f), new GradientColorKey(Color.yellow, 1.0f) },
    //                 new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g7;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g7;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 8 (Dark Red → Orange → Yellow).");
    //             break;

    //         case 8:
    //             Gradient g8 = new Gradient();
    //             g8.SetKeys(
    //                 new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.gray, 0.5f), new GradientColorKey(Color.black, 1.0f) },
    //                 new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.4f, 0.5f), new GradientAlphaKey(0f, 1f) }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g8;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g8;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 9 (White → Gray → Black).");
    //             break;

    //         case 9:
    //             Gradient g9 = new Gradient();
    //             g9.SetKeys(
    //                 new GradientColorKey[] { new GradientColorKey(new Color(0.5f, 0f, 1f), 0.0f), new GradientColorKey(Color.magenta, 0.5f), new GradientColorKey(Color.red, 1.0f) },
    //                 new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.6f, 0.5f), new GradientAlphaKey(0f, 1f) }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g9;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g9;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 10 (Purple → Magenta → Red).");
    //             break;

    //         case 10:
    //             Gradient g10 = new Gradient();
    //             g10.SetKeys(
    //                 new GradientColorKey[] { new GradientColorKey(new Color(0.2f, 1f, 0.8f), 0.0f), new GradientColorKey(Color.cyan, 0.5f), new GradientColorKey(Color.blue, 1.0f) },
    //                 new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g10;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g10;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 11 (Aqua → Cyan → Blue).");
    //             break;

    //         case 11:
    //             Gradient g11 = new Gradient();
    //             g11.SetKeys(
    //                 new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.8f, 0.6f), 0.0f), new GradientColorKey(new Color(1f, 0.5f, 0.5f), 0.5f), new GradientColorKey(Color.red, 1.0f) },
    //                 new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.6f, 0.5f), new GradientAlphaKey(0f, 1f) }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g11;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g11;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 12 (Peach → Light Red → Red).");
    //             break;

    //         case 12:
    //             Gradient g12 = new Gradient();
    //             g12.SetKeys(
    //                 new GradientColorKey[] { 
    //                     new GradientColorKey(new Color(0.6f, 0.9f, 0.2f), 0.0f), 
    //                     new GradientColorKey(Color.green, 0.5f), 
    //                     new GradientColorKey(new Color(0f, 0.4f, 0f), 1.0f) 
    //                 },
    //                 new GradientAlphaKey[] { 
    //                     new GradientAlphaKey(1f, 0f), 
    //                     new GradientAlphaKey(0.5f, 0.5f), 
    //                     new GradientAlphaKey(0f, 1f) 
    //                 }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g12;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g12;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 13 (Lime → Green → Dark Green).");
    //             break;

    //         case 13:
    //             Gradient g13 = new Gradient();
    //             g13.SetKeys(
    //                 new GradientColorKey[] { 
    //                     new GradientColorKey(new Color(1f, 0.9f, 0.3f), 0.0f), 
    //                     new GradientColorKey(new Color(1f, 0.6f, 0.6f), 0.5f), 
    //                     new GradientColorKey(new Color(0.8f, 0.3f, 1f), 1.0f) 
    //                 },
    //                 new GradientAlphaKey[] { 
    //                     new GradientAlphaKey(1f, 0f), 
    //                     new GradientAlphaKey(0.6f, 0.5f), 
    //                     new GradientAlphaKey(0f, 1f) 
    //                 }
    //             );
    //             TrailRendererRight.instance.TrailRenderer.colorGradient = g13;
    //             if (TrailRendererLeft.instance != null)
    //             {
    //                 TrailRendererLeft.instance.TrailRenderer.colorGradient = g13;
    //             }
    //             Debug.Log("Activating TrailRenderer with gradient 14 (Gold → Soft Pink → Violet).");
    //             break;

    //         default:
    //             Debug.Log("Invalid trail index.");
    //             break;
    //     }
}

}
