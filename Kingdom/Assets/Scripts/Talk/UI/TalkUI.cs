using System.Collections.Generic;
using UnityEngine;

public class TalkUI : MonoBehaviour
{
    //提示预制体
    public TalkUIItem prefabs;
    //提示物体父物体
    public Transform parent;

    private List<TalkUIItem> headTipsItems = new();

    void OnEnable() { 
        EventHandler.ShowTalkEvent += OnShowTipsEvent;
        EventHandler.DisShowTalkEvent += OnDisShowTipsEvent;
    }
    void OnDisable() {
        EventHandler.ShowTalkEvent -= OnShowTipsEvent;
        EventHandler.DisShowTalkEvent -= OnDisShowTipsEvent;
     }

    

    private void OnShowTipsEvent(TipsData data)
    {
        //Debug.Log("id是"+data.belongID);
        if (!headTipsItems.Find(item=>item.selfData.belongID == data.belongID))//此时还未生成提示
        {
            var tipsItem = Instantiate(prefabs,parent);
            tipsItem.Init(data);
            headTipsItems.Add(tipsItem);
        }
    }

    private void OnDisShowTipsEvent(string gameobjectID)
    {
        //Debug.Log("清除id是"+gameobjectID);
        var tipsItem = headTipsItems.Find(item=>item.selfData.belongID == gameobjectID);
        if (tipsItem)
        {
            headTipsItems.Remove(tipsItem);
            Destroy(tipsItem.gameObject);
        }
    }
}
