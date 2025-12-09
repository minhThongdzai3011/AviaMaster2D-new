using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;

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
    public bool isMaxPower = false;

    public void checkPlay()
    {
        transform.DOPause();
        newPositionX = transform.position;
        float deltaX = newPositionX.x - positionX.x;
        Debug.Log($"Delta X: {deltaX:F2}");
        
        int temp = Mathf.RoundToInt((deltaX / 3.2f) * 100);
        temp = Mathf.Clamp(temp, 0, 100);
        Debug.Log($"Temp Value: {temp}%");
        
        // ✅ Set isMaxPower NGAY LẬP TỨC
        if (temp >= 80)
        {
            isMaxPower = true;
            GManager.instance.isBonus = true;    
            Debug.Log(" Kích hoạt Max Power!");
            GManager.instance.newMapText.text = "Max Power Activated!";
            if(ExplosionScale.instance != null) ExplosionScale.instance.Explosion();
            if (EffectAirplane.instance != null) EffectAirplane.instance.MakePlaneGold();
            if (DestroyWheels.instance != null) DestroyWheels.instance.Golden();
            foreach (var propeller in EffectRotaryFront.instances)
            {
                propeller.gameObject.GetComponent<Renderer>().material = Plane.instance.GoldMaterial;
            }
            if (Plane.instance.explosionEffect != null) Plane.instance.explosionEffect.Play();

            StartCoroutine(DelayTwoSeconds(1f));
        }
        else
        {
            isMaxPower = false;
            Debug.Log(" Không kích hoạt Max Power.");
            GManager.instance.isBonus = false;
            RandomTextMaxPower();
        }
    }
    public void RandomTextMaxPower(){
        string[] texts = {
        "Over 95% = MAX POWER",
        "Hit 95%+ for MAX POWER",
        "Cross 95% → MAX POWER",
        "95% unlocks MAX POWER",
        "Reach 95%+ to power up"
        };
        int randomIndex = Random.Range(0, texts.Length);
        GManager.instance.newMapText.text = texts[randomIndex];
    }

        IEnumerator DelayTwoSeconds(float delay)
        {
            yield return new WaitForSeconds(delay);
            
        }
}
