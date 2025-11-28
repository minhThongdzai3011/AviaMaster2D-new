using UnityEngine;

public class AnimUmbrella : MonoBehaviour
{
    public float amplitude = 0.01f;   
    public float frequency = 2f;     
    public float speedX = 1f;        

    private Vector3 startPos;

    void Start()
    {
        // Lưu vị trí ban đầu của Bird
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Tính toán dao động Y
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // Tăng dần vị trí X theo thời gian
        float newX = startPos.x + Time.time * speedX;

        // Gán vị trí mới
        transform.localPosition = new Vector3(newX, newY, startPos.z);
    }
}