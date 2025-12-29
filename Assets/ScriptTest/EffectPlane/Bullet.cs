using Best.HTTP.SecureProtocol.Org.BouncyCastle.Math.Field;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 40f;   
    public float lifeTime = 10f; 

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    // void OnCollisionEnter2D(Collision2D collision)
    // {
    //     Destroy(gameObject);
    // }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("bird"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.crashBonusSoundClip);
            Plane.instance.RandomPrizeBird();
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Bonus4"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.obstacleCollisionSoundClip);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Bonus2"))
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