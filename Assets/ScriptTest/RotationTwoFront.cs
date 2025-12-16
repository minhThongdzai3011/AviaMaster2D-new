using System.Collections;
using UnityEngine;

public class RotationTwoFront : MonoBehaviour
{
    [Header("Rotation")]
    [Tooltip("Tốc độ quay (độ/giây)")]
    [SerializeField] private float speedDegreesPerSec = 900f;

    [Tooltip("Quay theo local hay world")]
    public bool useLocal = true;

    [Tooltip("Quay theo trục Z (chuẩn 2D). Bỏ tick nếu muốn quay X")]
    public bool rotateZAxis = true;

    [Header("Options")]
    [Tooltip("Tự quay khi object được bật")]
    public bool playOnEnable = false;

    [Tooltip("Reset rotation về ban đầu khi dừng")]
    public bool resetOnStop = false;

    private Coroutine rotRoutine;
    private Quaternion initialLocalRotation;
    private bool isRunning;

    // ===================== UNITY =====================

    void Awake()
    {
        initialLocalRotation = transform.localRotation;
    }

    void OnEnable()
    {
        if (playOnEnable)
            StartRotation();
    }

    void OnDisable()
    {
        StopRotation();
    }

    // ===================== PUBLIC API =====================

    public void StartRotation()
    {
        if (rotRoutine != null) return;

        rotRoutine = StartCoroutine(RotateRoutine());
        isRunning = true;
    }

    public void StopRotation()
    {
        if (rotRoutine != null)
        {
            StopCoroutine(rotRoutine);
            rotRoutine = null;
        }

        isRunning = false;

        if (resetOnStop)
            transform.localRotation = initialLocalRotation;
    }

    public void StopWithDeceleration(float duration)
    {
        if (!isRunning) return;
        StartCoroutine(DecelerateAndStop(duration));
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    // ===================== COROUTINES =====================

    private IEnumerator RotateRoutine()
    {
        while (true)
        {
            float delta = speedDegreesPerSec * Time.deltaTime;

            if (rotateZAxis)
            {
                transform.Rotate(0f, 0f, delta, useLocal ? Space.Self : Space.World);
            }
            else
            {
                transform.Rotate(delta, 0f, 0f, useLocal ? Space.Self : Space.World);
            }

            yield return null;
        }
    }

    private IEnumerator DecelerateAndStop(float duration)
    {
        float startSpeed = speedDegreesPerSec;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = Mathf.SmoothStep(1f, 0f, t);
            float currentSpeed = startSpeed * eased;

            float delta = currentSpeed * Time.deltaTime;

            if (rotateZAxis)
            {
                transform.Rotate(0f, 0f, delta, useLocal ? Space.Self : Space.World);
            }
            else
            {
                transform.Rotate(delta, 0f, 0f, useLocal ? Space.Self : Space.World);
            }

            yield return null;
        }

        StopRotation();
    }
}
