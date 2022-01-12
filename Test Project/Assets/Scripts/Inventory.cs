using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {
  private List<Item> items;

  public Inventory() {
    items = new List<Item>(); 

    addItem(new Item { itemType = Item.ItemType.Apple, amount = 1});
    addItem(new Item { itemType = Item.ItemType.Carrot, amount = 1});
    addItem(new Item { itemType = Item.ItemType.Meat, amount = 1});
  }

  public void addItem(Item item) {
    items.Add(item);
  }

  public List<Item> getItems() {
    return items;
  }
}
