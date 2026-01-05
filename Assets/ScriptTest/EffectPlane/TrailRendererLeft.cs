using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererLeft : MonoBehaviour
{
    public static TrailRendererLeft  instance;
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
        // Sync rotation Z với máy bay (chỉ xoay trục Z)
        if (GManager.instance != null && GManager.instance.airplaneRigidbody2D != null)
        {
            float airplaneRotZ = GManager.instance.airplaneRigidbody2D.transform.eulerAngles.z;
            // Normalize góc về -180 đến 180 độ (giống GManager)
            if (airplaneRotZ > 180f) airplaneRotZ -= 360f;
            transform.rotation = Quaternion.Euler(0f, 0f, airplaneRotZ);
        }
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
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.green, 0.5f), new GradientColorKey(Color.cyan, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            trailRenderer.colorGradient = gradient;
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
                // ✨ Máy bay 1 – Vàng óng ánh
                case 0:
                    Gradient g0 = new Gradient();
                    g0.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(new Color(1f, 0.85f, 0.2f), 0f),
                            new GradientColorKey(Color.white, 0.5f),
                            new GradientColorKey(new Color(1f, 0.75f, 0.1f), 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(1f, 0f),
                            new GradientAlphaKey(0.7f, 0.4f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g0;
                    break;

                // 🌈 Máy bay 2 – Cầu vồng (xếp dọc, mảng rõ)
                case 1:
                    Gradient g1 = new Gradient();
                    g1.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(Color.red, 0f),
                            new GradientColorKey(Color.yellow, 0.25f),
                            new GradientColorKey(Color.green, 0.5f),
                            new GradientColorKey(Color.cyan, 0.75f),
                            new GradientColorKey(Color.magenta, 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(1f, 0f),
                            new GradientAlphaKey(0.6f, 0.5f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g1;
                    break;

                // 🔥 Máy bay 3 – Xanh than + đỏ động cơ (hầm hố)
                case 2:
                    Gradient g2 = new Gradient();
                    g2.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(new Color(0.1f, 0.1f, 0.2f), 0f),
                            new GradientColorKey(Color.red, 0.6f),
                            new GradientColorKey(new Color(1f, 0.3f, 0.1f), 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(1f, 0f),
                            new GradientAlphaKey(0.8f, 0.5f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g2;
                    break;

                // 🌈 Máy bay 4 – Cầu vồng (đảo thứ tự màu)
                case 3:
                    Gradient g3 = new Gradient();
                    g3.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(Color.magenta, 0f),
                            new GradientColorKey(Color.cyan, 0.33f),
                            new GradientColorKey(Color.yellow, 0.66f),
                            new GradientColorKey(Color.red, 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(0.9f, 0f),
                            new GradientAlphaKey(0.5f, 0.5f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g3;
                    break;

                // 💛 Máy bay 5 – Vàng óng đậm
                case 4:
                    Gradient g4 = new Gradient();
                    g4.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(new Color(1f, 0.7f, 0f), 0f),
                            new GradientColorKey(new Color(1f, 0.85f, 0.4f), 0.5f),
                            new GradientColorKey(new Color(0.8f, 0.5f, 0f), 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(1f, 0f),
                            new GradientAlphaKey(0.6f, 0.5f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g4;
                    break;

                // 💛 Máy bay 6 – Vàng đậm (fade nhanh)
                case 5:
                    Gradient g5 = new Gradient();
                    g5.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(new Color(1f, 0.75f, 0.1f), 0f),
                            new GradientColorKey(Color.white, 0.3f),
                            new GradientColorKey(new Color(1f, 0.6f, 0f), 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(0.9f, 0f),
                            new GradientAlphaKey(0.4f, 0.5f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g5;
                    break;

                // 💖 Máy bay 7 – Hồng + trắng kim tuyến
                case 6:
                    Gradient g6 = new Gradient();
                    g6.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(new Color(1f, 0.5f, 0.8f), 0f),
                            new GradientColorKey(Color.white, 0.5f),
                            new GradientColorKey(new Color(1f, 0.7f, 0.9f), 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(1f, 0f),
                            new GradientAlphaKey(0.8f, 0.4f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g6;
                    break;

                // 🔵 Máy bay 8 – Xanh than + xanh lam (động cơ mạnh)
                case 7:
                    Gradient g7 = new Gradient();
                    g7.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(new Color(0.05f, 0.1f, 0.2f), 0f),
                            new GradientColorKey(new Color(0.2f, 0.6f, 1f), 0.5f),
                            new GradientColorKey(Color.cyan, 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(1f, 0f),
                            new GradientAlphaKey(0.7f, 0.5f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g7;
                    break;

                // 💛 Máy bay 9 – Vàng đậm (đuôi sáng mạnh)
                case 8:
                    Gradient g8 = new Gradient();
                    g8.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(new Color(0.9f, 0.6f, 0f), 0f),
                            new GradientColorKey(new Color(1f, 1f, 0.6f), 0.7f),
                            new GradientColorKey(Color.white, 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(1f, 0f),
                            new GradientAlphaKey(0.9f, 0.6f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g8;
                    break;

                // 💛 Máy bay 10 – Vàng nhạt hơn
                case 9:
                    Gradient g9 = new Gradient();
                    g9.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(new Color(1f, 0.85f, 0.4f), 0f),
                            new GradientColorKey(Color.white, 0.5f),
                            new GradientColorKey(new Color(1f, 0.7f, 0.2f), 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(0.8f, 0f),
                            new GradientAlphaKey(0.4f, 0.5f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g9;
                    break;

                // 🌈 Máy bay 11 – Cầu vồng pastel
                case 10:
                    Gradient g10 = new Gradient();
                    g10.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(new Color(0.8f, 0.6f, 1f), 0f),
                            new GradientColorKey(new Color(0.6f, 1f, 0.9f), 0.5f),
                            new GradientColorKey(new Color(1f, 0.9f, 0.6f), 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(0.9f, 0f),
                            new GradientAlphaKey(0.5f, 0.5f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g10;
                    break;

                // 🌌 Máy bay 12 – Xanh đen vũ trụ
                case 11:
                    Gradient g11 = new Gradient();
                    g11.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(new Color(0f, 0.05f, 0.1f), 0f),
                            new GradientColorKey(new Color(0.1f, 0.3f, 0.6f), 0.5f),
                            new GradientColorKey(new Color(0.4f, 0.8f, 1f), 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(1f, 0f),
                            new GradientAlphaKey(0.6f, 0.6f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g11;
                    break;

                // 🌈 Máy bay 13 – Cầu vồng neon
                case 12:
                    Gradient g12 = new Gradient();
                    g12.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(Color.cyan, 0f),
                            new GradientColorKey(Color.magenta, 0.5f),
                            new GradientColorKey(Color.yellow, 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(1f, 0f),
                            new GradientAlphaKey(0.7f, 0.5f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g12;
                    break;

                // 💛 Máy bay 14 – Vàng kim cao cấp
                case 13:
                    Gradient g13 = new Gradient();
                    g13.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(new Color(1f, 0.8f, 0.3f), 0f),
                            new GradientColorKey(new Color(1f, 0.9f, 0.6f), 0.5f),
                            new GradientColorKey(new Color(0.9f, 0.7f, 0.2f), 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(1f, 0f),
                            new GradientAlphaKey(0.6f, 0.5f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g13;
                    break;
                case 14:
                    Gradient g14 = new Gradient();
                    g14.SetKeys(
                        new GradientColorKey[] {
                            new GradientColorKey(new Color(0f, 0.1f, 0.4f), 0f),
                            new GradientColorKey(new Color(0f, 0.5f, 1f), 0.5f),
                            new GradientColorKey(new Color(0f, 0.2f, 0.6f), 1f)
                        },
                        new GradientAlphaKey[] {
                            new GradientAlphaKey(1f, 0f),
                            new GradientAlphaKey(0.7f, 0.5f),
                            new GradientAlphaKey(0f, 1f)
                        }
                    );
                    trailRenderer.colorGradient = g14;
                    break;

                default:
                    Debug.Log("Invalid trail index");
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