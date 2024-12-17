using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [Header("物品数据")] public ItemDataList_SO itemDataList_SO;

    [Header("背包数据")] public InventoryBag_SO playerBag;

    private void Start()
    {
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.itemList);
    }

    public ItemDetails GetItemDetails(int ID)
    {
        return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
    }

    /// <summary>
    /// 向背包增加物品
    /// </summary>
    /// <param name="item"></param>
    /// <param name="toDestory"></param>
    public void AddItem(Item item, bool toDestory)
    {
        var index = GetItemIndexInBag(item.itemID);
        AddItemAtIndex(item.itemID,index,1);//TODO:后续物品数量是动态
        Debug.Log(GetItemDetails(item.itemID).itemID + "Name" + GetItemDetails(item.itemID).ItemName);
        if (toDestory)
        {
            Destroy(item.gameObject);
        }
    }

    private bool CheckBagCapacity(out int index)
    {
        index = -1;
        for (int i = 0; i < playerBag.itemList.Count; i++)
        {
            if (playerBag.itemList[i].itemID == 0)
            {
                index = i;
                return true;
            }
        }

        return false;
    }

    private int GetItemIndexInBag(int ID)
    {
        for (int i = 0; i < playerBag.itemList.Count; i++)
        {
            if (playerBag.itemList[i].itemID == ID)
            {
                return i;
            }
        }

        return -1;
    }

    private void AddItemAtIndex(int ID, int index, int amount)
    {
        int i = 0;
        if (index == -1 && CheckBagCapacity(out i)) //背包没有这个物体，并且有空位
        {
            var item = new InventoryItem {itemID = ID, itemAmount = amount};
            playerBag.itemList[i] = item;
        }
        else if (i != -1) //背包有这个物体
        {
            Debug.Log("背包有这个物品，增加数量");
            int currentAmount = playerBag.itemList[index].itemAmount + amount;
            var item = new InventoryItem { itemID = ID,itemAmount = currentAmount};
            playerBag.itemList[index] = item;
        }
        else //背包没有这个物体，并且背包已经满了
        {
            Debug.Log("背包已满，无法添加物品");
        }
        
        //更新背包UI
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.itemList);
    }

    //同背包里交换物品
    public void SwapItem(int fromIndex,int targetIndex)
    {
        InventoryItem currentItem = playerBag.itemList[fromIndex];
        InventoryItem targetItem = playerBag.itemList[targetIndex];

        if (targetItem.itemID != 0)
        {
            playerBag.itemList[targetIndex] = currentItem;
            playerBag.itemList[fromIndex] = targetItem;
        }
        else
        {
            playerBag.itemList[targetIndex] = currentItem;
            playerBag.itemList[fromIndex] = new InventoryItem();
        }
        
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player,playerBag.itemList);
    }
}