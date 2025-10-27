using UnityEngine;
using Cinemachine; // Thêm thư viện Cinemachine

public class CameraManager : MonoBehaviour
{
    // Gán Virtual Camera vào trường này trong Inspector
    public CinemachineVirtualCamera virtualCamera;

    // Các thiết lập cho hiệu ứng zoom
    public float zoomSpeed = 2f;
    public float minOrthoSize = 3f;
    public float maxOrthoSize = 10f;
    public float smoothSpeed = 5f;

    private float targetOrthoSize;

    void Start()
    {
        // Kiểm tra xem đã gán Virtual Camera chưa
        if (virtualCamera == null)
        {
            Debug.LogError("Virtual Camera chưa được gán!");
            return;
        }

        // Thiết lập kích thước ban đầu
        targetOrthoSize = virtualCamera.m_Lens.OrthographicSize;
    }

    void Update()
    {
        // Lấy giá trị cuộn chuột
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0.0f)
        {
            // Thay đổi kích thước mục tiêu
            targetOrthoSize -= scroll * zoomSpeed;

            // Giới hạn kích thước camera trong một khoảng nhất định
            targetOrthoSize = Mathf.Clamp(targetOrthoSize, minOrthoSize, maxOrthoSize);
        }

        // Di chuyển kích thước hiện tại đến kích thước mục tiêu một cách mượt mà
        virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
            virtualCamera.m_Lens.OrthographicSize, 
            targetOrthoSize, 
            smoothSpeed * Time.deltaTime
        );
    }
}
