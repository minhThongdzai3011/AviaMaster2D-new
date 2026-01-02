using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaryFront : MonoBehaviour
{
    // Thay đổi từ singleton sang List để hỗ trợ nhiều cánh quạt
    public static List<RotaryFront> instances = new List<RotaryFront>();

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
    
    // Thêm biến để theo dõi rotation hiện tại
    private float currentRotationX = 0f;

    void Awake()
    {
        // Thêm instance này vào list
        if (!instances.Contains(this))
        {
            instances.Add(this);
        }

        initialLocalRotation = transform.localRotation;
        // Lưu rotation X ban đầu
        currentRotationX = transform.localRotation.eulerAngles.x;
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
        instances.Remove(this);
    }

    void Update()
    {
        if (useLocal)
        {
            transform.localRotation = Quaternion.Euler(currentRotationX, 0f, -90f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(currentRotationX, 0f, 90f);
        }
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
            currentRotationX = initialLocalRotation.eulerAngles.x;
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

            // Cập nhật rotation X và giữ nguyên Y, Z
            float delta = currentSpeed * Time.deltaTime;
            currentRotationX += delta;
            
            if (useLocal)
            {
                Vector3 currentEuler = transform.localRotation.eulerAngles;
                transform.localRotation = Quaternion.Euler(currentRotationX, currentEuler.y, currentEuler.z);
            }
            else
            {
                Vector3 currentEuler = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(currentRotationX, currentEuler.y, currentEuler.z);
            }

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
            currentRotationX += delta;
            
            if (useLocal)
            {
                // Chỉ thay đổi rotation X, giữ nguyên Y và Z
                Vector3 currentEuler = transform.localRotation.eulerAngles;
                transform.localRotation = Quaternion.Euler(currentRotationX, currentEuler.y, currentEuler.z);
            }
            else
            {
                Vector3 currentEuler = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(currentRotationX, currentEuler.y, currentEuler.z);
            }
            
            yield return null;
        }
    }

    // Helper kiểm tra trạng thái
    public bool IsRunning()
    {
        return isRunning;
    }
}