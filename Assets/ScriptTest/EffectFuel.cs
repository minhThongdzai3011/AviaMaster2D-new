using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class EffectFuel : MonoBehaviour
{
    public static EffectFuel instance;
    public Color colorA = Color.white;
    public Color colorB = Color.gray;
    public float duration = 1f; // thời gian cho mỗi nửa chu kỳ

    private Image img;
    private Coroutine blinkRoutine;

    void Awake()
    {
        instance = this;
        img = GetComponent<Image>();
    }

    // Bắt đầu nhấp nháy
    public void StartBlink()
    {
        if (blinkRoutine == null)
        {
            blinkRoutine = StartCoroutine(Blink());
            Debug.Log("Blinking Started");  
        }
    }

    // Dừng nhấp nháy
    public void StopBlink()
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            blinkRoutine = null;
            // Reset về màu gốc nếu muốn
            img.color = colorA;
            Debug.Log("Blinking Stopped");
        }
    }

    private IEnumerator Blink()
    {
        while (true)
        {
            float elapsed = 0f;

            // White -> Gray
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                img.color = Color.Lerp(colorA, colorB, elapsed / duration);
                yield return null;
            }

            elapsed = 0f;

            // Gray -> White
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                img.color = Color.Lerp(colorB, colorA, elapsed / duration);
                yield return null;
            }
        }
    }
}