using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SlotUI))]
public class ShowToolTIp : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SlotUI slotUI;
    private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

    void Awake()
    {
        slotUI = GetComponent<SlotUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotUI.itemAmount != 0)
        {
            inventoryUI.itemToolTips.gameObject.SetActive(true);
            inventoryUI.itemToolTips.SetupToolTip(slotUI.itemDetails,slotUI.SlotType);
            inventoryUI.itemToolTips.GetComponent<RectTransform>().pivot =new Vector2(0.5f,0);
            inventoryUI.itemToolTips.transform.position = transform.position+Vector3.up*60;
        }
        else
        {
            inventoryUI.itemToolTips.gameObject.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryUI.itemToolTips.gameObject.SetActive(false);
    }
}
