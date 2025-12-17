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
            Gradient gradientPerfect = new Gradient();
            gradientPerfect.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.green, 0.5f), new GradientColorKey(Color.cyan, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.5f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            trailRenderer.colorGradient = gradientPerfect;
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