using UnityEngine;
using Cinemachine;

public class TestCamera : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("VirtualCamera chưa được gán trong Inspector!");
            return;
        }

        // Lấy FramingTransposer trong phần Body thay vì Composer trong Aim
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer == null)
        {
            Debug.LogError("VirtualCamera chưa có FramingTransposer. Hãy đặt Body = Framing Transposer.");
            return;
        }

        // Giá trị ban đầu
        transposer.m_ScreenX = 0.5f;
        transposer.m_ScreenY = 0.5f;

        // Sau 2 giây thì đổi sang 0.7, 0.7
        Invoke(nameof(ChangeScreenXY), 2f);
    }

    void ChangeScreenXY()
    {
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer != null)
        {
            transposer.m_ScreenX = 0.7f;
            transposer.m_ScreenY = 0.7f;
        }
    }
}