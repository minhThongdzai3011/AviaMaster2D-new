using UnityEngine;
using System.Collections.Generic;

public class EffectExplosionBonus : MonoBehaviour
{
    public static EffectExplosionBonus instance;

    public Rigidbody2D rb;          // Rigidbody2D gắn vào cánh quạt
    public float force = 5f;        // lực văng ra
    public float torque = 2f;       // lực xoay thêm để chân thực
    private bool exploded = false;

    void Awake() {
        instance = this;
    }

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.simulated = false; 
        }
    }

    public void Explode()
    {
        Debug.Log("Explode called on bonus " + rb + " : " + exploded);
        if (rb != null && !exploded)
        {
            // rb.gameObject.SetActive(true);
            Debug.Log("Exploding propeller");

            rb.isKinematic = false;
            rb.simulated = true; 
            float angle = Random.Range(0f, Mathf.PI);

            Vector2 randomDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

            rb.AddForce(randomDir * force, ForceMode2D.Impulse);

            // Thêm torque để xoay loạn
            float randomTorque = Random.Range(-torque, torque);
            rb.AddTorque(randomTorque, ForceMode2D.Impulse);
            exploded = true;
        }
    }


    public static void ExplodeAll()
    {
        Debug.Log("Exploding all bonuses");
        instance.Explode();
    }




}