using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererRight : MonoBehaviour
{
    public static TrailRendererRight instance;
    public bool isBoosterActive = false;
    private TrailRenderer trailRenderer;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        trailRenderer = GetComponent<TrailRenderer>();
        Debug.Log("TrailRendererRight instance assigned." + (instance != null ? "Success" : "Failure"));
        Debug.Log("TrailRenderer component found: " + (trailRenderer != null ? "Yes" : "No"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayTrail()
    {
        if(isBoosterActive)
        {
            trailRenderer.enabled = true;
            Debug.Log("TrailRenderer enabled: " + trailRenderer.enabled);
        }
        else
        {
            trailRenderer.enabled = false;
            Debug.Log("TrailRenderer enabled: " + trailRenderer.enabled);
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
    }

}