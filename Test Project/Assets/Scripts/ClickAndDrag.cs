using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAndDrag : MonoBehaviour {
  private bool dragging = false;
  private float distance;
  private float originalZ;
  private new Renderer renderer;

  void Start() {
      renderer = GetComponent<Renderer>();
  }

  void OnMouseEnter() {
    // TODO Highlight
  }

  void OnMouseExit() {
    // TODO Remove Highlight
  }

  void OnMouseDown() {
    distance = Vector3.Distance(transform.position, Camera.main.transform.position);
    dragging = true;
    originalZ = transform.position.z;
    transform.position = new Vector3(transform.position.x, transform.position.y, originalZ-1);
  }

  void OnMouseUp() {
    dragging = false;
    transform.position = new Vector3(transform.position.x, transform.position.y, originalZ);
  }

  void Update() {
    if (dragging) {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      Vector3 rayPoint = ray.GetPoint(distance);
      transform.position = new Vector3(rayPoint.x, rayPoint.y, transform.position.z);
    }
  }
}
