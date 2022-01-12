using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler {
  [SerializeField] private Canvas canvas;
  private RectTransform rectTransform;
  private CanvasGroup canvasGroup;

  public void OnDrag(PointerEventData eventData) {
    rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
  }

  public void OnBeginDrag(PointerEventData eventData) {
    canvasGroup.alpha = 0.8f;
    canvasGroup.blocksRaycasts = false;
  }

  public void OnEndDrag(PointerEventData eventData) {
    canvasGroup.alpha = 1.0f;
    canvasGroup.blocksRaycasts = true;
  }

  public void OnPointerDown(PointerEventData eventData) {}

  public void OnDrop(PointerEventData eventData) {}

  private void Awake() {
    rectTransform = GetComponent<RectTransform>();
    canvasGroup = GetComponent<CanvasGroup>();
  }
}
