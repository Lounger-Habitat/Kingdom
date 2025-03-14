using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.IO;

public class AgentController : MonoBehaviour
{
    [SerializeField]
    private AgentMove agentMove;
    [SerializeField]
    private AgentDialogue agentDialogue;
    [SerializeField]
    private InventoryBag_SO agentBagData;
    public bool isGround = true;
    public string testJson = @"{
    ""Actions"": [
        {
            ""ActionName"": ""Move"",
            ""Params"": [
                ""Env-Cube 0""
            ]
        },
        {
            ""ActionName"": ""Move"",
            ""Params"": [
                ""Env-Cube 2""
            ]
        },
        {
            ""ActionName"": ""Move"",
            ""Params"": [
                ""Env-Cube 1""
            ]
        },
        {
            ""ActionName"": ""Move"",
            ""Params"": [
                ""Env-Cube 3""
            ]
        },
        {
            ""ActionName"": ""Jump"",
            ""Params"": [
                ""10""
            ]
        }
    ]";

    string result = @"[{""ActionName"":""Move"",""Params"":""中心广场""},  {""ActionName"":""Move"",""Params"":""书店""},  {""ActionName"":""Move"",""Params"":""武器店""},  {""ActionName"":""Move"",""Params"":""珠宝店""}]";

    //string userPrompt = @"你刚到此地，还不熟悉环境，根据地图有【铁匠铺、肉摊、书店、海鲜店、服装店、武器店、珠宝店、中心广场、城门口、一号农田、马场】，你打算随便逛一逛，计划出你要去的地方。可执行指令有：1.Move(target):移动到target位置，target为地图中地点名字。遵循json格式,仅回复json格式，例如： [{ActionName:Move,Params:铁匠铺},...]";
    //string userPromptChat="";
    string resData = "";

    private bool isRuning;
    void Update()
    {

    }

    //从本地文件读取提示词
    JArray GetUserPromptFromSA(){
        string filePath = Application.streamingAssetsPath + "/NPCPrompt.json";
        string content = File.ReadAllText(filePath);
        JArray jArray = JArray.Parse(content);
        return jArray;
    }

    // [ContextMenu("TwoRoleChat")]
    // public void ChatWithOther()
    // {
    //     AgentDialogueManager.Instance.TowRoleplayChat(name,"SM_Chr_Bartender_01","");
    // }

    void Start()
    {
        actionIndex = 0;
        //人物加载完成，开始加载要干的事情，把自己人物信息全部加载进来
        //根据当前信息请求模型，让模型告诉我该干什么
        var userPrompt =GetUserPromptFromSA();
        //Debug.Log(userPrompt[0].ToString());
        NetHttpManager.Instance.POST("chat", userPrompt[0].ToString(), (res) =>
        {
            JObject jobject = JObject.Parse(res);
            //Debug.Log(jobject["res"].ToString());
            List<ActionWithParamsType2> actionList = JsonConvert.DeserializeObject<List<ActionWithParamsType2>>(jobject["result"].ToString());
            //Debug.Log(actionList[0].ActionName+actionList[0].Params);
            //agentMove.MoveToTaregt(actionList[0].Params);
            RecordActionList(actionList);
            StartCoroutine(ExecuteTasks(actionList));
        });
    }
    public int actionIndex = 100000;
    public int statusIndex = 200000;
    //记录返回指令列表
    void RecordActionList(List<ActionWithParamsType2> list)
    {
        foreach (var item in list)
        {
            actionIndex++;
            switch (item.ActionName)
            {
                case "Move":
                    item.actionIndex = actionIndex;
                    EventHandler.CallAddActionEvent(actionIndex.ToString(), $"移动到{item.Params}");
                    break;
                case "Chat":
                    item.actionIndex = actionIndex;
                    EventHandler.CallAddActionEvent(actionIndex.ToString(), $"找到{item.Params}交谈");
                    break;
                case "Plant":
                    break;
                case "BuyTitle":
                    break;
                case "Trade":
                    break;
                case "Give":
                    break;
            }
        }
    }
    public void LoadAgentAction()
    {
        // 从文件中读取agent的行为
        string json = System.IO.File.ReadAllText("MessageQueue/AgentAction.json");
        ActionList actionList = JsonConvert.DeserializeObject<ActionList>(json);
        //StartCoroutine(ExecuteTasks(actionList));
    }

    public void TestTodoList()
    {
        // 解析json
        ActionList actionList = JsonConvert.DeserializeObject<ActionList>(testJson);
        //StartCoroutine(ExecuteTasks(actionList));
    }

    IEnumerator ExecuteTasks(List<ActionWithParamsType2> actionList)
    {
        isRuning = true;//开始执行指令
        foreach (var action in actionList)
        {
            Debug.Log($"开始执行任务{action.ActionName}");
            yield return StartCoroutine(ExecuteSingleTask(action));
            Debug.Log($"任务{action.ActionName}执行完毕");
        }
        statusIndex++;
        EventHandler.CallAddStatusEvent(statusIndex.ToString(), $"当前所哟指令执行完毕，等待下一轮指令");
        isRuning = false;//指令执行完成
    }
    IEnumerator ExecuteSingleTask(ActionWithParamsType2 action)
    {
        switch (action.ActionName)
        {
            case "Move":
                EventHandler.CallRunActionEvent(action.actionIndex.ToString());
                statusIndex++;
                EventHandler.CallAddStatusEvent(statusIndex.ToString(), $"正在向{action.Params}移动");
                agentMove.MoveToTaregt(action.Params);
                yield return new WaitUntil(() => !agentMove.isMove);
                EventHandler.CallCompleteActionEvent(action.actionIndex.ToString());
                statusIndex++;
                EventHandler.CallAddStatusEvent(statusIndex.ToString(), $"到达{action.Params}位置");
                break;
            case "Chat":
                Debug.Log("对话了，我的宝贝");
                EventHandler.CallRunActionEvent(action.actionIndex.ToString());
                statusIndex++;
                EventHandler.CallAddStatusEvent(statusIndex.ToString(), $"向{action.Params}发起对话");
                statusIndex++;
                EventHandler.CallAddStatusEvent(statusIndex.ToString(), "");
                AgentDialogueManager.Instance.TowRoleplayChat(name,action.Params,"",statusIndex.ToString());
                yield return new WaitUntil(()=>AgentDialogueManager.Instance.twoRoleplayChatComplete>=2);
                EventHandler.CallCompleteActionEvent(action.actionIndex.ToString());
                statusIndex++;
                EventHandler.CallAddStatusEvent(statusIndex.ToString(), $"与{action.Params}对话完成");
                break;
            case "Plant"://执行种地指令
                Debug.Log("种地了，我的宝贝");
                //先检查当前位置是否在种植地附近
                GridMapManager.Instance.CheckPlantPos(transform.position,action.Params,out var isPlantPos);//TODO:第二个参数为种植地块ID，目前还未大模型还未实时调整
                if (!isPlantPos.Equals(Vector3.zero))
                {
                    //还未到指定位置，先将角色移动到指定位置
                    agentMove.MoveToTaregt(isPlantPos);
                    yield return new WaitUntil(() => !agentMove.isMove);
                }
                yield return GridMapManager.Instance.PlantSeed(-1,action.Params);//TODO:此种植方法不正确，需要修改，第一个参数为种子ID，第二个参数为种植地块ID
                break;
            case"BuyTitle"://购买种植地
                //先检查当前是否能购买土地
                CheckPosDistance("神秘老头",out var targetPos);
                if (!targetPos.Equals(Vector3.zero))
                {
                    //还未到指定位置，先将角色移动到指定位置
                    agentMove.MoveToTaregt(targetPos);
                    yield return new WaitUntil(() => !agentMove.isMove);
                }
                CheckMoneyBuyTitle();
                yield return new WaitForSeconds(1.0f);
                break;
            case "Trade"://同其他角色交易
                //跟其他角色交易，需要清楚其他角色是谁，需要交易的物品是什么
                //设定虚拟参数:交易对象(other)，交易物品，交易数量
                RoleTrading(action.Params);
                yield return new WaitForSeconds(2.0f);//交易完成后等一下，防止刚过去就完成，不好看
                break;
            case "Give"://给予物品，直接将某物交给对方
                GiveItemToOther(action.Params);
                yield return new WaitForSeconds(2.0f);//太快完成动作不好看
                break;
            default:
                break;
        }
        Debug.Log($"任务{action.ActionName}执行完毕");
    }

    /// <summary>
    /// 检查当前位置是否在指定位置附近
    /// </summary>
    /// <param name="enviromentName">环境名称，可从环境管理查找到具体位置</param>
    /// <param name="targetPos">如果不在附近，会返回目标位置</param>
    public void CheckPosDistance(string enviromentName,out Vector3 targetPos)
    {
        targetPos = EnvironmentManager.Instance.TargetTransform(enviromentName).position;
        if (Vector3.Distance(transform.position, targetPos) < 0.5f)
        {
            targetPos = Vector3.zero;
        }
    }

    /// <summary>
    /// 从神秘老头购买一块种植地
    /// </summary>
    public void CheckMoneyBuyTitle()
    {
        if (agentBagData.money>=50)
        {
            agentBagData.money -=50;
            GridMapManager.Instance.OpenCropTile();//开启一块地
            EventHandler.CallShowTextTipsEvent("开垦了一块田地");
        }else{
            EventHandler.CallShowTextTipsEvent("钱不够，搞点钱去吧");
        } 
    }

    /// <summary>
    /// 角色之间交易
    /// </summary>
    /// <param name="paramsStr">json格式数据,具体格式见文件</param>
    public void RoleTrading(string paramsStr){
        //首先交易是双方背包物品交换，所以需要传入双方背包
        //agentBagData 是发起交易角色背包，
        JObject jObject = JObject.Parse(paramsStr);
        string otherName = jObject["otherName"].ToString();//交易对象
        var otherBagData = AgentGroup.Instance.GetAgentController(otherName).GetComponent<NPCFuction>().shopData;//对方背包
        //检查双方背包是否有交易物品，并且数量是否符合要求//TODO：目前全部默认都符合要求
        //将我要交给对方的物品从我背包中移除，添加到对方背包中。物品可能是多个
        JArray tradeItems = JArray.Parse(jObject["mytradeItems"].ToString());
        foreach (var item in tradeItems)
        {
            var itemData = item.ToString().Split('*');
            int id =int.Parse(itemData[0]);
            int count = int.Parse(itemData[1]);
            InventoryManager.Instance.RemoveItem(id,count,agentBagData);//把我背包物品移除
            InventoryManager.Instance.AddItemAtBag(otherBagData,id,count);//加入对方背包
        }
        //如果我要交易的金币大于0
        int myTradeMoney = jObject["mytradeMoney"].ToObject<int>();
        if (myTradeMoney>0)
        {
            agentBagData.money -= myTradeMoney;
            otherBagData.money += myTradeMoney;
        }
        //将对方要交给我的物品从对方背包中移除，添加到我的背包中。物品可能是多个
        JArray otherTradeItems = JArray.Parse(jObject["othertradeItems"].ToString());
        foreach (var item in otherTradeItems)
        {
            var itemData = item.ToString().Split('*');
            int id =int.Parse(itemData[0]);
            int count = int.Parse(itemData[1]);
            InventoryManager.Instance.RemoveItem(id,count,otherBagData);//把对方背包物品移除
            InventoryManager.Instance.AddItemAtBag(agentBagData,id,count);//加入我背包
        }
        //如果对方要交易的金币大于0
        int otherTradeMoney = jObject["othertradeMoney"].ToObject<int>();
        if (otherTradeMoney > 0)
        {
            agentBagData.money += otherTradeMoney;
            otherBagData.money -= otherTradeMoney;
        }
        //TODO:更新背包UI
        Debug.Log("交易完成,后续需要将本消息传递到监视面板中");
    }
    /// <summary>
    /// 给与其他角色物品
    /// </summary>
    /// <param name="paramsStr">json格式数据,具体格式见文件</param>
    public void GiveItemToOther(string paramsStr){
        //首先交易是双方背包物品交换，所以需要传入双方背包
        //agentBagData 是发起交易角色背包，
        JObject jObject = JObject.Parse(paramsStr);
        string otherName = jObject["otherName"].ToString();//交易对象
        var otherBagData = AgentGroup.Instance.GetAgentController(otherName).GetComponent<NPCFuction>().shopData;//对方背包
        //将我要交给对方的物品从我背包中移除，添加到对方背包中。物品可能是多个,需要检测我背包里有这些东西吗
        JArray Items = JArray.Parse(jObject["Items"].ToString());
        foreach (var item in Items)
        {
            var itemData = item.ToString().Split('*');
            int id =int.Parse(itemData[0]);
            int count = int.Parse(itemData[1]);
            InventoryManager.Instance.RemoveItem(id,count,agentBagData);//把我背包物品移除
            InventoryManager.Instance.AddItemAtBag(otherBagData,id,count);//加入对方背包
        }
        //如果我要交易的金币大于0
        int myTradeMoney = jObject["mytradeMoney"].ToObject<int>();
        if (myTradeMoney>0)
        {
            agentBagData.money -= myTradeMoney;
            otherBagData.money += myTradeMoney;
        }
}
}
[Serializable]
public class ActionWithParamsType2
{
    public string ActionName;
    public string Params;
    public  int actionIndex;//表示此条指令的唯一标识
}