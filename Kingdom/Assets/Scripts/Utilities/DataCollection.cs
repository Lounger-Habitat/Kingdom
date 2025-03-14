using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemDetails
{
    public int itemID;
    public string ItemName;

    public ItemType itemType;

    public Sprite itemIcon;
    public Sprite itemOnWorldSprite;
    public GameObject itemPrefab;
    public string itemDescription;

    public int itemUseRadius;
    public bool canPickedup;
    public bool canDropped;
    public bool canCarried;
    public int itemPrice;

    [Range(0, 1)]
    public float sellPercentage;
}

[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}

[System.Serializable]
public class AnimatorType
{
    public PartType partType;
    public PartName partName;
    public AnimatorOverrideController overrideController;
}

[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 position;
}

[System.Serializable]
public class SceneFurniture
{
    public int itemID;
    public SerializableVector3 position;
    public int boxIndex;
}

[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;

    public GridType gridType;

    public bool boolTypeValue;

    public Vector3 worldPos;
    //是否可以使用
    public bool canUse;
    //
    public GameObject canUseObj;

    public string tileName;//测试使用名字获取
}

[System.Serializable]
public class TileDetails
{
    public int gridX, gridY;

    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNPCObstacle;

    public int daySinceDug = -1;
    public int daySinceWatered = -1;

    public int seedItemID = -1;

    public int growthDays = -1;
    public int daysSinceLastHarvest = -1;
    public bool canUse;

    public Vector3 worldPos;
}


[System.Serializable]
public class NPCPosition
{
    public Transform npc;

    public string startScene;

    public Vector3 position;
}


//
[System.Serializable]
public class SceneRoute
{
    public string fromSceneName;

    public string gotoSceneName;

    public List<ScenePath> scenePathList;
}

[System.Serializable]
public class ScenePath
{
    public string sceneName;

    public Vector2Int fromGridCell;

    public Vector2Int gotoGridCell;
}

[System.Serializable]
public class TaskDetail
{
    //任务ID
    public int ID;
    //任务名称
    public string taskName;
    //任务描述
    public string des;
    //任务是否完成
    public bool isDone;
    //任务所需物品ID,可能是击杀任务不需要
    public List<int> itemID;
    //任务物品所需数量,与上边一一对应，如果第一个数为-1，那么列表里第二个数字为总数量，只要所有物品数量总和符合即可完成
    public List<int> itemNumbers;
    //TODO:如果是击杀怪物任务，有怪物编号、击杀数量
    //前置任务，可能是多个任务
    public List<int> front_task;
    //是否已经接取任务
    public bool received;
    //奖励物品列表
    public List<int> rewardItemID;
    //奖励物品数量
    public List<int> rewardNumber;
    //奖励钱数量
    public int rewardMoney;
}

[System.Serializable]
public class TaskData
{
    //当前任务ID
    public int taskID;
    //当前任务对话列表
    public List<DialoguePiece> dialoguePieces;
}
