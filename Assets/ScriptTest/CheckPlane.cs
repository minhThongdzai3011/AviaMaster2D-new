using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class CheckPlane : MonoBehaviour
{
    [Header("Buttons (RectTransforms)")]
    public RectTransform[] buttonRects = new RectTransform[3]; // kéo 3 nút, thứ tự từ trên xuống

    [Header("Entrance")]
    public float entranceDelayBetween = 0.08f;
    public float entranceDuration = 0.35f;
    public Ease entranceEase = Ease.OutCubic;
    public Vector2 entranceFromOffset = new Vector2(-60f, 0f);

    [Header("Hover")]
    public float hoverScale = 1.03f;
    public float hoverDur = 0.16f;
    public Ease hoverEase = Ease.OutSine;

    [Header("Click Pulse (scale only)")]
    public float baseScale = 1f;
    public float slowPeak = 1.08f;
    public float fastPeak = 1.14f;
    public float slowUp = 0.28f;
    public float slowDown = 0.36f;
    public float fastUp = 0.10f;
    public float fastDown = 0.16f;
    public Ease upEase = Ease.OutBack;
    public Ease downEase = Ease.InQuad;

    [Header("Idle (when no interaction)")]
    public float idleInterval = 5f; // sau khi không có tương tác, chạy idle
    public float idlePulsePeak = 1.04f; // nhẹ hơn click
    public float idleUp = 0.18f;
    public float idleDown = 0.22f;
    public Ease idleEase = Ease.OutSine;
    public bool idleLoop = true; // nếu true, tiếp tục check và chạy lại sau idleInterval

    // internals
    Tween[] hoverTweens;
    Tween currentClickTween;
    Tween[] idleTweens;
    float[] lastInteractionTime;

    void Reset()
    {
        if (buttonRects == null || buttonRects.Length == 0)
        {
            var rects = GetComponentsInChildren<RectTransform>();
            if (rects.Length > 1)
            {
                int count = Mathf.Min(3, rects.Length - 1);
                buttonRects = new RectTransform[count];
                for (int i = 0; i < count; i++) buttonRects[i] = rects[i + 1];
            }
        }
    }

    void Awake()
    {
        if (buttonRects == null) return;
        int n = buttonRects.Length;
        hoverTweens = new Tween[n];
        idleTweens = new Tween[n];
        lastInteractionTime = new float[n];

        for (int i = 0; i < n; i++)
        {
            var rt = buttonRects[i];
            if (rt)
            {
                rt.localScale = Vector3.one * baseScale;
                AddEventTriggers(i);
                lastInteractionTime[i] = Time.unscaledTime; // bắt đầu đếm từ lúc awake
            }
        }
    }

    void OnEnable()
    {
        // reset interaction times when panel re-enabled
        if (lastInteractionTime != null)
        {
            for (int i = 0; i < lastInteractionTime.Length; i++) lastInteractionTime[i] = Time.unscaledTime;
        }
    }

    void Update()
    {
        // dùng unscaledTime để idle vẫn chạy khi Time.timeScale = 0? nếu muốn bỏ, dùng Time.time
        if (buttonRects == null) return;
        float now = Time.unscaledTime;

        for (int i = 0; i < buttonRects.Length; i++)
        {
            if (buttonRects[i] == null) continue;
            // nếu đang có hover tween playing (user hover), cập nhật lastInteractionTime
            if (hoverTweens != null && hoverTweens[i] != null && hoverTweens[i].IsActive())
            {
                lastInteractionTime[i] = now;
                continue;
            }

            // nếu đã quá idleInterval và chưa có idleTween active, play idle pulse
            if (now - lastInteractionTime[i] >= idleInterval)
            {
                // nếu idleTween đang chơi thì bỏ qua
                if (idleTweens[i] == null || !idleTweens[i].IsActive())
                {
                    idleTweens[i] = PlayIdlePulseFor(buttonRects[i], i);
                    lastInteractionTime[i] = now; // reset timer
                }
            }
        }
    }

    void OnDisable()
    {
        KillAllTweens();
    }

    void KillAllTweens()
    {
        if (hoverTweens != null)
        {
            for (int i = 0; i < hoverTweens.Length; i++)
                if (hoverTweens[i] != null && hoverTweens[i].IsActive()) hoverTweens[i].Kill();
        }

        if (idleTweens != null)
        {
            for (int i = 0; i < idleTweens.Length; i++)
                if (idleTweens[i] != null && idleTweens[i].IsActive()) idleTweens[i].Kill();
        }

        if (currentClickTween != null && currentClickTween.IsActive()) currentClickTween.Kill();
        DOTween.Kill(this);
    }

    // Entrance: gọi khi UI bật
    public void PlayEntrance()
    {
        if (buttonRects == null) return;
        Sequence s = DOTween.Sequence();

        for (int i = 0; i < buttonRects.Length; i++)
        {
            var rt = buttonRects[i];
            if (rt == null) continue;

            Vector2 original = rt.anchoredPosition;
            rt.anchoredPosition = original + entranceFromOffset;
            rt.localScale = Vector3.one * 0.96f;

            s.Insert(i * entranceDelayBetween, rt.DOAnchorPos(original, entranceDuration).SetEase(entranceEase));
            s.Insert(i * entranceDelayBetween, rt.DOScale(Vector3.one, entranceDuration).SetEase(entranceEase));
        }

        s.Play();
    }

    // Called by Button.onClick (bind PlayUpgradeOnThroughButton)
    public void PlayUpgradeOnThroughButton()
    {
        var go = EventSystem.current.currentSelectedGameObject;
        if (go == null) return;

        for (int i = 0; i < buttonRects.Length; i++)
        {
            if (buttonRects[i] != null && buttonRects[i].gameObject == go)
            {
                // cập nhật lastInteractionTime để ngừng ngay idle cho nút này
                lastInteractionTime[i] = Time.unscaledTime;
                PlayUpgradePulseFor(buttonRects[i]);
                break;
            }
        }
    }

    // Public direct call: PlayUpgradeOn(index)
    public void PlayUpgradeOn(int index)
    {
        if (index < 0 || index >= buttonRects.Length) return;
        lastInteractionTime[index] = Time.unscaledTime;
        PlayUpgradePulseFor(buttonRects[index]);
    }

    // EventTriggers for hover
    void AddEventTriggers(int index)
    {
        var rt = buttonRects[index];
        if (rt == null) return;

        var et = rt.GetComponent<EventTrigger>();
        if (et == null) et = rt.gameObject.AddComponent<EventTrigger>();

        var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener((data) => { OnHoverEnter(index); lastInteractionTime[index] = Time.unscaledTime; });
        et.triggers.Add(enter);

        var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener((data) => { OnHoverExit(index); lastInteractionTime[index] = Time.unscaledTime; });
        et.triggers.Add(exit);

        var down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        down.callback.AddListener((data) => { lastInteractionTime[index] = Time.unscaledTime; });
        et.triggers.Add(down);

        var up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        up.callback.AddListener((data) => { lastInteractionTime[index] = Time.unscaledTime; });
        et.triggers.Add(up);
    }

    void OnHoverEnter(int index)
    {
        if (index < 0 || index >= buttonRects.Length) return;
        if (hoverTweens[index] != null && hoverTweens[index].IsActive()) hoverTweens[index].Kill();

        var rt = buttonRects[index];
        hoverTweens[index] = rt.DOScale(baseScale * hoverScale, hoverDur).SetEase(hoverEase).SetUpdate(true);
    }

    void OnHoverExit(int index)
    {
        if (index < 0 || index >= buttonRects.Length) return;
        if (hoverTweens[index] != null && hoverTweens[index].IsActive()) hoverTweens[index].Kill();

        var rt = buttonRects[index];
        hoverTweens[index] = rt.DOScale(Vector3.one * baseScale, hoverDur).SetEase(Ease.InOutSine).SetUpdate(true);
    }

    // Core click pulse (scale + tiny rotation shake), no Image usage
    void PlayUpgradePulseFor(RectTransform rt)
    {
        if (rt == null) return;

        if (currentClickTween != null && currentClickTween.IsActive()) currentClickTween.Kill();

        rt.localScale = Vector3.one * baseScale;
        rt.localRotation = Quaternion.identity;

        Sequence seq = DOTween.Sequence();

        seq.Append(rt.DOScale(baseScale * slowPeak, slowUp).SetEase(upEase));
        seq.Append(rt.DOScale(baseScale, slowDown).SetEase(downEase));

        seq.Append(rt.DOScale(baseScale * fastPeak, fastUp).SetEase(Ease.OutCubic));
        seq.Join(rt.DORotate(new Vector3(0f, 0f, 4f), fastUp).SetEase(Ease.OutCubic).SetLoops(2, LoopType.Yoyo));
        seq.Append(rt.DOScale(baseScale, fastDown).SetEase(downEase));
        seq.Append(rt.DORotate(Vector3.zero, 0.08f).SetEase(Ease.OutQuad));

        seq.Append(rt.DOScale(baseScale * 1.02f, 0.07f).SetEase(Ease.OutQuad));
        seq.Append(rt.DOScale(baseScale, 0.07f).SetEase(Ease.InQuad));

        currentClickTween = seq;
        seq.Play();
    }

    // Idle pulse for a single button (returns the Tween so we can track/kill)
    Tween PlayIdlePulseFor(RectTransform rt, int index)
    {
        if (rt == null) return null;

        // ensure not conflicting with click tween
        if (currentClickTween != null && currentClickTween.IsActive()) return null;

        // simple gentle pulse
        Sequence seq = DOTween.Sequence();
        seq.Append(rt.DOScale(baseScale * idlePulsePeak, idleUp).SetEase(idleEase));
        seq.Append(rt.DOScale(baseScale, idleDown).SetEase(idleEase));

        // if idleLoop true, when finished we leave lastInteractionTime so Update will schedule next after idleInterval
        seq.OnComplete(() =>
        {
            if (idleLoop)
            {
                lastInteractionTime[index] = Time.unscaledTime; // restart countdown
            }
        });

        seq.Play();
        return seq;
    }
}