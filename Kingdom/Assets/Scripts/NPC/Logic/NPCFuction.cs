using System.Collections.Generic;
using UnityEngine;
//记录NPC本身所有交互动作，数据，玩家本身也可以是NPC
public class NPCFuction : MonoBehaviour
{
    //背包数据
    public InventoryBag_SO shopData;

    private bool isOpen;

    void Update(){
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    public void OnFinishSpeak()
    {
        //当前脚本里负责开启商店,通知UI显示开启商店按钮
        EventHandler.CallOnShowSelectTipsEvent("shopBtnID","商店",OpenShop);
    }

    public void OpenShop()
    {  
        isOpen = true;
        EventHandler.CallBaseBagOpenEvent(SlotType.Shop,shopData);
        EventHandler.CallDisShowSelectAllTipsEvent();//开启商店后关闭右侧所有按钮
    }

    public void CloseShop(){
        isOpen = false;
        EventHandler.CallBaseBagCloseEvent(SlotType.Shop);
    }
    
    //任务数据,记录当前玩家或NPC接取的任务
    public List<int> taskIDList=new ();//可以记录任务ID，具体任务信息，在查看时可以再去查找

    public void AddTask(int ID)
    {
        taskIDList.Add(ID);
    }

    void OnTriggerEnter(Collider other)
    {
        var npcFunction = other.GetComponent<NPCFuction>();
        if (npcFunction)
        {
            Debug.Log("有对方进入");
            //EventHandler.CallShowTipsEvent(new TipsData("按下<color=\"black\">空格</color>对话", transform.position + Vector3.up * 1.8f, gameobjectID));
            otherNPCFunction = npcFunction;
        }
    }



    //向对方添加任务
    public NPCFuction otherNPCFunction;

    public void AddTaskToOther(int ID)
    {
        if (otherNPCFunction)
        {
            otherNPCFunction.AddTask(ID);
            otherNPCFunction = null;
        }
    }

    ///种地老头相关方法
    public void OnFinishOpenBuyTitle()
    {
        EventHandler.CallOnShowSelectTipsEvent("BuyTitleID","50开垦田地",CheckMoneyBuyTitle);
    }

    public void CheckMoneyBuyTitle()
    {
        if (otherNPCFunction.shopData.money>=50)
        {
            otherNPCFunction.shopData.money -=50;
            GridMapManager.Instance.OpenCropTile();//开启一块地
            EventHandler.CallShowTextTipsEvent("开垦了一块田地");
        }else{
            EventHandler.CallShowTextTipsEvent("钱不够，搞点钱去吧");
        }
        EventHandler.CallDisShowSelectAllTipsEvent();//关闭右侧选择按钮   
    }
}
