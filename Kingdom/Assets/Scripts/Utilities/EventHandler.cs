using System;
using UnityEngine;

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
}
