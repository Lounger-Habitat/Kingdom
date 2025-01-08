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

            //进入种植区域，打开种植面板
           EventHandler.CallShowCropPanelEvent(bag_SO,other.transform.position);
        }
    }
}
