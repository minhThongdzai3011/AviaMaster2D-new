using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererRight : MonoBehaviour
{
    public static TrailRendererRight instance;
    public bool isBoosterActive = false;
    private TrailRenderer trailRenderer;
    private Gradient originalGradient;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        trailRenderer = GetComponent<TrailRenderer>();

        originalGradient = trailRenderer.colorGradient;

        Debug.Log("TrailRendererRight instance assigned." + (instance != null ? "Success" : "Failure"));
        Debug.Log("TrailRenderer component found: " + (trailRenderer != null ? "Yes" : "No"));
        // trailRenderer.enabled = false; // Khởi đầu tắt

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayTrail()
    {
        Debug.Log("PlayTrail called. isBoosterActive: " + isBoosterActive + ", isMaxPower: " + PositionX.instance.isMaxPower);
        if(isBoosterActive && !PositionX.instance.isMaxPower)
        {
            float time = 0.24f;
            Gradient gradientPerfect = new Gradient();
            gradientPerfect.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.yellow, 0.5f), new GradientColorKey(Color.white, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            trailRenderer.colorGradient = gradientPerfect;
            trailRenderer.time = time;
            trailRenderer.enabled = true;

            Debug.Log("TrailRenderer enabled: " + trailRenderer.enabled);
        }
        else if (isBoosterActive && PositionX.instance.isMaxPower)
        {
            float time = 0.24f;
            Debug.Log("Activating TrailRenderer with normal gradient.");
            Gradient gradient = new Gradient();
            // gradient.SetKeys(
            //     new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.green, 0.5f), new GradientColorKey(Color.cyan, 1.0f) },
            //     new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
            // );
            // trailRenderer.colorGradient = gradient;
            trailRenderer.time = time;
            trailRenderer.enabled = true;
            Debug.Log("TrailRenderer enabled: " + trailRenderer.enabled);
        }
        else
        {
            float time = 0.12f;
            trailRenderer.time = time;
            trailRenderer.enabled = true;

        }
    }

    public void TrailEffect()
    {
        if (!gameObject.activeInHierarchy || !enabled)
            return;

        // Đảm bảo TrailRenderer bật để thấy fade
        if (!trailRenderer.enabled)
            trailRenderer.enabled = true;

        StartCoroutine(FadeOutTrail(trailRenderer, 2.0f));
    }



    IEnumerator FadeOutTrail(TrailRenderer trail, float duration)
    {
        Debug.Log($"{name} bắt đầu fade, time start = {trail.time}");

        float startTime = trail.time;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            trail.time = Mathf.Lerp(startTime, 0f, elapsed / duration);


            yield return null;
        }

        trail.enabled = false;
        trail.time = startTime;

        Debug.Log($"{name} fade DONE");
    } public void ChangeColor()
    {
        if (PositionX.instance.isMaxPower)
        {
            // Gradient gradientPerfect = new Gradient();
            // gradientPerfect.SetKeys(
            //     new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.green, 0.5f), new GradientColorKey(Color.cyan, 1.0f) },
            //     new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
            // );
            // trailRenderer.colorGradient = gradientPerfect;



            switch (Shop.instance.isCheckedPlaneIndex)
            {
                case 0:
                    Gradient g0 = new Gradient();
                    g0.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.green, 0.5f), new GradientColorKey(Color.cyan, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
                    );
                    trailRenderer.colorGradient = g0;
                    Debug.Log("Activating TrailRenderer with gradient 1 (Yellow → Green → Cyan).");
                    break;

                case 1:
                    Gradient g1 = new Gradient();
                    g1.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.magenta, 0.5f), new GradientColorKey(Color.blue, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.6f, 0.5f), new GradientAlphaKey(0f, 1f) }
                    );
                    trailRenderer.colorGradient = g1;
                    Debug.Log("Activating TrailRenderer with gradient 2 (Red → Magenta → Blue).");
                    break;

                case 2:
                    Gradient g2 = new Gradient();
                    g2.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(Color.cyan, 0.0f), new GradientColorKey(Color.blue, 0.5f), new GradientColorKey(Color.white, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.7f, 0.5f), new GradientAlphaKey(0f, 1f) }
                    );
                    trailRenderer.colorGradient = g2;
                    Debug.Log("Activating TrailRenderer with gradient 3 (Cyan → Blue → White).");
                    break;

                case 3:
                    Gradient g3 = new Gradient();
                    g3.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.5f, 0f), 0.0f), new GradientColorKey(Color.red, 0.5f), new GradientColorKey(Color.black, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
                    );
                    trailRenderer.colorGradient = g3;
                    Debug.Log("Activating TrailRenderer with gradient 4 (Orange → Red → Black).");
                    break;

                case 4:
                    Gradient g4 = new Gradient();
                    g4.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(new Color(0.5f, 1f, 0.5f), 0.5f), new GradientColorKey(Color.white, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.6f, 0.5f), new GradientAlphaKey(0f, 1f) }
                    );
                    trailRenderer.colorGradient = g4;
                    Debug.Log("Activating TrailRenderer with gradient 5 (Green → Light Green → White).");
                    break;

                case 5:
                    Gradient g5 = new Gradient();
                    g5.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(Color.magenta, 0.0f), new GradientColorKey(new Color(1f, 0.5f, 0.8f), 0.5f), new GradientColorKey(Color.yellow, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
                    );
                    trailRenderer.colorGradient = g5;
                    Debug.Log("Activating TrailRenderer with gradient 6 (Magenta → Pink → Yellow).");
                    break;

                case 6:
                    Gradient g6 = new Gradient();
                    g6.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(new Color(0.3f, 0.7f, 1f), 0.5f), new GradientColorKey(Color.cyan, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.6f, 0.5f), new GradientAlphaKey(0f, 1f) }
                    );
                    trailRenderer.colorGradient = g6;
                    Debug.Log("Activating TrailRenderer with gradient 7 (Blue → Sky Blue → Cyan).");
                    break;

                case 7:
                    Gradient g7 = new Gradient();
                    g7.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(new Color(0.8f, 0.2f, 0.2f), 0.0f), new GradientColorKey(new Color(1f, 0.6f, 0.2f), 0.5f), new GradientColorKey(Color.yellow, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
                    );
                    trailRenderer.colorGradient = g7;
                    Debug.Log("Activating TrailRenderer with gradient 8 (Dark Red → Orange → Yellow).");
                    break;

                case 8:
                    Gradient g8 = new Gradient();
                    g8.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.gray, 0.5f), new GradientColorKey(Color.black, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.4f, 0.5f), new GradientAlphaKey(0f, 1f) }
                    );
                    trailRenderer.colorGradient = g8;
                    Debug.Log("Activating TrailRenderer with gradient 9 (White → Gray → Black).");
                    break;

                case 9:
                    Gradient g9 = new Gradient();
                    g9.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(new Color(0.5f, 0f, 1f), 0.0f), new GradientColorKey(Color.magenta, 0.5f), new GradientColorKey(Color.red, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.6f, 0.5f), new GradientAlphaKey(0f, 1f) }
                    );
                    trailRenderer.colorGradient = g9;
                    Debug.Log("Activating TrailRenderer with gradient 10 (Purple → Magenta → Red).");
                    break;

                case 10:
                    Gradient g10 = new Gradient();
                    g10.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(new Color(0.2f, 1f, 0.8f), 0.0f), new GradientColorKey(Color.cyan, 0.5f), new GradientColorKey(Color.blue, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0f, 1f) }
                    );
                    trailRenderer.colorGradient = g10;
                    Debug.Log("Activating TrailRenderer with gradient 11 (Aqua → Cyan → Blue).");
                    break;

                case 11:
                    Gradient g11 = new Gradient();
                    g11.SetKeys(
                        new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.8f, 0.6f), 0.0f), new GradientColorKey(new Color(1f, 0.5f, 0.5f), 0.5f), new GradientColorKey(Color.red, 1.0f) },
                        new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0.6f, 0.5f), new GradientAlphaKey(0f, 1f) }
                    );
                    trailRenderer.colorGradient = g11;
                    Debug.Log("Activating TrailRenderer with gradient 12 (Peach → Light Red → Red).");
                    break;

                case 12:
                    Gradient g12 = new Gradient();
                    g12.SetKeys(
                        new GradientColorKey[] { 
                            new GradientColorKey(new Color(0.6f, 0.9f, 0.2f), 0.0f), 
                            new GradientColorKey(Color.green, 0.5f), 
                            new GradientColorKey(new Color(0f, 0.4f, 0f), 1.0f) 
                        },
                        new GradientAlphaKey[] { 
                            new GradientAlphaKey(1f, 0f), 
                            new GradientAlphaKey(0.5f, 0.5f), 
                            new GradientAlphaKey(0f, 1f) 
                        }
                    );
                    trailRenderer.colorGradient = g12;
                    Debug.Log("Activating TrailRenderer with gradient 13 (Lime → Green → Dark Green).");
                    break;

                case 13:
                    Gradient g13 = new Gradient();
                    g13.SetKeys(
                        new GradientColorKey[] { 
                            new GradientColorKey(new Color(1f, 0.9f, 0.3f), 0.0f), 
                            new GradientColorKey(new Color(1f, 0.6f, 0.6f), 0.5f), 
                            new GradientColorKey(new Color(0.8f, 0.3f, 1f), 1.0f) 
                        },
                        new GradientAlphaKey[] { 
                            new GradientAlphaKey(1f, 0f), 
                            new GradientAlphaKey(0.6f, 0.5f), 
                            new GradientAlphaKey(0f, 1f) 
                        }
                    );
                    trailRenderer.colorGradient = g13;
                    Debug.Log("Activating TrailRenderer with gradient 14 (Gold → Soft Pink → Violet).");
                    break;

                default:
                    Debug.Log("Invalid trail index.");
                    break;
            }
        }
        else
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.yellow, 0.5f), new GradientColorKey(Color.white, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            trailRenderer.colorGradient = gradient;

        }
    }

    public void StopTrail()
    {
        trailRenderer.enabled = false;
    }
}