using System;
using UnityEngine;
using UnityEngine.Events;

public static class EventHandler
{
    public static event Action<InventoryLocation, InventoryBag_SO> updateInventoryUI;

    public static void CallUpdateInventoryUI(InventoryLocation location, InventoryBag_SO bag_SO)
    {
        updateInventoryUI?.Invoke(location, bag_SO);
    }

    public static event Action UpdateCropUIEvent;

    public static void CallUpdateCropUIEvent()
    {
        UpdateCropUIEvent?.Invoke();
    }

    public static event Action<int, Vector3> instantiateItemInScene;
    public static void CallInstantiateItemInScene(int ID, Vector3 pos)
    {
        instantiateItemInScene?.Invoke(ID, pos);
    }

    public static event Action<int, int> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute, int hour)
    {
        GameMinuteEvent?.Invoke(minute, hour);
    }

    public static event Action<int, int, int, int, Season> GameDateEvent;

    public static void CallGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }

    public static event Action<DialoguePiece> ShowDialogueEvent;

    public static void CallShowDialogueEvent(DialoguePiece piece)
    {
        ShowDialogueEvent?.Invoke(piece);
    }

    public static event Action<TipsData> ShowTipsEvent;
    public static void CallShowTipsEvent(TipsData data)
    {
        ShowTipsEvent?.Invoke(data);
    }

    public static event Action<string> DisShowTipsEvent;
    public static void CallDisShowTipsEvent(string gameobjectID)
    {
        DisShowTipsEvent?.Invoke(gameobjectID);
    }

    public static event Action<SlotType, InventoryBag_SO> BaseBagOpenEvent;

    public static void CallBaseBagOpenEvent(SlotType slotype, InventoryBag_SO bag_So)
    {
        BaseBagOpenEvent?.Invoke(slotype, bag_So);
    }

    public static event Action<SlotType> BaseBagCloseEvent;

    public static void CallBaseBagCloseEvent(SlotType slotype)
    {
        BaseBagCloseEvent?.Invoke(slotype);
    }

    public static event Action<ItemDetails, bool> ShowTradeUI;

    public static void CallShowTradeUI(ItemDetails item, bool isSell)
    {
        ShowTradeUI?.Invoke(item, isSell);
    }
    public static event Action<int, TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int ID, TileDetails tile)
    {
        PlantSeedEvent?.Invoke(ID, tile);
    }
    public static event Action<int, Season> GameDayEvent;
    public static void CallGameDayEvent(int day, Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;
    public static void CallExecuteActionAfterAnimation(Vector3 pos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }

    public static event Action<InventoryBag_SO, Vector3> ShowCropPanelEvent;
    public static void CallShowCropPanelEvent(InventoryBag_SO bagData, Vector3 worldPos)
    {
        ShowCropPanelEvent?.Invoke(bagData, worldPos);
    }
    public static event Action DisShowCropPanelEvent;
    public static void CallDisShowCropPanelEvent()
    {
        DisShowCropPanelEvent?.Invoke();
    }

    public static event Action<InventoryBag_SO, TaskDetail> ShowSubmitPanelEvent;
    public static void CallShowSubmitPanelEvent(InventoryBag_SO bagData, TaskDetail taskDetail)
    {
        ShowSubmitPanelEvent?.Invoke(bagData, taskDetail);
    }
    public static event Action DisShowSubmitPanelEvent;
    public static void CallDisShowSubmitPanelEvent()
    {
        DisShowSubmitPanelEvent?.Invoke();
    }
    public static event Action<int, Vector3, ItemType> DropItemEvent;
    public static void CallDropItemEvent(int ID, Vector3 pos, ItemType itemType)
    {
        DropItemEvent?.Invoke(ID, pos, itemType);
    }

    public static event Action<string> ShowCropTipsEvent;
    public static void CallShowCropTips(string ID)
    {
        ShowCropTipsEvent?.Invoke(ID);
    }

    public static event Action<string> DisShowCropTipsEvent;
    public static void CallDisShowCropTips(string ID)
    {
        DisShowCropTipsEvent?.Invoke(ID);
    }

    public static event Action<int> HarvestAtPlayerPosition;
    public static void CallHarvestAtPlayerPosition(int ID)
    {
        HarvestAtPlayerPosition?.Invoke(ID);
    }

    public static event Action RefreshCurrentMap;
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }

    public static event Action<string> ShowTextTipsEvent;
    public static void CallShowTextTipsEvent(string msg)
    {
        ShowTextTipsEvent?.Invoke(msg);
    }

    public static event Action<float> FadePanelEvent;
    /// <summary>
    /// 渐变Panel
    /// </summary>
    /// <param name="value">1黑屏，0透明</param>
    public static void CallFadePanelEvent(float value)
    {
        FadePanelEvent?.Invoke(value);
    }


    //对话完成后右侧选择按钮UI

    public static event Action<string,string,UnityAction> OnShowSelectTipsEvent;
    /// <summary>
    /// 打开右侧对话选择按钮，可以生成多个
    /// </summary>
    /// <param name="ID">当前按钮ID，通过此ID删除按钮</param>
    /// <param name="btnMsg">按钮显示信息</param>
    /// <param name="btnOnClick">按钮点击后触发回调</param>
    public static void CallOnShowSelectTipsEvent(string ID,string btnMsg,UnityAction btnOnClick)
    {
        OnShowSelectTipsEvent?.Invoke(ID,btnMsg,btnOnClick);
    }

    public static event Action<string> DisShowSelectTipsEvent;
    /// <summary>
    /// 删除右侧对话选择按钮
    /// </summary>
    /// <param name="ID">当前按钮ID，通过此ID删除按钮</param>
    public static void CallDisShowSelectTipsEvent(string ID)
    {
        DisShowSelectTipsEvent?.Invoke(ID);
    }

    public static event Action DisShowSelectAllTipsEvent;
    /// <summary>
    /// 删除所有右侧对话按钮
    /// </summary>
    public static void CallDisShowSelectAllTipsEvent()
    {
        DisShowSelectAllTipsEvent?.Invoke();
    }

    public static event Action<TipsData> ShowTalkEvent;
    public static void CallShowTalkEvent(TipsData data)
    {
        ShowTalkEvent?.Invoke(data);
    }

    public static event Action<string> DisShowTalkEvent;
    public static void CallDisShowTalkEvent(string gameobjectID)
    {
        DisShowTalkEvent?.Invoke(gameobjectID);
    }
}
