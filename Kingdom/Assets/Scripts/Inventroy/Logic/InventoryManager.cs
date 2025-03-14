using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [Header("物品数据")] public ItemDataList_SO itemDataList_SO;

    [Header("背包数据")] public InventoryBag_SO playerBag;

    //public int playerMoney;

    private void OnEnable()
    {
        EventHandler.DropItemEvent += OnDropItemEvent;
        EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
        //EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
        //EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        //EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }
    private void OnDisable()
    {
        EventHandler.DropItemEvent -= OnDropItemEvent;
        EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
        //EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
        //EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
        //EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }
    private void Start()
    {
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag);
    }

    public ItemDetails GetItemDetails(int ID)
    {
        return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
    }


    private void OnDropItemEvent(int ID, Vector3 pos, ItemType itemType)
    {
        RemoveItem(ID, 1);
    }

    /// <summary>
    /// 向背包增加物品
    /// </summary>
    /// <param name="item"></param>
    /// <param name="toDestory"></param>
    public void AddItem(Item item, bool toDestory)
    {
        var index = GetItemIndexInBag(item.itemID);
        AddItemAtIndex(item.itemID, index, 1);//TODO:后续物品数量是动态
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

    private void OnHarvestAtPlayerPosition(int ID)
    {
        //是否已经有该物品
        var index = GetItemIndexInBag(ID);

        AddItemAtIndex(ID, index, 1);

        //更新UI
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag);
    }

    private void AddItemAtIndex(int ID, int index, int amount)
    {
        int i = 0;
        if (index == -1 && CheckBagCapacity(out i)) //背包没有这个物体，并且有空位
        {
            var item = new InventoryItem { itemID = ID, itemAmount = amount };
            playerBag.itemList[i] = item;
        }
        else if (i != -1) //背包有这个物体
        {
            Debug.Log("背包有这个物品，增加数量");
            int currentAmount = playerBag.itemList[index].itemAmount + amount;
            var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
            playerBag.itemList[index] = item;
        }
        else //背包没有这个物体，并且背包已经满了
        {
            Debug.Log("背包已满，无法添加物品");
        }

        //更新背包UI
        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag);
    }

    //同背包里交换物品
    public void SwapItem(int fromIndex, int targetIndex)
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

        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag);
    }

    /// <summary>
    /// 移除指定数量的背包物品
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <param name="removeAmount">数量</param>
    public void RemoveItem(int ID, int removeAmount)
    {
        var index = GetItemIndexInBag(ID);

        if (playerBag.itemList[index].itemAmount > removeAmount)
        {
            var amount = playerBag.itemList[index].itemAmount - removeAmount;
            var item = new InventoryItem { itemID = ID, itemAmount = amount };
            playerBag.itemList[index] = item;
        }
        else if (playerBag.itemList[index].itemAmount == removeAmount)
        {
            var item = new InventoryItem();
            playerBag.itemList[index] = item;
        }

        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag);
    }

    public void TradeItem(ItemDetails itemDetails, int amount, bool isSell)
    {
        int cost = itemDetails.itemPrice * amount;

        //获取在背包的位置，可能没有
        int index = GetItemIndexInBag(itemDetails.itemID);

        if (isSell)//卖
        {
            if (playerBag.itemList[index].itemAmount >= amount)
            {
                //数量够，可以卖出
                RemoveItem(itemDetails.itemID, amount);
                cost = (int)(cost * itemDetails.sellPercentage);
                playerBag.money += cost;

            }
            else
            {
                //数量不够啊兄弟，再想想
            }
        }
        else//买
        {
            if (playerBag.money - cost >= 0)//钱够
            {
                AddItemAtIndex(itemDetails.itemID, index, amount);//TODO：可能有背包已满情况
                playerBag.money -= cost;
            }
            else
            {
                //钱不够再想想
            }
        }

        EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 下方方法不再只针对玩家，会有NPC 与 NPC之间的交换，所以会传入背包，不在固定使用玩家背包
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void AddItemAtBag(InventoryBag_SO bag,int ID, int amount)
    {
        //是否已经有该物品
        var index = GetItemIndexInBag(ID,bag);
        int i = 0;
        if (index == -1 && CheckBagCapacity(bag,out i)) //背包没有这个物体，并且有空位
        {
            var item = new InventoryItem { itemID = ID, itemAmount = amount };
            bag.itemList[i] = item;
        }
        else if (i != -1) //背包有这个物体
        {
            Debug.Log("背包有这个物品，增加数量");
            int currentAmount = bag.itemList[index].itemAmount + amount;
            var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
            bag.itemList[index] = item;
        }
        else //背包没有这个物体，并且背包已经满了
        {
            Debug.Log("背包已满，无法添加物品");
        }

        //更新背包UI
        //EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag);
    }
    private bool CheckBagCapacity(InventoryBag_SO bag,out int index)
    {
        index = -1;
        for (int i = 0; i < bag.itemList.Count; i++)
        {
            if (bag.itemList[i].itemID == 0)
            {
                index = i;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 移除指定数量的背包物品
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <param name="removeAmount">数量</param>
    /// <param name="bag">背包</param>
    public void RemoveItem(int ID, int removeAmount,InventoryBag_SO bag)
    {
        var index = GetItemIndexInBag(ID,bag);

        if (bag.itemList[index].itemAmount > removeAmount)
        {
            var amount = bag.itemList[index].itemAmount - removeAmount;
            var item = new InventoryItem { itemID = ID, itemAmount = amount };
            bag.itemList[index] = item;
        }
        else if (bag.itemList[index].itemAmount == removeAmount)
        {
            var item = new InventoryItem();
            bag.itemList[index] = item;
        }

        //EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, bag);
    }

    private int GetItemIndexInBag(int ID,InventoryBag_SO bag)
    {
        for (int i = 0; i < bag.itemList.Count; i++)
        {
            if (bag.itemList[i].itemID == ID)
            {
                return i;
            }
        }
        return -1;
    }
}