using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CropSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("组件获取")][SerializeField] private Image slotImage;
    [SerializeField] private TMP_Text amountText;
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
        Debug.Log("点击了种植");
        if (itemAmount == 0) return;
        // isSelect = !isSelect;
        // inventoryUI.UpdateSlotHightlight(slotIndex);
        EventHandler.CallExecuteActionAfterAnimation(cropUI.cropPoint,itemDetails);
    }

    // public void OnBeginDrag(PointerEventData eventData)
    // {
    //     if (itemAmount != 0)
    //     {
    //         inventoryUI.dragImage.enabled = true;
    //         inventoryUI.dragImage.sprite = slotImage.sprite;
    //         inventoryUI.dragImage.SetNativeSize();

    //         isSelect = true;
    //         inventoryUI.UpdateSlotHightlight(slotIndex);
    //     }
    // }

    // public void OnDrag(PointerEventData eventData)
    // {
    //     inventoryUI.dragImage.transform.position = Input.mousePosition;
    // }

    // public void OnEndDrag(PointerEventData eventData)
    // {
    //     inventoryUI.dragImage.enabled = false;

    //     Debug.Log(eventData.pointerCurrentRaycast.gameObject);

    //     if (eventData.pointerCurrentRaycast.gameObject != null)
    //     {
    //         var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
    //         if (targetSlot != null)
    //         {
    //             int targetIndex = targetSlot.slotIndex;

    //             if (SlotType == SlotType.Bag && targetSlot.SlotType == SlotType.Bag)//同背包物品位置交换
    //             {
    //                 InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
    //             }
    //             else if (SlotType == SlotType.Bag && targetSlot.SlotType == SlotType.Shop)//卖出
    //             {
    //                 EventHandler.CallShowTradeUI(itemDetails, true);
    //             }
    //             else if (SlotType == SlotType.Shop && targetSlot.SlotType == SlotType.Bag)//买入
    //             {
    //                 EventHandler.CallShowTradeUI(itemDetails, false);
    //             }
    //         }
    //     }
    //     else
    //     {
    //         //放地上了
    //         if (itemDetails.canDropped)
    //         {
    //             var pos = new Vector3(0, 0, 0);//物品放下位置，后期可以改成人物四周随机位置
    //             EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
    //         }
    //     }

    //     inventoryUI.UpdateSlotHightlight(-1);
    // }
}
