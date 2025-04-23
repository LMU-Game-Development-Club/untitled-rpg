using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentController : MonoBehaviour
{
    public enum EquipmentType {
        Head,
        Hand
    }

    // Names must be unique
    [Serializable]
    public struct EquipmentUIItem {
        public EquipmentUIItem(string a, string b, Sprite c, EquipmentType d) {
            name = a;
            description = b;
            icon = c;
            type = d;
        }

        public string name;
        public string description;
        public Sprite icon;
        public EquipmentType type;
    }

    private static EquipmentUIItem EmptyItem = new EquipmentUIItem("Empty", null, null, EquipmentType.Head);

    [Header("Refs")]
    public GameObject InventoryItemPrefab;
    public Image HeadSlotRenderer;
    public UI_DialogueOnClick HeadSlotOnClick;
    public Image HandSlotRenderer;
    public UI_DialogueOnClick HandSlotOnClick;
    public Transform InventoryContentTransform;

    [Header("Inventory")]
    public List<EquipmentUIItem> Inventory;

    private List<(Transform, EquipmentUIItem)> EquipmentContainers = new List<(Transform, EquipmentUIItem)>();

    public EquipmentUIItem HeadSlot = EmptyItem;
    public EquipmentUIItem HandSlot = EmptyItem;

    private Canvas canvas;
    private float canvasScaleFactor;

    void Start() {
        canvas = GetComponent<Canvas>();
        canvasScaleFactor = canvas.scaleFactor;
        UpdateInventoryContainer();
        UpdateSlots();

        HeadSlotOnClick.onClick.AddListener(HeadSlotClick);
        HandSlotOnClick.onClick.AddListener(HandSlotClick);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousePosition = new Vector2(Input.mousePosition.x / canvasScaleFactor, Input.mousePosition.y / canvasScaleFactor);

            foreach ((Transform, EquipmentUIItem) pair in EquipmentContainers) {
                Transform container = pair.Item1;
                Rect rect =  container.GetComponent<RectTransform>().rect;
                Vector2 TopLeftP = new Vector2(container.position.x, container.position.y);
                Vector2 minP = TopLeftP + new Vector2(0, -rect.height);
                Vector2 maxP = TopLeftP + new Vector2(rect.width, 0);
                if (mousePosition.x > minP.x && mousePosition.y > minP.y && mousePosition.x < maxP.x && mousePosition.y < maxP.y) {
                    EquipmentUIItem item = pair.Item2;
                    switch (item.type) {
                        case EquipmentType.Head:
                            if (HeadSlot.name != "Empty") {
                                Inventory.Add(HeadSlot);
                            }
                            HeadSlot = item;
                            break;
                        case EquipmentType.Hand:
                            if (HandSlot.name != "Empty") {
                                Inventory.Add(HandSlot);
                            }
                            HandSlot = item;
                            break;
                    }
                    Inventory.Remove(item);
                    UpdateInventoryContainer();
                    UpdateSlots();
                }
            }
        }
    }

    void HeadSlotClick() {
        if (HeadSlot.name == "Empty"){
            return;
        }
        Inventory.Add(HeadSlot);
        HeadSlot = EmptyItem;
        UpdateInventoryContainer();
        UpdateSlots();
    }

    void HandSlotClick() {
        if (HandSlot.name == "Empty"){
            return;
        }
        Inventory.Add(HandSlot);
        HandSlot = EmptyItem;
        UpdateInventoryContainer();
        UpdateSlots();
    }

    void UpdateInventoryContainer() {
        foreach (Transform container in InventoryContentTransform) {
            Destroy(container.gameObject);
        }

        EquipmentContainers = new List<(Transform, EquipmentUIItem)>();
        foreach(EquipmentUIItem equipmentItem in Inventory) {
            GameObject newContainer = Instantiate(InventoryItemPrefab);
            newContainer.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = equipmentItem.name;
            newContainer.transform.Find("ItemDescription").GetComponent<TextMeshProUGUI>().text = equipmentItem.description;
            newContainer.transform.Find("Icon").GetComponent<Image>().sprite = equipmentItem.icon;
            newContainer.transform.parent = InventoryContentTransform;
            EquipmentContainers.Add((newContainer.transform, equipmentItem));
        }
    }

    void UpdateSlots() {
        if (HeadSlot.name == "Empty") {
            HeadSlotRenderer.color = Color.black;
            HeadSlotRenderer.sprite = null;
        } else {
            HeadSlotRenderer.color = Color.white;
            HeadSlotRenderer.sprite = HeadSlot.icon;
        }

        if (HandSlot.name == "Empty") {
            HandSlotRenderer.color = Color.black;
            HandSlotRenderer.sprite = null;
        } else {
            HandSlotRenderer.color = Color.white;
            HandSlotRenderer.sprite = HandSlot.icon;
        }
    }
}
