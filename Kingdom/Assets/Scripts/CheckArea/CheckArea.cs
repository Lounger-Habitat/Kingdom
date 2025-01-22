using Unity.VisualScripting;
using UnityEngine;

public class CheckArea : MonoBehaviour
{
    public int ID;//如果ID不为空，表示有这个物品才能进入对应区域

    [TextArea]
    public string tipsText;

    void OnTriggerEnter(Collider other)
    {
        if (ID>-1)
        {
            //需要物品才能进去
            var npcFunction = other.GetComponent<NPCFuction>();
            if (npcFunction.shopData.itemList.Find(item => item.itemID == ID).itemAmount>0)//有这个物品，可以进入
            {
                EventHandler.CallShowTextTipsEvent("通过验证，可以进入");
                transform.GetChild(0).GetComponent<Collider>().enabled = false;
            }
            else
            {
                EventHandler.CallShowTextTipsEvent(tipsText);
            }
        }
        else
        {
            //表示这块暂时不能进去
            EventHandler.CallShowTextTipsEvent(tipsText);
        }
    }

    void OnTriggerExit(Collider other)
    {

    }
}
