using UnityEngine;

public class TipsData
{
    public string tipsText;
    public Vector3 worldPos;

    public string belongID;//所属物体的ID，唯一性。在生成和隐藏时使用
    public TipsData(string tips,Vector3 pos,string ID){
        tipsText = tips;
        worldPos = pos;
        belongID = ID;
    }
}
