using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class BirdMaxPower : MonoBehaviour
{
    public static BirdMaxPower instance;
    public Vector2 startPos = new Vector2(-1000, 000);
    public Vector2 midPos   = new Vector2(600, 300);
    public Vector2 endPos   = new Vector2(1000, 500);

    public float flyDuration1 = 5f;   // thời gian bay từ start -> mid
    public float flyDuration2 = 6f;   // thời gian bay từ mid -> end
    public float pauseTime    = 3f;   // thời gian dừng lại
    public float amplitude    = 10f;  // biên độ dao động lên xuống
    public float frequency    = 2f;   // tần số dao động

    public Image imageGuide;
    public Image pannelGuide;
    public TextMeshProUGUI textGuide;
    public Image imageGuide1;
    public Image imageGuide2;    
    public bool isMaxPower = false;

    void Start()
    {
        instance = this;
        if (isMaxPower)
        {
            transform.position = startPos;
            
        }
    }

    IEnumerator FlySequence()
    {
        // Bay từ start -> mid
        yield return StartCoroutine(FlyToPoint(startPos, midPos, flyDuration1));

        // Dừng lại 3s, trong lúc đó dao động lên xuống
        float elapsed = 0f;
        imageGuide.DOFade(1f, 2f);
        textGuide.DOFade(1f, 2f);
        Vector2 basePos = midPos;
        while (elapsed < pauseTime)
        {
            elapsed += Time.deltaTime;
            float offsetY = Mathf.Sin(elapsed * Mathf.PI * frequency) * amplitude;
            transform.position = new Vector2(basePos.x, basePos.y + offsetY);
            yield return null;
        }
        yield return StartCoroutine(FadeOutText());

        // Bay tiếp từ mid -> end
        Debug.Log("Bird is flying away!");
        yield return StartCoroutine(FlyToPoint(midPos, endPos, flyDuration2));
        pannelGuide.gameObject.SetActive(false);
        isMaxPower = false;  
        gameObject.SetActive(false);
        PlayerPrefs.Save(); 
        
        
    }

    IEnumerator FlyToPoint(Vector2 from, Vector2 to, float duration, float amplitude = 20f, float frequency = 2f)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Vị trí tuyến tính từ A -> B
            Vector2 linearPos = Vector2.Lerp(from, to, t);

            // Dao động lên xuống bằng sin
            float offsetY = Mathf.Sin(elapsed * frequency) * amplitude;

            // Cộng thêm dao động vào trục Y
            transform.position = new Vector2(linearPos.x, linearPos.y + offsetY);

            yield return null;
        }
    }
    IEnumerator FadeOutText()
    {
        imageGuide.DOFade(0f, 0.5f);
        textGuide.DOFade(0f, 0.5f);
        yield return new WaitForSeconds(0.5f); 
    }

    public IEnumerator DelaytoMaxPower()
    {
        StartCoroutine(FlySequence());
        yield return new WaitForSeconds(5f);
    }



}