using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDisplayInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
  public string info;
  
  public void OnPointerEnter(PointerEventData eventData) {
    Tooltip.showTooltip(info);
  }

  public void OnPointerExit(PointerEventData eventData) {
    Tooltip.hideTooltip();
  }

  public void setInfo(string i) {
    info = i;
  }
}
