using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class RightSelectUI : MonoBehaviour
{
    public GameObject prefab;
    public Transform tipsParent;

    private List<GameObject> tipsList = new();
    void OnEnable()
    {
        EventHandler.OnShowSelectTipsEvent += OnShowSelectTipsEvent;
        EventHandler.DisShowSelectTipsEvent += DisShowSelectTipsEvent;
        EventHandler.DisShowSelectAllTipsEvent += DisShowAllSelectTipsEvent;
    }

    void OnDisable()
    {
        EventHandler.OnShowSelectTipsEvent -= OnShowSelectTipsEvent;
        EventHandler.DisShowSelectTipsEvent -= DisShowSelectTipsEvent;
        EventHandler.DisShowSelectAllTipsEvent -= DisShowAllSelectTipsEvent;
    }

    private void DisShowAllSelectTipsEvent()
    {
        foreach (var item in tipsList)
        {
            Destroy(item);
        }
        tipsList.Clear();
    }

    private void DisShowSelectTipsEvent(string tipsID)
    {
        var obj = tipsList.Find(item => item.name == tipsID);
        if (obj)
        {
            tipsList.Remove(obj);
            Destroy(obj);
        }
    }

    private void OnShowSelectTipsEvent(string tipsID,string btnMsg,UnityAction btnAction)
    {
        var obj = Instantiate(prefab, tipsParent);
        obj.name = tipsID;
        obj.transform.Find("Text (TMP)").GetComponent<TMP_Text>().text = btnMsg;
        obj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(btnAction);
        tipsList.Add(obj);
    }
}
