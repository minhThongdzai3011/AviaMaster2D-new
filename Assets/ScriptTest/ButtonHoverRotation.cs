using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHoverRotation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f; // Độ/giây
    public bool rotateOnHover = true;
    
    private bool isHovering = false;
    private Tweener rotationTween;

    void Update()
    {
        if (isHovering && rotateOnHover)
        {
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Hover vào {gameObject.name}");
        isHovering = true;
        
        // Optional: Thêm scale effect khi hover
        transform.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"Hover ra khỏi {gameObject.name}");
        isHovering = false;
        
        // Reset rotation về 0
        transform.DORotate(Vector3.zero, 0.3f).SetEase(Ease.OutBack);
        
        // Reset scale
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    void OnDisable()
    {
        // Cleanup khi disable
        isHovering = false;
        rotationTween?.Kill();
    }
}