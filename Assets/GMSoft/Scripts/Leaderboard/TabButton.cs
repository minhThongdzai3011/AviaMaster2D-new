using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Leaderboard
{
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public string tabName;

        [OnValueChanged("OnInteractableChange"), ShowInInspector]
        private bool interactable = true;
        [OnInspectorInit("AutoSetTargetGraphic")]
        public Image targetGraphic;

        private bool isHover;
        private bool isPressed;

        public bool isSelected = false;

        public UnityAction<TabButton> OnSelectTab = null;

        public enum Transistion
        {
            None, ColorTint, SpriteSwap
        }

        [EnumToggleButtons, OnValueChanged("OnTransistionChange")]
        public Transistion transistion = Transistion.ColorTint;

        [ShowIf("@transistion==Transistion.ColorTint")]
        public Color normalColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f);
        [ShowIf("@transistion==Transistion.ColorTint")]
        public Color highlightedColor = new Color(245 / 255f, 245 / 255f, 245 / 255f, 255 / 255f);
        [ShowIf("@transistion==Transistion.ColorTint")]
        public Color pressedColor = new Color(200 / 255f, 200 / 255f, 200 / 255f, 255);
        [ShowIf("@transistion==Transistion.ColorTint")]
        public Color selectedColor = new Color(245 / 255f, 245 / 255f, 245 / 255f, 255 / 255f);
        [ShowIf("@transistion==Transistion.ColorTint")]
        public Color disabledColor = new Color(200 / 255f, 200 / 255f, 200 / 255f, 128 / 255f);
        [ShowIf("@transistion==Transistion.ColorTint")]
        public float fadeDuration = 0.1f;

        [ShowIf("@transistion==Transistion.SpriteSwap")]
        public Sprite normalSprite;
        [ShowIf("@transistion==Transistion.SpriteSwap")]
        public Sprite highlightedSprite;
        [ShowIf("@transistion==Transistion.SpriteSwap")]
        public Sprite pressedSprite;
        [ShowIf("@transistion==Transistion.SpriteSwap")]
        public Sprite selectedSprite;
        [ShowIf("@transistion==Transistion.SpriteSwap")]
        public Sprite disabledSprite;

        private void AutoSetTargetGraphic()
        {
            if (targetGraphic == null)
            {
                targetGraphic = GetComponent<Image>();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isSelected) return;
            Select();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isSelected) return;
            if (!interactable) return;
            isPressed = true;
            PressedGraphic();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isSelected) return;
            if (!interactable) return;
            if (isPressed) return;
            isHover = true;
            HighlightedGraphic();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isSelected) return;
            if (!interactable) return;
            if (isPressed) return;
            isHover = false;
            NormalGraphic();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isSelected) return;
            if (!interactable) return;
            isPressed = false;
            if (!isHover)
            {
                NormalGraphic();
            }
            else
            {
                HighlightedGraphic();
            }
        }

        private void OnInteractableChange(bool value)
        {
            UpdateTargetGraphicEditMode();
        }

        private void OnTransistionChange(Transistion value)
        {
            UpdateTargetGraphicEditMode();
        }

        public void SetInteractable(bool value)
        {
            interactable = value;
            if (!interactable)
            {
                DisableGraphic();
            }
        }

        public bool GetIntaractable() => interactable;

        private void UpdateTargetGraphicEditMode()
        {
            if (targetGraphic == null) return;
            if (!interactable)
            {
                if (transistion == Transistion.ColorTint)
                {
                    targetGraphic.color = disabledColor;
                }
                else if (transistion == Transistion.SpriteSwap)
                {
                    if (disabledSprite != null)
                    {
                        targetGraphic.sprite = disabledSprite;
                    }
                }
                else
                {
                    targetGraphic.color = normalColor;
                    if (normalSprite != null)
                    {
                        targetGraphic.sprite = normalSprite;
                    }
                }
            }
            else
            {
                if (transistion == Transistion.ColorTint)
                {
                    targetGraphic.color = normalColor;
                }
                else if (transistion == Transistion.SpriteSwap)
                {
                    if (normalSprite != null)
                    {
                        targetGraphic.sprite = normalSprite;
                    }
                }
            }
#if UNITY_EDITOR
            EditorApplication.delayCall += () =>
            {
                SceneView.RepaintAll();
                EditorUtility.SetDirty(targetGraphic);
            };
#endif
        }

        public void Select()
        {
            if (isSelected) return;
            isSelected = true;
            isHover = false;
            OnSelectTab?.Invoke(this);
            SelectedGraphic();
        }

        public void Deselect()
        {
            isSelected = false;
            if (!isHover)
            {
                NormalGraphic();
            }
            else
            {
                HighlightedGraphic();
            }
        }

        private void NormalGraphic()
        {
            ChangeGraphic(normalColor, normalSprite);
        }

        private void HighlightedGraphic()
        {
            ChangeGraphic(highlightedColor, highlightedSprite);
        }

        private void PressedGraphic()
        {
            ChangeGraphic(pressedColor, pressedSprite);
        }

        private void SelectedGraphic()
        {
            ChangeGraphic(selectedColor, selectedSprite);
        }

        private void DisableGraphic()
        {
            ChangeGraphic(disabledColor, disabledSprite);
        }

        private void ChangeGraphic(Color color, Sprite sprite)
        {
            if (targetGraphic == null) return;
            if (transistion == Transistion.None) return;
            if (transistion == Transistion.ColorTint)
            {
                targetGraphic.DOColor(color, fadeDuration);
            }
            else if (transistion == Transistion.SpriteSwap)
            {
                if (sprite != null)
                {
                    targetGraphic.sprite = sprite;
                }
            }
        }
    }
}
