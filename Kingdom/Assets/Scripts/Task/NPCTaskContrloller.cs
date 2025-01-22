using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public class NPCTaskContrloller : MonoBehaviour
{
    //任务列表,一个NPC可能有多个任务
    public List<TaskData> taskDatas;
    private DialogueController dialogueController => GetComponent<DialogueController>();
    private NPCFuction npcFuction => GetComponent<NPCFuction>();
    void Start()
    {
        RefreshTask();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            RefreshTask();
        }
    }
    private void RefreshTask()
    {
        //bool isSet = false;
        currentDetail = null;
        foreach (var taskItem in taskDatas)
        {
            //首先根据任务ID查到任务
            var taskDetail = TaskManager.Instance.taskDataList_SO.GetTaskDetail(taskItem.taskID);
            //检查当前任务是否完成
            if (taskDetail.isDone)
            {

            }
            else
            {
                //没有完成，把当前任务挂载到NPC身上
                SetTaskDataToNPC(taskItem, taskDetail);
                //isSet = true;//表示本次已经挂载过任务
                break;
            }
        }

    }

    private void SetTaskDataToNPC(TaskData data, TaskDetail taskDetail)
    {
        currentDetail = taskDetail;
        if (taskDetail.received)
        {
            //任务被接取，那就只挂载任务对话里最后一条信息
            var piece = data.dialoguePieces.Last();
            var pieceList = new List<DialoguePiece>() { piece };
            dialogueController.SetPieceList(pieceList);
        }
        else
        {
            //任务没有被接取，挂载除了最后一条的信息
            var pieceList = new List<DialoguePiece>();
            for (int i = 0; i < data.dialoguePieces.Count-1; i++)
            {
                pieceList.Add(data.dialoguePieces[i]);
            }
            dialogueController.SetPieceList(pieceList);
        }
    }

    private TaskDetail currentDetail = null;
    //接取任务
    public void ReceiveTask()
    {   
        if (currentDetail==null) return;//没有任务
        if (!currentDetail.received)//任务没有被接取，直接自动接取任务
        {
            currentDetail.received = true;
            TaskManager.Instance.taskDataList_SO.ModifyTaskData(currentDetail);
            npcFuction.AddTaskToOther(currentDetail.ID);
            EventHandler.CallShowTextTipsEvent($"接取了{currentDetail.taskName}任务");
            RefreshTask();
        }else
        {
            //任务被接取，可以通知页面打开提交按钮
            EventHandler.CallOnShowSelectTipsEvent("TaskBtnID","提交物品",OpenSubmitPanel);//通知提交UI
        }
    }

    public void OpenSubmitPanel()
    {
        EventHandler.CallShowSubmitPanelEvent(npcFuction.otherNPCFunction.shopData,currentDetail);
        EventHandler.CallDisShowSelectAllTipsEvent();
    }

}
