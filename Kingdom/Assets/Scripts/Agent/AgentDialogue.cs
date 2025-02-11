using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentDialogue : MonoBehaviour
{
    //当前角色是否在对话
    public bool isTalking = false;
    //当前角色是否在聆听
    //public bool isListening = false;
    //那个角色在说话
    public AgentDialogue otherAgent = null;
    //当前角色的对话内容栈
    public Queue<string> dialogueStack = new Queue<string>();
    public List<string> messages;

    void Start()
    {
        dialogueStack.Clear();
        foreach (var item in messages)
        {
            dialogueStack.Enqueue(item);
        }
    }

    [ContextMenu("TestTalk")]
    public void TestTalk(){
        StartTalk(otherAgent);
    }

    //当前角色开始说话，其他角色开始聆听
    //首先通知聆听角色，我要开始跟你说话了
    //开启说话方法，逐句输出对话内容，并且发送给对方（后续可能是同时把所有对话请求到，不需要逐句发送）
    //每句话说完，通知对方，我话讲完了，你可以继续说了，如果对方讲完，我没有内容，通知对方我结束了

    public void StartTalk(AgentDialogue otherAgent){
        //通知对方聆听
        //otherAgent.isListening = true;
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
        Debug.Log($"{otherAgent.name}对方说:{message}");
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
