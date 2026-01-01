using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 10f;
    public float rotateSmooth = 15f;

    private float targetAngle;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.angularVelocity = 0f;
        }
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Được gọi từ PlaneShoot
    public void SetTargetAngle(float angle)
    {
        targetAngle = angle;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        // Smooth rotation cho đẹp (KHÔNG ảnh hưởng hướng bay)
        float currentZ = transform.eulerAngles.z;
        float z = Mathf.LerpAngle(currentZ, targetAngle, Time.deltaTime * rotateSmooth);
        transform.rotation = Quaternion.Euler(0, 0, z);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("bird"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.crashBonusSoundClip);
            Plane.instance.RandomPrizeBird();
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Bonus4"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.obstacleCollisionSoundClip);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Bonus2"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.obstacleCollisionSoundClip);
            EffectExplosionBonus ef = collision.GetComponent<EffectExplosionBonus>();
            ef.ExplosionEffect();
            ef.ExplosionEffect1();
            collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(gameObject);
        }
    }
}
