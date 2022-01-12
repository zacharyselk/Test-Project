using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour {
  [SerializeField] private int rowSize = 10;
  [SerializeField] private float itemSlotSize = 40.0f;

  private Inventory inventory;
  private Transform itemSlotContainer;
  private Transform itemSlotTemplate;
  private bool inventoryOpen = false;

  public void setInventory(Inventory i) {
    inventory = i;
    refreshItems();
  }

  public void togglePosition() {
    RectTransform transform = gameObject.GetComponent<RectTransform>();
    float moveAmount = 310.0f;
    if (inventoryOpen) {
      transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, transform.anchoredPosition.y-moveAmount);
    } else {
      transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, transform.anchoredPosition.y+moveAmount);
    }

    inventoryOpen = !inventoryOpen;
  }

  private void Awake() {
    itemSlotContainer = transform.Find("Item_Slot_Container");
    itemSlotTemplate = itemSlotContainer.Find("Item_Slot_Template");
    if (itemSlotContainer == null) {
      Debug.LogError("Could not fine Item_Slot_Container!");
    }
    if (itemSlotTemplate == null) {
      Debug.LogError("Could not fine Item_Slot_Template!");
    }
  }

  private void refreshItems() {
    int index = 0;
    foreach (Item item in inventory.getItems()) {
      if (itemSlotContainer == null) {
      itemSlotContainer = transform.Find("Item_Slot_Container");
    }
    if (itemSlotTemplate == null) {
      itemSlotTemplate = itemSlotContainer.Find("Item_Slot_Template");
    }
      //Transform itemInstance = Instantiate(itemSlotTemplate, itemSlotContainer);
      //Debug.Log("0");
      RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
      //Debug.Log("1");
      itemSlotRectTransform.gameObject.SetActive(true);
      //Debug.Log("2");
      itemSlotRectTransform.anchoredPosition = new Vector2((index%rowSize)*itemSlotSize, (index/rowSize)*itemSlotSize);
      //Debug.Log("3");
      //itemInstance.GetComponent<UIDisplayInfo>().setInfo(item.getInfo());
      Image image = itemSlotRectTransform.Find("Image").GetComponent<Image>();
      image.sprite = item.getSprite();
      ++index;
    }
  }
}
