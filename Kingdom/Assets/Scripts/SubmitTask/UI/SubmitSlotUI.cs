using UnityEngine;
using UnityEngine.UI;

public class SubmitSlotUI : MonoBehaviour
{
    [Header("组件获取")][SerializeField] private Image slotImage;
    [SerializeField] private TMPro.TMP_Text amountText;
    [SerializeField] public Image slotHightLight;
    [SerializeField] private Button button;
    [Header("格子类型")] public SlotType SlotType;
    public bool isSelect;

    public int slotIndex;
    public ItemDetails itemDetails;

    public int itemAmount;

    //private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();
    private CropUI cropUI => GetComponentInParent<CropUI>();

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
        //button.interactable = true;
    }

    public void UpdateEmptySlot()
    {
        if (isSelect)
        {
            isSelect = false;
        }

        slotImage.enabled = false;
        amountText.text = string.Empty;
        //button.interactable = false;
    }
}
