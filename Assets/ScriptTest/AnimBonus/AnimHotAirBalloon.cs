using UnityEngine;

public class AnimHotAirBalloon : MonoBehaviour
{
    public float speedY = 1f;   // tốc độ bay lên (1 đơn vị/giây)
    private Vector3 startPos;

    void Start()
    {
        // Lưu vị trí ban đầu
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Tăng dần vị trí Y theo thời gian
        float newY = startPos.y + Time.time * speedY;

        // Gán vị trí mới
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}