using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridMapManager : Singleton<GridMapManager>
{
    [Header("地图信息")]
    public List<MapData_SO> mapDatalist;
    public Transform tileParent;
    private Season currentSeason;

    //场景名字+坐标和对应的瓦片信息
    private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();

    private void OnEnable()
    {
        EventHandler.ExecuteActionAfterAnimation += OnExecuteActionAfterAnimation;
        //EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.GameDayEvent += OnGameDayEvent;
        EventHandler.RefreshCurrentMap += RefreshMap;
    }
    private void OnDisable()
    {
        EventHandler.ExecuteActionAfterAnimation -= OnExecuteActionAfterAnimation;
        //EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.GameDayEvent -= OnGameDayEvent;
        EventHandler.RefreshCurrentMap -= RefreshMap;
    }

    /// <summary>
    /// 每天执行一次
    /// </summary>
    /// <param name="day"></param>
    /// <param name="season"></param>
    private void OnGameDayEvent(int day, Season season)
    {
        currentSeason = season;

        foreach (var tile in tileDetailsDict)
        {
            // if (tile.Value.daySinceWatered > -1)
            // {
            //     tile.Value.daySinceWatered = -1;
            // }
            // if (tile.Value.daySinceDug > -1)
            // {
            //     tile.Value.daySinceDug++;
            // }
            // //超期消除挖坑
            // if (tile.Value.daySinceDug > 5 && tile.Value.seedItemID == -1)
            // {
            //     tile.Value.daySinceDug = -1;
            //     tile.Value.canDig = true;
            //     tile.Value.growthDays = -1;
            // }
            if (tile.Value.seedItemID != -1)//给种子增加生长时间
            {
                tile.Value.growthDays++;
            }
        }

        RefreshMap();
    }
    private void Start()
    {
        //ISaveable saveable = this;
        //saveable.RegisterSaveable();

        foreach (var mapData in mapDatalist)
        {
            //firstLoadDict.Add(mapData.sceneName, true);
            InitTileDetailsDict(mapData);
        }
    }


    /// <summary>
    /// 根据地图信息生成字典
    /// </summary>
    /// <param name="mapData"></param>
    private void InitTileDetailsDict(MapData_SO mapData)
    {
        foreach (TileProperty tileProperty in mapData.tileProperties)
        {
            TileDetails tileDetails = new TileDetails
            {
                worldPos = tileProperty.worldPos
            };

            //字典的Key
            string key = tileDetails.worldPos.x.ToString("00") + "x" + tileDetails.worldPos.y.ToString("00") + "y" + tileDetails.worldPos.z.ToString("00") + "z" + mapData.sceneName;

            if (GetTileDetails(key) != null)
            {
                tileDetails = GetTileDetails(key);
            }

            switch (tileProperty.gridType)
            {
                case GridType.Diggable:
                    tileDetails.canDig = tileProperty.boolTypeValue;
                    break;
                case GridType.DropItem:
                    tileDetails.canDropItem = tileProperty.boolTypeValue;
                    break;
                case GridType.PlaceFurniture:
                    tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                    break;
                case GridType.NPCObstacle:
                    tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                    break;
            }

            if (GetTileDetails(key) != null)
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails);
            }
            if (tileProperty.canUseObj == null)
            {
                var title = tileParent.Find(tileProperty.tileName);
                title.GetComponent<Collider>().enabled = tileProperty.canUse;
                title.Find("CanUse").gameObject.SetActive(tileProperty.canUse);
            }
            else
            {
                //根据存储信息，设定当前地块是否可以启用
                tileProperty.canUseObj.SetActive(tileProperty.canUse);
                tileProperty.canUseObj.transform.parent.GetComponent<Collider>().enabled = tileProperty.canUse;
            }
        }
    }

    /// <summary>
    /// 根据key返回瓦片信息
    /// </summary>
    /// <param name="key">x+y+地图名字</param>
    /// <returns></returns>
    public TileDetails GetTileDetails(string key)
    {
        if (tileDetailsDict.ContainsKey(key))
        {
            return tileDetailsDict[key];
        }
        else return null;
    }

    /// <summary>
    /// 刷新当前地图
    /// </summary>
    private void RefreshMap()
    {
        // if (digTilemap != null)
        //     digTilemap.ClearAllTiles();
        // if (waterTilemap != null)
        //     waterTilemap.ClearAllTiles();

        foreach (var crop in FindObjectsByType<Crop>(FindObjectsSortMode.None))
        {
            Destroy(crop.gameObject);
        }

        DisplayMap(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// 显示地图瓦片
    /// </summary>
    /// <param name="sceneName"></param>
    private void DisplayMap(string sceneName)
    {
        foreach (var tile in tileDetailsDict)
        {
            var key = tile.Key;
            var tileDetails = tile.Value;

            if (key.Contains(sceneName))
            {
                // if (tileDetails.daySinceDug > -1)
                //     SetDigGround(tileDetails);
                // if (tileDetails.daySinceWatered > -1)
                //     SetWaterGround(tileDetails);
                if (tileDetails.seedItemID > -1)
                    EventHandler.CallPlantSeedEvent(tileDetails.seedItemID, tileDetails);
            }
        }
    }

    /// <summary>
    /// 执行实际工具或物品功能
    /// </summary>
    /// <param name="mouseWorldPos">角色进入的位置</param>
    /// <param name="itemDetails">物品信息</param>
    private void OnExecuteActionAfterAnimation(Vector3 enterPos, ItemDetails itemDetails)
    {
        //var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
        var currentTile = GetTileDetailsOnMousePosition(enterPos);
        Debug.Log("当前title是空么" + currentTile);
        if (currentTile != null)
        {
            //Crop currentCrop = GetCropObject(mouseWorldPos);

            //TODO:物品使用实际功能
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:             //种子
                    if (currentTile.seedItemID != -1)//当前地块已经种植，无法继续种植
                    {
                        Debug.Log("已经种了，不让种了");
                        return;
                    }
                    EventHandler.CallPlantSeedEvent(itemDetails.itemID, currentTile);
                    EventHandler.CallDropItemEvent(itemDetails.itemID, enterPos, itemDetails.itemType);
                    //EventHandler.CallPlaySoundEvent(SoundName.Plant);
                    break;
                // case ItemType.Commodity:        //商品
                //     EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, itemDetails.itemType);
                //     break;
                // case ItemType.HoeTool:          //锄头
                //     SetDigGround(currentTile);
                //     currentTile.daySinceDug = 0;
                //     currentTile.canDig = false;
                //     currentTile.canDropItem = false;
                //     //加音效
                //     EventHandler.CallPlaySoundEvent(SoundName.Hoe);
                //     break;
                // case ItemType.WaterTool:    //水壶
                //     SetWaterGround(currentTile);
                //     currentTile.daySinceWatered = 0;
                //     //音效
                //     EventHandler.CallPlaySoundEvent(SoundName.Water);
                //     break;
                // case ItemType.BreakTool:    //十字镐
                // case ItemType.ChopTool:     //斧头
                //     //执行收割方法
                //     currentCrop.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                //     break;
                // case ItemType.CollectTool:  //菜篮子
                //     //Crop currentCrop = GetCropObject(mouseWorldPos);
                //     //执行收割方法
                //     currentCrop.ProcessToolAction(itemDetails, currentTile);
                //     break;
                // case ItemType.ReapTool:     //镰刀
                //     var reapCount = 0;
                //     for (int i = 0; i < itemInRadius.Count; i++)
                //     {
                //         EventHandler.CallParticalEffectEvent(ParticaleEffectType.ReapableScenery, itemInRadius[i].transform.position + Vector3.up);
                //         itemInRadius[i].SpawnHarvestItems();
                //         Destroy(itemInRadius[i].gameObject);
                //         reapCount++;
                //         if (reapCount >= Settings.reapAmount)
                //             break;
                //     }
                //     EventHandler.CallPlaySoundEvent(SoundName.Reap);
                //     break;
                case ItemType.Furniture:
                    //在地图上生成物品 ItemManager
                    //移除当前物品（图纸） InventoryManager
                    //移除资源物品 InventoryManager
                    //EventHandler.CallBuildFurnitureEvent(itemDetails.itemID, mouseWorldPos);
                    break;
            }

            UpdateTileDetails(currentTile);
        }
    }

    /// <summary>
    /// 根据坐标返回瓦片信息
    /// </summary>
    /// <param name="mouseGridPos">鼠标网格坐标</param>
    /// <returns></returns>
    public TileDetails GetTileDetailsOnMousePosition(Vector3 mouseGridPos)
    {
        string key = mouseGridPos.x.ToString("00") + "x" + mouseGridPos.y.ToString("00") + "y" + mouseGridPos.z.ToString("00") + "z" + SceneManager.GetActiveScene().name;
        return GetTileDetails(key);
    }
    /// <summary>
    /// 更新瓦片信息
    /// </summary>
    /// <param name="tileDetails">tileDetails</param>
    public void UpdateTileDetails(TileDetails tileDetails)
    {
        string key = tileDetails.worldPos.x.ToString("00") + "x" + tileDetails.worldPos.y.ToString("00") + "y" + tileDetails.worldPos.z.ToString("00") + "z" + SceneManager.GetActiveScene().name;
        if (tileDetailsDict.ContainsKey(key))
        {
            tileDetailsDict[key] = tileDetails;
        }
        else
        {
            tileDetailsDict.Add(key, tileDetails);
        }
    }

    public bool CheckTileHasSeed(Vector3 mouseGridPos)
    {
        var tileDetail = GetTileDetailsOnMousePosition(mouseGridPos);
        if (tileDetail == null)
        {
            return false;
        }

        if (tileDetail.seedItemID != -1)
        {
            return true;
        }
        return false;
    }

    [ContextMenu("openTitle")]
    //开启可种植区域
    public void OpenCropTile()
    {
        var titleList = mapDatalist[0];
        foreach (var item in titleList.tileProperties)
        {
            if (!item.canUse)
            {
                item.canUse = true;//设定可以种植
                item.canUseObj.SetActive(true);
                item.canUseObj.transform.parent.GetComponent<Collider>().enabled = true;//打开碰撞器
                break;
            }
        }
    }

    public void CloseCropTile()
    {

    }
}
