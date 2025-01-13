using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CropUI : MonoBehaviour
{
    [SerializeField] GameObject cropPanel;
    public GameObject cropSlotPrefab;

    [SerializeField] private List<CropSlotUI> baseBagSlots;

    private InventoryBag_SO curentBagData;
    public Vector3 cropPoint;
    private void OnEnable()
    {
        EventHandler.ShowCropPanelEvent += OnShowCropPanelEvent;
        EventHandler.DisShowCropPanelEvent += OnCloseCropPanelEvent;
        EventHandler.UpdateCropUIEvent += OnUpdateCropUIEvent;
    }
    private void OnDisable()
    {
        EventHandler.ShowCropPanelEvent -= OnShowCropPanelEvent;
        EventHandler.DisShowCropPanelEvent -= OnCloseCropPanelEvent;
        EventHandler.UpdateCropUIEvent -= OnUpdateCropUIEvent;
    }

    private void OnUpdateCropUIEvent()
    {
        if (cropPanel.activeSelf)
        {
            ClearSlotUI();
            OnShowCropPanelEvent(curentBagData, cropPoint);
        }
    }
    private void ClearSlotUI()//清除种植UI
    {
        foreach (var item in baseBagSlots)
        {
            Destroy(item.gameObject);
        }
        baseBagSlots.Clear();
    }

    //将背包里的种子相关显示出来
    private void OnShowCropPanelEvent(InventoryBag_SO bagData, Vector3 worldPos)
    {
        curentBagData = bagData;
        cropPoint = worldPos;//种植地点
        cropPanel.SetActive(true);
        if(baseBagSlots!=null)ClearSlotUI();
        baseBagSlots = new();
        var detailList = new List<ItemDetails>();
        var amount = new List<int>();
        int index = 0;
        for (int i = 0; i < bagData.itemList.Count; i++)
        {
            var detail = InventoryManager.Instance.GetItemDetails(bagData.itemList[i].itemID);
            if (detail != null && detail.itemType == ItemType.Seed)
            {
                detailList.Add(detail);
                amount.Add(bagData.itemList[i].itemAmount);
                var slot = Instantiate(cropSlotPrefab, cropPanel.transform).GetComponent<CropSlotUI>();
                slot.slotIndex = index;
                baseBagSlots.Add(slot);
                index++;
            }

        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(cropPanel.transform as RectTransform);
        for (int i = 0; i < detailList.Count; i++)
        {
            if (amount[i] > 0)
            {
                //var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                baseBagSlots[i].UpdateSlot(detailList[i], amount[i]);
            }
            else
            {
                baseBagSlots[i].UpdateEmptySlot();
            }
        }
    }

    private void OnCloseCropPanelEvent()
    {
        if (!cropPanel.activeSelf)return;
        cropPanel.SetActive(false);
        ClearSlotUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnCloseCropPanelEvent();
        }
    }
}
