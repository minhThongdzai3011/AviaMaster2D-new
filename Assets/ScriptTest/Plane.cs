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
            // Xử lý khi va chạm với mặt đất
            Debug.Log("Máy bay đã va chạm với mặt đất!");
            // Có thể thêm logic kết thúc trò chơi hoặc giảm máu ở đây
            StartCoroutine(UpMass());
        }

    }
    IEnumerator UpMass()
    {
        if (GManager.instance != null && GManager.instance.airplaneRigidbody2D != null)
        {
            GManager.instance.airplaneRigidbody2D.mass = 1.2f;
            yield return new WaitForSeconds(0.2f);
            GManager.instance.airplaneRigidbody2D.mass = 1.4f;
            yield return new WaitForSeconds(0.2f);
            GManager.instance.airplaneRigidbody2D.mass = 1.6f;
            yield return new WaitForSeconds(0.2f);
            GManager.instance.airplaneRigidbody2D.mass = 1.8f;
            yield return new WaitForSeconds(0.2f);
            GManager.instance.airplaneRigidbody2D.mass = 2f;
            yield return new WaitForSeconds(0.2f);
            GManager.instance.airplaneRigidbody2D.mass = 2.2f;
            yield return new WaitForSeconds(0.2f);
            GManager.instance.airplaneRigidbody2D.mass = 2.4f;
            yield return new WaitForSeconds(0.2f);
            GManager.instance.airplaneRigidbody2D.mass = 2.6f;
            yield return new WaitForSeconds(0.2f);
            GManager.instance.airplaneRigidbody2D.mass = 2.8f;
            yield return new WaitForSeconds(0.2f);
            GManager.instance.airplaneRigidbody2D.mass = 3f;
        }
        else
        {
            if (GManager.instance == null)
                Debug.Log("GManager instance is null!");
            else
                Debug.Log("AirplaneRigidbody2D chưa được khởi tạo!");
        }


    }
}
