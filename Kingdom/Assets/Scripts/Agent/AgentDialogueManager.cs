using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AgentDialogueManager : MonoBehaviour
{
    public List<AgentDialogue> agentDialogues = new();
    public static AgentDialogueManager Instance;

    public AgentDialogue[] chatAgents = new AgentDialogue[2];
    void Awake()
    {
        Instance = this;
    }
    JArray GetUserPromptFromSA()
    {
        string filePath = Application.streamingAssetsPath + "/NPCPrompt.json";
        string content = File.ReadAllText(filePath);
        JArray jArray = JArray.Parse(content);
        return jArray;
    }
    int index = -1;
    int currentIndex = 0;

    public int twoRoleplayChatComplete = 0;//当前对话完成标记，每人对话完成加一，两人都完成是二
    
    /// <summary>
    /// 描述
    /// </summary>
    /// <param name="aName"></param>
    /// <param name="bName"></param>
    /// <param name="topic"></param>
    /// <param name="statusID">实时状态对话ID唯一标识</param>
    public void TowRoleplayChat(string aName, string bName, string topic,string statusID)
    {
        index = -1;
        currentIndex = 0;
        var userPrompt = GetUserPromptFromSA();
        var request = userPrompt[3].ToString().Replace("nameOne", aName).Replace("nameTwo", bName);
        Debug.Log(request);
        chatAgents[0] = agentDialogues.Find(a => aName == a.name);
        chatAgents[1] = agentDialogues.Find(b => bName == b.name);
        twoRoleplayChatComplete = 0;
        chatAgents[0].StartShowTalkMessage(chatAgents[1], true,statusID);
        chatAgents[1].StartShowTalkMessage(chatAgents[0], false,statusID);
        NetHttpManager.Instance.ChatWithConnectSSE("http://localhost:8000/agent/tworoleplay", request, CallBack);
    }

    private void CallBack(string resMessage)
    {
        JObject jObject = JObject.Parse(resMessage);
        if (jObject.ContainsKey("event"))
        {
            if (jObject["event"].ToString().Contains("start:assistant"))//开始对话
            {
                index++;
                currentIndex = index % 2;
                //Debug.Log($"当前对话是第{currentIndex}个");
            }

        }
        chatAgents[currentIndex].TalkCallBack(resMessage);
    }
}
