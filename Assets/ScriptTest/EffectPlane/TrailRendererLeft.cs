using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailRendererLeft : MonoBehaviour
{
    public static TrailRendererLeft instance;
    public bool isBoosterActive = false;
    private TrailRenderer trailRenderer;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        trailRenderer = GetComponent<TrailRenderer>();
        Debug.Log("TrailRendererLeft instance assigned." + (instance != null ? "Success" : "Failure"));
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
        StartCoroutine(FadeOutTrail(trailRenderer, 2.0f));
    }

    IEnumerator FadeOutTrail(TrailRenderer trail, float duration)
    {
        float startTime = trail.time;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            trail.time = Mathf.Lerp(startTime, 0f, elapsed / duration);
            yield return null;
        }

        trail.enabled = false; // tắt hẳn sau khi fade xong
        trailRenderer.time = startTime;
    }
}
