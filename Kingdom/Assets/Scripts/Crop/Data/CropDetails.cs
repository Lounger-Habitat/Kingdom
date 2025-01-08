using UnityEngine;

[System.Serializable]
public class CropDetails
{
    public int seedItemID;

    [Header("不同阶段需要的天数")]
    public int[] growthDays;

    public int TotalGrowthDays{
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }

    [Header("不同阶段物品的prefab")]
    public GameObject[] growthPrefabs;

    public Sprite[] growthSprite;

    [Header("可种植季节")]
    public Season[] seasons;


    [Space]
    [Header("收割工具")]
    public int[] harvestToolItemID;
    [Header("每种工具使用次数")]
    public int[] requireActionCount;
    [Header("转换新物品ID")]
    public int transferItemID;

    [Space]
    [Header("收割果实信息")]
    public int[] producedItemID;              //生产个数

    public int[] producedMinAmount;           //生产个数最小值

    public int[] producedMaxAmount;           //生产个数最大值

    public Vector2 spawnRadius;               //生长范围

    [Header("再次生长时间")]
    public int daysToRegrow;

    public int regrowTimes;                   //可再次生长次数

    [Header("Options")]
    public bool generateAtPlayerPosition;     //是否在Player身上生成
    public bool hasAnimation;
    public bool hasParticalEffect;            //是否有粒子特效

    public ParticaleEffectType effectType;

    public Vector3 effectPos;

    public SoundName soundEffect;

    /// <summary>
    /// 检查当前工具是否可用
    /// </summary>
    /// <param name="toolID">工具ID</param>
    /// <returns></returns>
    public bool CheckToolAvailable(int toolID)
    {
        foreach(var tool in harvestToolItemID)
        {
            if (tool == toolID)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 获得工具需要使用的次数
    /// </summary>
    /// <param name="toolID">工具ID</param>
    /// <returns></returns>
    public int GetTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
        {
            if (harvestToolItemID[i] == toolID)
                return requireActionCount[i];
        }
        return -1;
    }
}
