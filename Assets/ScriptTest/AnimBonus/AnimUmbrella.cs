using UnityEngine;

public class AnimUmbrella : MonoBehaviour
{
    public float moveRange = 0.05f;     // 5px ~ 0.05f (nếu 1 unit ~100px)
    public float moveSpeed = 1.5f;      // tốc độ dao động
    public float rotationAngle = 20f;   // góc xoay tối đa ±20 độ
    public float rotationSpeed = 2f;    // tốc độ xoay

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float offsetX = Mathf.Sin(Time.time * moveSpeed) * moveRange;
        float offsetY = Mathf.Cos(Time.time * moveSpeed * 0.7f) * moveRange; 

        Vector3 newPos = startPos + new Vector3(offsetX, offsetY, 0f);
        transform.localPosition = newPos;

        float targetZ = Mathf.Lerp(-rotationAngle, rotationAngle, (offsetX + moveRange) / (2f * moveRange));

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZ);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}