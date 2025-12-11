using UnityEngine;

public class AnimCatDiamond : MonoBehaviour
{
    public float speedY = 1f;       
    public float distanceY = 10f;   
    public float speedX = 3f;       

    private Vector3 startPos1;

    void Start()
    {
        startPos1 = transform.localPosition;
    }

    void Update()
    {
        float offsetY = Mathf.PingPong(Time.time * speedY, distanceY);

        float offsetX = Time.time * speedX;

        transform.localPosition = new Vector3(startPos1.x + offsetX, startPos1.y + offsetY, startPos1.z);
    }
}