using UnityEngine;

public class AnimHotAirBalloon : MonoBehaviour
{
    public float speedY = 1f; 
    public float distanceY = 10f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float offsetY = Mathf.PingPong(Time.time * speedY, distanceY);

        transform.localPosition = new Vector3(startPos.x, startPos.y + offsetY, startPos.z);
    }
}