using Unity.VisualScripting;
using UnityEngine;

public class CropEnter : MonoBehaviour
{
    public InventoryBag_SO bag_SO;//TODO:更改为从当前进入的NPC获取
    void OnTriggerEnter(Collider other)//进入种植区域
    {
        Debug.Log("go enter");
        if (other.tag.Contains("Tile"))
        {
            if (GridMapManager.Instance.CheckTileHasSeed(other.transform.position))
            {
                return;//有种子种植了，无需再次打开种植面板
            }
                //进入种植区域，打开种植面板
            EventHandler.CallShowCropPanelEvent(bag_SO, other.transform.position);
        }
    }

    void OnTriggerExit(Collider other)//退出
    {
        Debug.Log("go Exit");
        if (other.tag.Contains("Tile"))
        {

            //进入种植区域，打开种植面板
            EventHandler.CallDisShowCropPanelEvent();
        }
    }
}
