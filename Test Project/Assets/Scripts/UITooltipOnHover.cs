using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITooltipOnHover : MonoBehaviour, IPointerEnterHandler {
  [SerializeField] private string tooltip;
  [SerializeField] private float waitTime = 0.5f;
  private ActionOnTimer timer;
  private bool timerSet = false;
  private Vector3 lastMousePosition;

  public void setTooltip(string tt) {
    tooltip = tt;
  }

  public void OnPointerEnter(PointerEventData eventData) {
    //Debug.Log("The cursor entered the selectable UI element.");
    if (!timerSet) {
      lastMousePosition = Input.mousePosition;
      timer.set(waitTime, () => { Tooltip.showTooltip(tooltip); });
      timerSet = true;
    }
  }

  private void Awake() {
    timer = gameObject.AddComponent(typeof(ActionOnTimer)) as ActionOnTimer;
  }

  private void Update() {
    if (timerSet && Input.mousePosition != lastMousePosition) {
      Tooltip.hideTooltip();
      lastMousePosition = Input.mousePosition;
      timer.stop();
      timerSet = false;
    }
  }
}
