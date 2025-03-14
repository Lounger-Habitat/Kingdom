using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class InspectorPanel : MonoBehaviour
{
    void OnEnable()
    {
        EventHandler.AddActionEvent += AddAction;
        EventHandler.RunActionEvent += SetActionLoading;
        EventHandler.CompleteActionEvent += SetActionComplete;
        EventHandler.AddStatusEvent += AddStatus;
        EventHandler.UpdateStatusEvent += UpdateStatus;
    }

    void OnDisable()
    {
        EventHandler.AddActionEvent -= AddAction;
        EventHandler.RunActionEvent -= SetActionLoading;
        EventHandler.CompleteActionEvent -= SetActionComplete;
        EventHandler.AddStatusEvent -= AddStatus;
        EventHandler.UpdateStatusEvent -= UpdateStatus;
    }
    #region  行动相关
    public RectTransform actionListContent;
    public ActionItem actionPrefab;

    private List<ActionItem> actionItems = new List<ActionItem>();

    public void AddAction(string actionID,string actionName)
    {
        var actionObject = Instantiate(actionPrefab, actionListContent);
        actionObject.gameObject.SetActive(true);
        actionObject.name = actionID;
        if (actionItems.Count > 0)
        {   //获取actionItems最后一个元素
            actionItems[actionItems.Count - 1].SetArrowIcon(true);
        }
        actionObject.SetActionItem(actionName);
        actionItems.Add(actionObject);

    }

    ActionItem GetActionItem(string actionID)
    {
        return actionItems.Find(actionItem =>  actionItem.name == actionID);
    }

    public void SetActionLoading(string actionID)
    {
        var actionItem = GetActionItem(actionID);
        if (actionItem != null)
        {
            actionItem.SetLoadingIcon(true);
        }
        //获取actionItem在列表中的索引
        int index = actionItems.FindIndex(a => actionItem ==a);
        if (index>=4)
        {
            float y = (index - 1.5f) * 74;
            actionListContent.DOAnchorPosY(y, 1f);
        }
    }

    public void SetActionComplete(string actionID)
    {
        var actionItem = GetActionItem(actionID);
        if (actionItem != null)
        {
            actionItem.SetCompleteIcon(true);
        }
    }
    #endregion 
    #region 实时动态相关
    public RectTransform statusListContent;
    public TMP_Text statusPrefab;

    private List<TMP_Text> statusItems = new List<TMP_Text>();
    string[] colors = new string[] { "#0000FF", "#008000", "#FFD700", "#800080", "#00FFFF", "#FF00FF" };
    public void AddStatus(string statusID, string statusmsg)
    {
        var statusObject = Instantiate(statusPrefab, statusListContent);
        statusObject.gameObject.SetActive(true);
        statusObject.name = statusID;
        //获取当前时间小时分钟
        statusObject.text =$"<color={colors[Random.Range(0,6)]}>[{System.DateTime.Now:HH:mm}]</color> {statusmsg}";
        statusItems.Add(statusObject);
    }

    public void UpdateStatus(string statusID, string statusmsg)
    {
        var statusItem = statusItems.Find(status => status.name == statusID);
        if (statusItem != null)
        {
            string colorText = statusItem.text.Split("</color>")[0]+"</color> ";
            statusItem.text  += statusmsg;
        }
    }
    #endregion
}
