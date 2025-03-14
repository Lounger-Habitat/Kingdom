using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class AgentDialogue : MonoBehaviour
{
    //当前角色是否在对话
    public bool isTalking = false;
    //当前角色是否在聆听
    //public bool isListening = false;
    //那个角色在说话
    public AgentDialogue otherAgent = null;
    //当前角色的对话内容
    public Queue<string> dialogueStack = new Queue<string>();
    public List<string> messages;

    public string statusID;
    void Start()
    {
        dialogueStack.Clear();
        foreach (var item in messages)
        {
            dialogueStack.Enqueue(item);
        }
    }

    JArray GetUserPromptFromSA(){
        string filePath = Application.streamingAssetsPath + "/NPCPrompt.json";
        string content = File.ReadAllText(filePath);
        JArray jArray = JArray.Parse(content);
        return jArray;
    }

    private string currentTalkStr;
    private int talkStrIndex=0;
    [ContextMenu("TestTalk")]
    public void TestTalk(){
        //StartTalk(otherAgent);
        var userPrompt =GetUserPromptFromSA();
        currentTalkStr=string.Empty;
        talkStrIndex=0;
        talkMessage.Clear();
        StartCoroutine(ShowTalkMessage());
        NetHttpManager.Instance.ChatWithConnectSSE("http://localhost:8000/test/multi",userPrompt[2].ToString(),TalkCallBack);
    }
    public void StartShowTalkMessage(AgentDialogue other,bool firstTalk,string sID){
        otherAgent = other;
        waitOtherTalk = !firstTalk;
        currentTalkStr=string.Empty;
        talkStrIndex=0;
        talkMessage.Clear();
        statusID =sID;
        StartCoroutine(ShowTalkMessage());
    }
    private List<string> talkMessage = new List<string>(); 
    public void TalkCallBack(string message)
    {
        //Debug.Log(message);
        JObject jObject = JObject.Parse(message);//解析成json
        if (jObject.ContainsKey("data"))
        {
            currentTalkStr += jObject["data"];
            talkMessage[talkStrIndex] = currentTalkStr;
        }
        if (jObject.ContainsKey("reasoning_data")){
            // TODO
            // 如果有 推理数据 在这里处理
        }
        if (jObject.ContainsKey("event"))//如果是event
        {
            if (jObject["event"].ToString().Contains("start:assistant"))//开始对话
            {
                
                //清除当前对话str
                currentTalkStr = string.Empty;
                talkMessage.Add(currentTalkStr);
            }
            if (jObject["event"].ToString().Contains("stop:end_turn"))
            {
                currentTalkStr+="[NEXT]";
                talkMessage[talkStrIndex] = currentTalkStr;
                Debug.Log($"这句话结束：{currentTalkStr}");
                talkStrIndex++;
            }
            if (jObject["event"].ToString().Contains("stop:end_SSE"))
            {
                talkMessage.Add("[STOP]");
            }
        }
        
    }

    public bool messageBool = true;
    public bool waitOtherTalk = true;
    IEnumerator ShowTalkMessage(){
        int currentIndex =0;
        string currentStr ="";
        int check=0;
        while (messageBool)
        {
            if (waitOtherTalk)
            {
                yield return new WaitForSeconds(0.5f);//现在是对方说话等待对方说完
                continue;
            }
            if (talkMessage.Count<=currentIndex)
            {
                yield return new WaitForSeconds(0.5f);
                check++;
                if (check>50)
                {
                    break;
                }
                continue;
            }
            check=0;
            var str = talkMessage[currentIndex];
            if (str.Contains("[STOP]"))
            {
                //全都说完了，撤退
                Debug.Log("全都说完了");
                break;
            }
            bool isEND = false;
            if (str.Contains("END"))
            {
                //全都说完了，撤退
                Debug.Log("全都说完了");
                str = str.Replace("[END]","");
                isEND = true;
            }
            bool talkNext = false;
            if (str.Contains("[NEXT]"))//表示这句完了
            {
                str = str.Replace("[NEXT]","");
                talkNext = true;
            }
            // int count = str.Length - currentStr.Length;
            for (int i = currentStr.Length; i < str.Length; i++)
            {
                currentStr+=str[i];
                EventHandler.CallUpdateStatusEvent(statusID.ToString(),str[i].ToString());
                EventHandler.CallShowTalkEvent(new TipsData(currentStr,transform.position + Vector3.up * 2.2f,$"{name}talkID",transform));
                yield return new WaitForSeconds(0.07f);
            }
            if (talkNext)//这句话完事了，停顿一下再说下一句话
            {
                //Debug.Log();
                currentIndex++;
                currentStr ="";
                EventHandler.CallUpdateStatusEvent(statusID.ToString(),"\n");
                Debug.Log("等着说下一句");
                yield return new WaitForSeconds(1.5f);
                EventHandler.CallDisShowTalkEvent($"{name}talkID");
                waitOtherTalk = true;
                otherAgent.waitOtherTalk = false;
            }
            if (isEND)
            {
                yield return new WaitForSeconds(1.5f);
                break;
            }
            yield return new WaitForSeconds(0.2f);
        }
        yield return new WaitForSeconds(0.2f);
        EventHandler.CallDisShowTalkEvent($"{name}talkID");//关闭对话窗口
        AgentDialogueManager.Instance.twoRoleplayChatComplete++;
    }

    //当前角色开始说话，其他角色开始聆听
    //首先通知聆听角色，我要开始跟你说话了
    //开启说话方法，逐句输出对话内容，并且发送给对方（后续可能是同时把所有对话请求到，不需要逐句发送）
    //每句话说完，通知对方，我话讲完了，你可以继续说了，如果对方讲完，我没有内容，通知对方我结束了

    public void StartTalk(AgentDialogue otherAgent){
        //检测当前能否对话，对方是否正在对话
        if (isTalking || otherAgent.isTalking)
        {
            return;
        }
        //通知对方聆听
        //开始说话
        isTalking = true;
        otherAgent.isTalking = true;
        //isListening = false;
        //逐句输出对话内容
        //发送给对方
        //每句话结束，通知对方，我话讲完了，你可以继续说了，如果对方讲完，我没有内容，通知对方我结束了
        StartCoroutine(Talk(otherAgent));
    }

    public IEnumerator Talk(AgentDialogue otherAgent){
        //逐句输出对话内容
        //发送给对方
        var talkMessage = dialogueStack.Dequeue();
        //通知UI显示
        EventHandler.CallShowTalkEvent(new TipsData(talkMessage,transform.position + Vector3.up * 2.2f,$"{name}talkID"));
        //每句话说固定时间
        yield return new WaitForSeconds(Settings.speakTime);
        EventHandler.CallDisShowTalkEvent($"{name}talkID");
        yield return new WaitForSeconds(0.56f);
        //告诉对方我说的话
        otherAgent.ReciveTalk(this, talkMessage);
        yield return null;
    }

    public void ReciveTalk(AgentDialogue otherAgent,string message)
    {
        //接受对话信息
        Debug.Log($"【{otherAgent.name}】对【{name}】说:{message}");
        //检查自己是否还有话，有就跟对面说
        if (dialogueStack.Count > 0)
        {
           StartCoroutine(Talk(otherAgent));
        }else{
            //没有话了，通知对方我结束了
            //isTalking = false;
            //isListening = false;
            otherAgent.EndTalk(this);
        }
    }

    public void EndTalk(AgentDialogue otherAgent){
       //调用这个，说明对方说完了
       //检查自己是否还有话，有就跟对面说，没有就算了
        if (dialogueStack.Count > 0)
        {
            StartCoroutine(Talk(otherAgent));
        }else{
            //没有话了,我也没话了
            isTalking = false;
            otherAgent.isTalking = false;
        }
    }
}
