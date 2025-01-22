using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
   [Header("提示UI")] public ItemToolTips itemToolTips;
   [Header("拖拽UI")] public Image dragImage;
   [Header("背包UI")]
   [SerializeField] private GameObject playBag;

   [SerializeField] private SlotUI[] PlayerSlots;
   private bool bagOpened;
   [Header("通用背包")]
   public GameObject baseBag;
   public GameObject shopSlotPrefab;

   [SerializeField] private List<SlotUI> baseBagSlots;

   [Header("交易UI")]
   public TradeUI tradeUI;

   public TMP_Text moneyText;
   private void OnEnable()
   {
      EventHandler.updateInventoryUI += OnUpdateInventoryUI;
      EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
      EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
      EventHandler.ShowTradeUI += OnShowTradeUI;

   }

   public void OnDisable()
   {
      EventHandler.updateInventoryUI -= OnUpdateInventoryUI;
      EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
      EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
      EventHandler.ShowTradeUI -= OnShowTradeUI;
   }

   private void OnShowTradeUI(ItemDetails details, bool isSell)
   {
      tradeUI.gameObject.SetActive(true);
      tradeUI.SetupTradeUI(details, isSell);
   }

   /// <summary>
   /// 关闭通用背包
   /// </summary>
   /// <param name="type"></param>
   private void OnBaseBagCloseEvent(SlotType type)
   {
      baseBag.SetActive(false);
      itemToolTips.gameObject.SetActive(false);
      UpdateSlotHightlight(-1);

      foreach (var item in baseBagSlots)
      {
         Destroy(item.gameObject);
      }
      baseBagSlots.Clear();
      if (type == SlotType.Shop)
      {
         bagOpened = true;
         playBag.SetActive(bagOpened);
      }
   }

   /// <summary>
   /// 打开通用背包，目前按照商店处理
   /// </summary>
   /// <param name="type"></param>
   /// <param name="bagData"></param>
   private void OnBaseBagOpenEvent(SlotType type, InventoryBag_SO bagData)
   {
      GameObject prefab = type switch
      {
         SlotType.Shop => shopSlotPrefab,
         _ => null
      };
      if (baseBagSlots != null)
      {
         foreach (var item in baseBagSlots)
         {
            Destroy(item.gameObject);
         }
         baseBagSlots.Clear();
      }
      //生成UI，根据数据生成对应数量的格子
      baseBag.SetActive(true);
      baseBagSlots = new();

      for (int i = 0; i < bagData.itemList.Count; i++)
      {
         var slot = Instantiate(prefab, baseBag.transform).GetComponent<SlotUI>();
         slot.slotIndex = i;
         baseBagSlots.Add(slot);
      }
      if (type == SlotType.Shop)
      {
         bagOpened = true;
         playBag.SetActive(bagOpened);
      }
      LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.transform as RectTransform);
      OnUpdateInventoryUI(InventoryLocation.Box, bagData);

   }

   private void Start()
   {
      //给每个柜子增加序号
      for (int i = 0; i < PlayerSlots.Length; i++)
      {
         PlayerSlots[i].slotIndex = i;
      }

      bagOpened = playBag.activeInHierarchy;
      //moneyText.text = InventoryManager.Instance.playerMoney.ToString();
   }

   private void OnUpdateInventoryUI(InventoryLocation location, InventoryBag_SO bag_SO)
   {
      var list = bag_SO.itemList;
      switch (location)
      {
         case InventoryLocation.Player:
            for (int i = 0; i < PlayerSlots.Length; i++)
            {
               if (list[i].itemAmount > 0)//TODO:检测list[i]是否存在
               {
                  var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                  PlayerSlots[i].UpdateSlot(item, list[i].itemAmount);
               }
               else
               {
                  PlayerSlots[i].UpdateEmptySlot();
               }
            }
            moneyText.text = bag_SO.money.ToString();//这更新的是玩家背包金钱
            EventHandler.CallUpdateCropUIEvent();
            break;
         case InventoryLocation.Box:
            for (int i = 0; i < baseBagSlots.Count; i++)
            {
               if (list[i].itemAmount > 0)
               {
                  var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                  baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
               }
               else
               {
                  baseBagSlots[i].UpdateEmptySlot();
               }
            }
            break;

      }
   }

   /// <summary>
   /// 打开关闭背包，button调用
   /// </summary>
   public void OpenBagUI()
   {
      bagOpened = !bagOpened;
      playBag.SetActive(bagOpened);
   }

   public void UpdateSlotHightlight(int index)
   {
      foreach (var item in PlayerSlots)
      {
         if (item.slotIndex == index && item.isSelect)
         {
            item.slotHightLight.gameObject.SetActive(true);
         }
         else
         {
            item.isSelect = false;
            item.slotHightLight.gameObject.SetActive(false);
         }
      }
   }

}
