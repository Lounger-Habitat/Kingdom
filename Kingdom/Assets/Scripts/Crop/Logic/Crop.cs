using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;

    public TileDetails tileDetails=> GridMapManager.Instance.GetTileDetailsOnMousePosition(transform.position);

    private int harvestActionCount;

    public bool CanHarvest => tileDetails.growthDays >= cropDetails.TotalGrowthDays;

    private string tipsID =>GetInstanceID().ToString();

    private bool playerEnter = false;
    // private Animator anim;

    // private Transform PlayerTransform => FindObjectOfType<Player>().transform;

    void OnTriggerEnter(Collider other){
        if (other.tag.Contains("Player"))
        {
            playerEnter = true;
            if (CanHarvest)//植物成熟了，可以采摘，通知UI显示
            {
                EventHandler.CallShowCropTips(tipsID);
            }
        }
    }

    void OnTriggerExit(Collider other){
        if (other.tag.Contains("Player"))
        {
            playerEnter = false;
            //通知UI关闭采摘
            EventHandler.CallDisShowCropTips(tipsID);
        }
    }

    void Update(){
        if (CanHarvest && playerEnter)
        {
            if (Input.GetKeyDown(KeyCode.F))//按下F采摘
            {
                Debug.Log("采摘");
                SpawnHarvestItems();
            }
        }
    }

    public void ProcessToolAction(ItemDetails tool, TileDetails tile)
    {
        //tileDetails = tile;

        //工具使用次数
        //int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
        //if (requireActionCount == -1) return;

        //anim = GetComponentInChildren<Animator>();

        //点击计数器
        //if (harvestActionCount < requireActionCount)
        //{
        //    harvestActionCount++;
            //判断是否有动画 树木
            // if (anim != null && cropDetails.hasAnimation)
            // {
            //     if (PlayerTransform.position.x < transform.position.x)
            //         anim.SetTrigger("RotateRight");
            //     else
            //         anim.SetTrigger("RotateLeft");
            // }
            //播放粒子
            //if (cropDetails.hasParticalEffect)
            //     EventHandler.CallParticalEffectEvent(cropDetails.effectType, transform.position + cropDetails.effectPos);
            //播放声音
            //if (cropDetails.soundEffect != SoundName.none)
            // {
            //var soundDetails = AudioManager.Instance.soundDetailsData.GetSoundDetails(cropDetails.soundEffect);
            //EventHandler.CallInitSoundEffect(soundDetails);
            //EventHandler.CallPlaySoundEvent(cropDetails.soundEffect);
            // }
        //}

        // if (harvestActionCount >= requireActionCount)
        // {
        //     if (cropDetails.generateAtPlayerPosition || !cropDetails.hasAnimation)
        //     {
        //         //生成农作物
        //         SpawnHarvestItems();
        //     }
        //     else if (cropDetails.hasAnimation)
        //     {
        //         if (PlayerTransform.position.x < transform.position.x)
        //             anim.SetTrigger("FallingRight");
        //         else
        //             anim.SetTrigger("FallingLeft");
        //         //EventHandler.CallPlaySoundEvent(SoundName.TreeFalling);
        //         StartCoroutine(HarvestAfterAnimation());
        //     }
        // }
    }

    // private IEnumerator HarvestAfterAnimation()
    // {
    //     while (!anim.GetCurrentAnimatorStateInfo(0).IsName("END"))
    //     {
    //         yield return null;
    //     }

    //     SpawnHarvestItems();

    //     //转换新物体
    //     if (cropDetails.transferItemID > 0)
    //     {
    //         CreateTransferCrop();
    //     }
    // }

    // private void CreateTransferCrop()
    // {
    //     tileDetails.seedItemID = cropDetails.transferItemID;
    //     tileDetails.daysSinceLastHarvest = -1;
    //     tileDetails.growthDays = 0;

    //     EventHandler.CallRefreshCurrentMap();
    // }

    /// <summary>
    /// 生成果实
    /// </summary>
    public void SpawnHarvestItems()
    {
        for (int i = 0; i < cropDetails.producedItemID.Length; i++)
        {
            int amountToProduce;

            if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
            {
                //代表只生产指定数量的
                amountToProduce = cropDetails.producedMinAmount[i];
            }
            else    //物品随机数量
            {
                amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
            }
             // //执行生成指定数量的物品
            for (int j = 0; j < amountToProduce; j++)
            {
                EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
            }
            var producedItem = InventoryManager.Instance.GetItemDetails(cropDetails.producedItemID[i]);
            var msg = $"收获{producedItem.ItemName}x{amountToProduce}";
            EventHandler.CallShowTextTipsEvent(msg);
            // for (int j = 0; j < amountToProduce; j++)
            // {
            //     if (cropDetails.generateAtPlayerPosition)
            //     {
            //         EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
            //     }
            //     else    //世界地图上生成物品
            //     {
            //         //判断应该生成的物品方向
            //         var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
            //         //一定范围内的随机
            //         var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
            //             transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);

            //         EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);
            //     }
            // }
        }

        if (tileDetails != null)
        {
            tileDetails.daysSinceLastHarvest++;

            //是否可以重复生长
            if (cropDetails.daysToRegrow > 0 && tileDetails.daysSinceLastHarvest < cropDetails.regrowTimes)
            {
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
                //刷新种子
                EventHandler.CallRefreshCurrentMap();
            }
            else    //不可重复生长
            {
                tileDetails.daysSinceLastHarvest = -1;
                tileDetails.seedItemID = -1;
                //TODO:自己设计
                //tileDetails.daySinceDug = -1;
            }
            EventHandler.CallDisShowCropTips(tipsID);
            Destroy(gameObject);
        }
    }
}

