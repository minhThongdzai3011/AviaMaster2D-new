using System.Collections;
using UnityEngine;

public class RotaryFront : MonoBehaviour
{
    public static RotaryFront instance { get; private set; }

    [Header("Rotation")]
    [Tooltip("Tốc độ quay (độ/giây)")]
    private float speedDegreesPerSec = 900f;

    [Tooltip("Quay theo local hay world")]
    public bool useLocal = true;

    [Header("Options")]
    [Tooltip("Nếu true sẽ tự StartRotation khi component bật")]
    public bool playOnEnable = false;

    [Tooltip("Nếu true reset rotation về initial khi dừng")]
    public bool resetOnStop = false;

    Coroutine rotRoutine;
    Quaternion initialLocalRotation;
    bool isRunning;

    void Awake()
    {
        // Singleton nhẹ, nếu cần thay đổi behavior hãy điều chỉnh
        if (instance != null && instance != this)
        {
            return;
        }
        instance = this;

        initialLocalRotation = transform.localRotation;
    }

    void OnEnable()
    {
        if (playOnEnable) StartRotation();
    }

    void OnDisable()
    {
        StopRotation();
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    // Bắt đầu quay nếu chưa chạy
    public void StartRotation()
    {
        Debug.Log("RotaryFront StartRotation called" + rotRoutine);
        if (rotRoutine == null)
        {
            rotRoutine = StartCoroutine(RotateRoutine());
            isRunning = true;
            Debug.Log("RotaryFront StartRotation started coroutine: " + rotRoutine);
        }
    }

    // Dừng ngay lập tức
    public void StopRotation()
    {
        if (rotRoutine != null)
        {
            StopCoroutine(rotRoutine);
            rotRoutine = null;
        }

        isRunning = false;

        if (resetOnStop)
        {
            transform.localRotation = initialLocalRotation;
        }
    }

    // Tùy chọn: dừng mượt bằng cách giảm tốc trong duration giây
    public void StopWithDeceleration(float duration)
    {
        if (!isRunning) return;
        // Nếu đang chạy coroutine quay thì khởi coroutine giảm tốc
        StartCoroutine(DecelerateAndStop(duration));
    }

    private IEnumerator DecelerateAndStop(float duration)
    {
        // Lưu tốc độ ban đầu
        float startSpeed = speedDegreesPerSec;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // easing mượt: SmoothStep từ 1 -> 0
            float eased = Mathf.SmoothStep(1f, 0f, t);
            float currentSpeed = startSpeed * eased;

            // áp dụng một bước quay với currentSpeed
            float delta = currentSpeed * Time.deltaTime;
            if (useLocal) transform.Rotate(delta, 0f, 0f, Space.Self);
            else transform.Rotate(delta, 0f, 0f, Space.World);

            yield return null;
        }

        // đảm bảo dừng hoàn toàn
        StopRotation();
    }

    private IEnumerator RotateRoutine()
    {
        while (true)
        {
            float delta = speedDegreesPerSec * Time.deltaTime;
            if (useLocal) transform.Rotate(delta, 0f, 0f, Space.Self);
            else transform.Rotate(delta, 0f, 0f, Space.World);
            yield return null;
        }
    }

    // Helper kiểm tra trạng thái
    public bool IsRunning()
    {
        return isRunning;
    }
}