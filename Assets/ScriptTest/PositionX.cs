using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PositionX : MonoBehaviour
{
    public static PositionX instance;
    public Vector2 positionX;
    public Vector2 newPositionX;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    
    {
        positionX = transform.position;
        
        // Di chuyển position.x thêm 100 đơn vị trong 1 giây
        transform.DOMoveX(transform.position.x + 3.2f, 1f)
            .SetLoops(-1, LoopType.Yoyo) 
            .SetEase(Ease.Linear);      
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void checkPlay()
    {
        transform.DOPause();
        newPositionX = transform.position;
        float deltaX = newPositionX.x - positionX.x;
        Debug.Log("Delta X: " + deltaX);
    }
}
