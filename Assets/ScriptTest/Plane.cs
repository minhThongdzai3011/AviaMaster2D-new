using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Plane : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StartCoroutine(UpMass());
        }
        if (collision.gameObject.tag == "Coin")
        {
            Destroy(collision.gameObject);
            GManager.instance.money += 1;
        }

    }

    IEnumerator UpMass()
    {
        if (GManager.instance == null) { Debug.Log("GManager instance is null!"); yield break; }
        Rigidbody2D rb = GManager.instance.airplaneRigidbody2D;
        if (rb == null) { Debug.Log("AirplaneRigidbody2D chưa được khởi tạo!"); yield break; }

        float step = 1f;
        float limit = 100.0f;
        float frictionForce = 0.95f; // Hệ số ma sát (0.9-0.99)
        float groundDrag = 5f; // Drag khi chạm đất

        WaitForSeconds wait = new WaitForSeconds(0.1f);

        // Lưu drag ban đầu để khôi phục sau
        float originalDrag = rb.drag;

        // Áp dụng drag cao ngay khi chạm đất
        rb.drag = groundDrag;


        while (rb.mass < limit)
        {
            // Tăng mass
            rb.mass = Mathf.Round((rb.mass + step) * 10f) / 10f;
            if (rb.mass > limit) rb.mass = limit;

            // Áp dụng ma sát cho velocity ngang
            Vector2 velocity = rb.velocity;

            // Ma sát chỉ ảnh hưởng đến chuyển động ngang (x)
            velocity.x *= frictionForce;

            // Nếu tốc độ ngang quá nhỏ thì dừng hẳn
            if (Mathf.Abs(velocity.x) < 0.5f)
            {
                velocity.x = 0f;
            }

            // Giữ máy bay trên mặt đất (không rơi xuống nữa)
            if (velocity.y < 0) velocity.y = 0f;

            // Cập nhật velocity
            rb.velocity = velocity;


            // Nếu máy bay đã gần như dừng hẳn
            if (velocity.magnitude < 0.1f)
            {
                rb.velocity = Vector2.zero;
                break;
            }

            yield return wait;
        }

        // Khôi phục drag ban đầu (tùy chọn)
        // rb.drag = originalDrag;

    }


}
