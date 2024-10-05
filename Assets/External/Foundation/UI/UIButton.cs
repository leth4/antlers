using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Foundation.UI
{
    [RequireComponent(typeof(Image))]
    public class UIButton : UIBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action OnClick;
        public event Action OnMouseEnter;
        public event Action OnMouseExit;

        [field: SerializeField] public bool IsInteractable { get; private set; }

        [SerializeField] private Image _image;

        [Header("Colors")]
        [SerializeField] private Color _defaultColor;
        [SerializeField] private Color _hoverCover;
        [SerializeField] private Color _holdColor;
        [SerializeField] private Color _disabledColor;

        public bool IsMouseOver { get; private set; }

        public void Toggle()
        {
            IsInteractable = !IsInteractable;
        }

        public void SetInteractable(bool interactable)
        {
            IsInteractable = interactable;
        }

        public void Click()
        {
            OnClick?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Click();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsMouseOver = true;
            OnMouseEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            IsMouseOver = false;
            OnMouseExit?.Invoke();
        }

        private void SetNewColor()
        {
            StopAllCoroutines();
        }

        private IEnumerator ApplyColorRoutine(Color color)
        {
            var startingColor = _image.color;
            var changeTime = 2f;
            for (float t = 0; t < changeTime; t += Time.deltaTime)
            {
                _image.color = Color.Lerp(startingColor, color, t / changeTime);
                yield return null;
            }
            _image.color = color;
        }
    }
}
