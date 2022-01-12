using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour {
  public static Tooltip instance { get; private set; }
  [SerializeField] private RectTransform canvasRectTransform;
  public TextMeshProUGUI tooltipText;
  public RectTransform tooltipBackground;
  public Vector2 paddingSize = new Vector2(8, 8);
  private RectTransform rectTransform;

  public static void showTooltip(string tooltipString) {
    instance._showTooltip(tooltipString);
  }

  public static void hideTooltip() {
    instance._hideTooltip();
  }

  private void Awake() {
    instance = this;
    rectTransform = transform.GetComponent<RectTransform>();

    hideTooltip();
  }

  private void setText(string tooltipString) {
    tooltipText.SetText(tooltipString);
    tooltipText.ForceMeshUpdate();

    Vector2 textSize = tooltipText.GetRenderedValues(false);
    
    tooltipBackground.sizeDelta = textSize + paddingSize;
  }

  private void _showTooltip(string tooltipString) {
    gameObject.SetActive(true);

    setText(tooltipString);
  }

  private void _hideTooltip() {
    gameObject.SetActive(false);
  }

  private void Update() {
    Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;
    if (anchoredPosition.x + tooltipBackground.rect.width > canvasRectTransform.rect.width) {
      // Tooltip is on the right side of the screen
      anchoredPosition.x = canvasRectTransform.rect.width - tooltipBackground.rect.width;
    }
    if (anchoredPosition.y + tooltipBackground.rect.height > canvasRectTransform.rect.height) {
      // Tooltip is on the top of the screen
      anchoredPosition.y = canvasRectTransform.rect.height - tooltipBackground.rect.height;
    }

    rectTransform.anchoredPosition = anchoredPosition;
  }
}
