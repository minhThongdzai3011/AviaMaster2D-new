using UnityEngine;


public class TestCamera : MonoBehaviour
{
    public static TestCamera instance;
    public ParticleSystem explosionEffect; // Gán hiệu ứng nổ trong Inspector
    public GameObject plane;      // Gán máy bay trong Inspector
    public Material black;   // Gán Material của máy bay trong Inspector
    private Material originalColor;
    public bool isDestroyed = false;

    void Start()
    {
        instance = this;
        if (plane == null)
        {
            Debug.LogError("Chưa gán Plane trong Inspector!");
        }
        originalColor = plane.GetComponent<Renderer>().material;
        
    }

    // Hàm làm đen máy bay
    public void MakePlaneBlack()
    {
        Debug.Log("Making plane black");
        if (plane != null)
        {
            plane.GetComponent<Renderer>().material = black;
            isDestroyed = true;
            EffectRotaryFront.ExplodeAll();
            DestroyWheels.instance.Explode();
            ExplosionScale.instance.Explosion();
            explosionEffect.Play();
        }
    }

    // Hàm khôi phục màu gốc
    public void RestorePlaneColor()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}