using UnityEngine;

public class AnimBalloon : MonoBehaviour
{
    public float verticalSpeed = 2f;     // tốc độ bay lên
    public float horizontalSpeed = 2f;   // tốc độ di chuyển ngang
    public float horizontalRange = 2f;   // biên độ dao động trái/phải

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Bay lên theo trục Y
        float newY = startPos.y + Time.time * verticalSpeed;

        // Dao động trái/phải mượt bằng sin
        float newX = startPos.x + Mathf.Sin(Time.time * horizontalSpeed) * horizontalRange;

        // Gán vị trí mới
        transform.localPosition = new Vector3(newX, newY, startPos.z);
    }
}