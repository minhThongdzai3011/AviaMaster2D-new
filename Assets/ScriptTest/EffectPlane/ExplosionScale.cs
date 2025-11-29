using UnityEngine;

public class ExplosionScale : MonoBehaviour
{
    public static ExplosionScale instance;
    public float duration = 1f;       // thời gian hiệu ứng (1 giây)
    public Vector3 startScale = new Vector3(0f, 0f, 0f);   // scale ban đầu
    public Vector3 endScale = new Vector3(10f, 10f, 1f);   // scale kết thúc

    private float timer = 0f;
    private bool playing = false;

    void Awake()
    {
        instance = this;
    }

    public void Explosion()
    {
        // Reset khi GameObject được bật
        transform.localScale = startScale;
        timer = 0f;
        playing = true;
    }

    void Update()
    {
        if (playing)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            // Nội suy scale từ start -> end
            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            // Kết thúc sau duration
            if (timer >= duration)
            {
                transform.localScale = endScale;
                playing = false;
                gameObject.SetActive(false);
            }
        }
    }
}