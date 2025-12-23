using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BolideEffect : MonoBehaviour
{
    
    public Transform meteor;
    public Vector3 startPos;
    public Vector3 endPos;
    public float duration = 2f;
    public float xOffsetRange = 1f; 
    public int delayStart = 0;
    

    void Start()
    {
        meteor.position = startPos;
    }

    void MoveMeteor()
    {
        if (meteor == null || !meteor.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Meteor object is null or inactive!");
            return;
        }
        meteor.DOKill();

        meteor.DORotate(new Vector3(0, 0, 360), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetUpdate(true);


        meteor.DOMove(endPos, duration)
            .SetEase(Ease.InQuad)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                    TrailRenderer trail = meteor.GetComponent<TrailRenderer>();
                    if (trail != null)
                    {

                        trail.enabled = false;
                        trail.Clear();
                    }
                    float randomX = Random.Range(-xOffsetRange, xOffsetRange);
                    Vector3 newStart = new Vector3(startPos.x + randomX, startPos.y, startPos.z);
                    Vector3 newEnd   = new Vector3(endPos.x + randomX, endPos.y, endPos.z);
                    meteor.position = newStart;
                    if (trail != null)
                    {
                        trail.Clear();
                        trail.enabled = true;
                    }
                    MoveMeteor();

        });
    }

    IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        MoveMeteor();
    }

    void Update()
    {
        if (GManager.instance.isPlayBolide == true)
        {
            StartCoroutine(DelayedStart(delayStart));
            GManager.instance.isPlayBolide = false;
        }
    }
}