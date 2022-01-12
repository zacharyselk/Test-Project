using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Item {
  public enum ItemType {
    Carrot,
    Apple,
    Meat,
  }

  public ItemType itemType;
  public int amount = 0;

  public Sprite getSprite() {
    Debug.Log((int)itemType);
    switch (itemType) {
      case ItemType.Apple: return ItemAssets.instance.appleSprite;
      case ItemType.Carrot: return ItemAssets.instance.carrotSprite;
      case ItemType.Meat: return ItemAssets.instance.meatSprite;
      default: Assert.IsTrue(false); return ItemAssets.instance.appleSprite;
    }
  }

  public string getInfo() {
    switch (itemType) {
      case ItemType.Apple: return "Apple";
      case ItemType.Carrot: return "Carrot";
      case ItemType.Meat: return "Meat";
      default: Assert.IsTrue(false); return "UNKNOWN";
    }
  }
}
