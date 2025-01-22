using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTips : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text DescriptionText;
    [SerializeField] private Text valueText;
    [SerializeField] private GameObject button;

    public void SetupToolTip(ItemDetails itemDetails, SlotType slotType)
    {
        nameText.text = itemDetails.ItemName;
        typeText.text = GetItemType(itemDetails.itemType);
        DescriptionText.text = itemDetails.itemDescription;
        if (itemDetails.itemType == ItemType.Commodity || itemDetails.itemType == ItemType.Seed || itemDetails.itemType == ItemType.Furniture)
        {

            button.SetActive(true);
            var price = itemDetails.itemPrice;
            if (slotType == SlotType.Bag)
            {
                price = (int)(price * itemDetails.sellPercentage);
            }
            valueText.text = price.ToString();
        }
        else
        {
            button.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetChild(1) as RectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    private string GetItemType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Seed => "种子",
            ItemType.Commodity => "商品",
            ItemType.Furniture => "家具",
            ItemType.HoeTool => "工具",
            ItemType.ChopTool => "工具",
            ItemType.BreakTool => "工具",
            ItemType.ReapTool => "工具",
            ItemType.WaterTool => "工具",
            ItemType.CollectTool => "工具",
            ItemType.ReapableScenery => "工具",
            _=>"无"
        };
    }
}
