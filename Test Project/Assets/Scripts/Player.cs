using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
  [SerializeField] UIInventory uiInventory;
  private Inventory inventory;

  private void Awake() {
    inventory = new Inventory();
    uiInventory.setInventory(inventory);
  }
}
