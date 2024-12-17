using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
   [Header("拖拽UI")] public Image dragImage;
   [Header("背包UI")]
   [SerializeField] private GameObject playBag;
   
   [SerializeField] private SlotUI[] PlayerSlots;

   
   private bool bagOpened;
   private void OnEnable()
   {
      EventHandler.updateInventoryUI += OnUpdateInventoryUI;
   }

   public void OnDisable()
   {
      EventHandler.updateInventoryUI -= OnUpdateInventoryUI;
   }

   private void Start()
   {
      //给每个柜子增加序号
      for (int i = 0; i < PlayerSlots.Length; i++)
      {
         PlayerSlots[i].slotIndex = i;
      }

      bagOpened = playBag.activeInHierarchy;
   }
   
   private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
   {
      switch (location)
      {
         case InventoryLocation.Player:
            for (int i = 0; i < PlayerSlots.Length; i++)
            {
               if (list[i].itemAmount>0)
               {
                  var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                  PlayerSlots[i].UpdateSlot(item,list[i].itemAmount);
               }
               else
               {
                  PlayerSlots[i].UpdateEmptySlot();
               }
            }
            break;
         case InventoryLocation.Box:
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
