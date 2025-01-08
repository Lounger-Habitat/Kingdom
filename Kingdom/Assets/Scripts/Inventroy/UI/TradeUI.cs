using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeUI : MonoBehaviour
{
    public Image itemIcon;

    public TMP_Text itemName;

    public TMP_InputField tradeAmount;

    public Button submitButton;
    public Button cancelButton;

    private ItemDetails item;

    private bool isSellTrade;

    void Awake()
    {
        submitButton.onClick.AddListener(TradeItem);
        cancelButton.onClick.AddListener(CancelTrade);
        gameObject.SetActive(false);
    }

    public void SetupTradeUI(ItemDetails item,bool isSell)
    {
        this.item = item;

        itemIcon.sprite = item.itemIcon;
        itemName.text = item.ItemName;
        isSellTrade = isSell;
        tradeAmount.text = string.Empty;
    
    }

    private void CancelTrade()
    {
        gameObject.SetActive(false);
    }

    private void TradeItem()
    {
        var amount = Convert.ToInt32(tradeAmount.text);
        InventoryManager.Instance.TradeItem(item,amount,isSellTrade);
        CancelTrade();
    }
}
