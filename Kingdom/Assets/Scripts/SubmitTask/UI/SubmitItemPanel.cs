using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubmitItemPanel : MonoBehaviour
{
    [SerializeField] GameObject SubmitPanel;
    public GameObject SubmitSlotPrefab;

    [SerializeField] private List<SubmitSlotUI> SubmitBagSlots;
    public Button submitBtn;

    public TMP_Text totalNumberText;

    private InventoryBag_SO currentBag;
    private void OnEnable()
    {
        EventHandler.ShowSubmitPanelEvent += OnShowSubmitPanelEvent;
        EventHandler.DisShowSubmitPanelEvent += OnCloseSubmitPanelEvent;

        //EventHandler.UpdateCropUIEvent += OnUpdateCropUIEvent;
    }
    private void OnDisable()
    {
        EventHandler.ShowSubmitPanelEvent -= OnShowSubmitPanelEvent;
        EventHandler.DisShowSubmitPanelEvent -= OnCloseSubmitPanelEvent;
        //EventHandler.UpdateCropUIEvent -= OnUpdateCropUIEvent;
    }

    private void OnUpdateCropUIEvent()
    {
        if (SubmitPanel.activeSelf)
        {
            ClearSlotUI();
            //OnShowCropPanelEvent(playerBag, cropPoint);
        }
    }
    private void ClearSlotUI()//清除种植UI
    {
        foreach (var item in SubmitBagSlots)
        {
            Destroy(item.gameObject);
        }
        SubmitBagSlots.Clear();
    }

    List<InventoryItem> needItemList = new ();
    TaskDetail currentTaskDetail;
    bool allItemNeed;
    //将背包里的任务相关显示出来
    private void OnShowSubmitPanelEvent(InventoryBag_SO bagData, TaskDetail taskDetail)
    {
        currentBag = bagData;
        currentTaskDetail = taskDetail;
        SubmitPanel.SetActive(true);
        if (SubmitBagSlots != null) ClearSlotUI();
        SubmitBagSlots = new();
        //detailList = new ();
        allItemNeed = taskDetail.itemNumbers[0] != -1;
        int totalNumber = 0;
        int index = 0;
        bool canSubmitItem = false;
        //根据任务需求
        for (int i = 0; i < taskDetail.itemID.Count; i++)//查询所需物品ID
        {
            var item = bagData.itemList.Find(id => id.itemID == taskDetail.itemID[i]);
            var detail = InventoryManager.Instance.GetItemDetails(item.itemID);
            if (allItemNeed)
            {
                canSubmitItem = canSubmitItem && item.itemAmount>= taskDetail.itemNumbers[i];//检查当前背包物品数量是否大于任务所需数量
            }
            else
            { //不是所有物品都需要，只是数量够就可以
              //检查背包里是否有当前物品
                
                if (item.itemAmount > 0)
                {
                    //有这个物品，显示这个物品
                    needItemList.Add(item);
                    if (detail != null)
                    {
                        var slot = Instantiate(SubmitSlotPrefab, SubmitPanel.transform).GetComponent<SubmitSlotUI>();
                        slot.slotIndex = index;
                        SubmitBagSlots.Add(slot);
                        slot.UpdateSlot(detail,item.itemAmount);
                        index++;
                    }
                    totalNumber +=item.itemAmount;
                }
            }
        }
        totalNumberText.gameObject.SetActive(!allItemNeed);
        if (!allItemNeed)
        {
            canSubmitItem = totalNumber>=taskDetail.itemNumbers[1];//当前所有物品总量是否大于所需数量
            totalNumberText.text = $"{totalNumber}/{taskDetail.itemNumbers[1]}";
        }
        submitBtn.interactable = canSubmitItem;//检查提交按钮是否可用
        LayoutRebuilder.ForceRebuildLayoutImmediate(SubmitPanel.transform as RectTransform);
    }

    private void OnCloseSubmitPanelEvent()
    {
        if (!SubmitPanel.activeSelf) return;
        SubmitPanel.SetActive(false);
        ClearSlotUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnCloseSubmitPanelEvent();
        }
    }

    public void OnClickSubmitBtn()
    {
        //点击了提交按钮，把背包里东西相应减少，
        if (allItemNeed)
        {
            //如果是需要所有物品都集齐    
        }else
        {   //只需总量符合要求即可
            int totalNeedNumber = currentTaskDetail.itemNumbers[1];
            for (int i = 0; i < needItemList.Count; i++)
            {
                if (totalNeedNumber>=needItemList[i].itemAmount)
                {
                    InventoryManager.Instance.RemoveItem(needItemList[i].itemID,needItemList[i].itemAmount,currentBag);
                    totalNeedNumber -=needItemList[i].itemAmount;
                }else
                {
                    InventoryManager.Instance.RemoveItem(needItemList[i].itemID,totalNeedNumber,currentBag);
                    totalNeedNumber = 0;
                }
                if (totalNeedNumber.Equals(0))
                {
                    //已经删除完成，跳出循环
                    break;
                }
            }

        }
        //给背包里塞入对应奖励
        for (int i = 0; i < currentTaskDetail.rewardItemID.Count; i++)
        {
            InventoryManager.Instance.AddItemAtBag(currentBag,currentTaskDetail.rewardItemID[i],currentTaskDetail.rewardNumber[i]);
            var itemDetail = InventoryManager.Instance.GetItemDetails(currentTaskDetail.rewardItemID[i]);
            EventHandler.CallShowTextTipsEvent($"获得{itemDetail.ItemName}x{currentTaskDetail.rewardNumber[i]}");
        }
        if (currentTaskDetail.rewardMoney>0)//金钱奖励是否大于0
        {
            currentBag.money += currentTaskDetail.rewardMoney;
            EventHandler.CallShowTextTipsEvent($"获得金钱x{currentTaskDetail.rewardMoney}");
        }

        //当前任务已经完成，修改数据
        currentTaskDetail.isDone = true;
        TaskManager.Instance.taskDataList_SO.ModifyTaskData(currentTaskDetail);
        //将提交面板关闭
        OnCloseSubmitPanelEvent();
    }
}
