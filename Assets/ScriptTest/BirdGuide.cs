using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class BirdGuide : MonoBehaviour
{
    public static BirdGuide instance;

    public Image imageStart;
    public Image imageMid;
    public Image imageEnd;
    public Vector2 startPos;
    public Vector2 midPos ;
    public Vector2 endPos ;

    public float flyDuration1 = 5f;   
    public float flyDuration2 = 6f;   
    public float pauseTime    = 3f;   
    public float amplitude    = 10f;  
    public float frequency    = 2f;   

    public Image imageGuide;
    public Image pannelGuide;
    public TextMeshProUGUI textGuide;
    public TextMeshProUGUI textGuide1;
    public TextMeshProUGUI textGuide2;
    public bool isShowGuide = true;

    void Start()
    {
        startPos = imageStart.rectTransform.anchoredPosition;
        midPos = imageMid.rectTransform.anchoredPosition;
        endPos = imageEnd.rectTransform.anchoredPosition;
        isShowGuide = PlayerPrefs.GetInt("HasSeenBirdGuide", 0) == 0;
        instance = this;
        if (isShowGuide)
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
        textGuide1.DOFade(1f, 2f);
        textGuide2.DOFade(1f, 2f);
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
        isShowGuide = false;  
        PlayerPrefs.SetInt("HasSeenBirdGuide", 1);
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
        textGuide1.DOFade(0f, 0.5f);
        textGuide2.DOFade(0f, 0.5f);
        yield return new WaitForSeconds(0.5f); 
    }

    public IEnumerator DelaytoGuide()
    {
        StartCoroutine(FlySequence());
        yield return new WaitForSeconds(5f);
    }



}