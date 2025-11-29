using UnityEngine;
using System.Collections.Generic;

public class DestroyWheels : MonoBehaviour
{

    public static DestroyWheels instance;

    public Rigidbody2D rb;          // Rigidbody2D gắn vào cánh quạt
    public float force = 5f;        // lực văng ra
    public float torque = 2f;       // lực xoay thêm để chân thực

    private bool exploded = false;

    void Awake() {
        instance = this;
    }

    void Start()
    {
        // Nếu chưa gán Rigidbody thì tự lấy
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Ban đầu khóa rigidbody để cánh quạt đứng yên
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.simulated = false; // tắt physics ban đầu
        }
    }

    // Hàm gọi khi máy bay nổ
    public void Explode()
    {
        if (rb != null && !exploded)
        {
            Debug.Log("Exploding propeller");

            rb.isKinematic = false;
            rb.simulated = true; // bật physics

            // Văng ra theo hướng ngẫu nhiên (chứ không chỉ hướng up)
            Vector2 randomDir = (Vector2.up + Random.insideUnitCircle).normalized;
            rb.AddForce(randomDir * force, ForceMode2D.Impulse);

            // Thêm torque để xoay loạn
            float randomTorque = Random.Range(-torque, torque);
            rb.AddTorque(randomTorque, ForceMode2D.Impulse);
            gameObject.GetComponent<Renderer>().material = Plane.instance.blackMaterial;
            Destroy(gameObject);
            exploded = true;
        }
    }
}