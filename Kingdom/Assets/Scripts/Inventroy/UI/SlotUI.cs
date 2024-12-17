using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("组件获取")] [SerializeField] private Image slotImage;
    [SerializeField] private TMP_Text amountText;
    [SerializeField] public Image slotHightLight;
    [SerializeField] private Button button;
    [Header("格子类型")] public SlotType SlotType;
    public bool isSelect;

    public int slotIndex;
    public ItemDetails itemDetails;

    public int itemAmount;

    private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

    private void Start()
    {
        isSelect = false;
        if (itemDetails.itemID == 0)
        {
            UpdateEmptySlot();
        }
    }

    public void UpdateSlot(ItemDetails item, int amount)
    {
        slotImage.enabled = true;
        itemDetails = item;
        slotImage.sprite = item.itemIcon;
        itemAmount = amount;
        amountText.text = amount.ToString();
        button.interactable = true;
    }

    public void UpdateEmptySlot()
    {
        if (isSelect)
        {
            isSelect = false;
        }

        slotImage.enabled = false;
        amountText.text = string.Empty;
        button.interactable = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemAmount == 0) return;
        isSelect = !isSelect;
        inventoryUI.UpdateSlotHightlight(slotIndex);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemAmount != 0)
        {
            inventoryUI.dragImage.enabled = true;
            inventoryUI.dragImage.sprite = slotImage.sprite;
            inventoryUI.dragImage.SetNativeSize();

            isSelect = true;
            inventoryUI.UpdateSlotHightlight(slotIndex);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        inventoryUI.dragImage.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inventoryUI.dragImage.enabled = false;

        Debug.Log(eventData.pointerCurrentRaycast.gameObject);

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
            if (targetSlot != null)
            {
                int targetIndex = targetSlot.slotIndex;

                //同背包物品位置交换
                if (SlotType == SlotType.Bag && targetSlot.SlotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }
            }
        }
        else
        {
            //放地上了
            
        }

        inventoryUI.UpdateSlotHightlight(-1);
    }
}