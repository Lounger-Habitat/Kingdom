using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CropTipsUI : MonoBehaviour
{
    public GameObject prefab;
    public Transform tipsParent;

    private List<GameObject> tipsList = new();
    void OnEnable()
    {
        EventHandler.ShowCropTipsEvent += OnShowCropTipsEvent;
        EventHandler.DisShowCropTipsEvent += DisOnShowCropTipsEvent;
    }

    void OnDisable()
    {
        EventHandler.ShowCropTipsEvent -= OnShowCropTipsEvent;
        EventHandler.DisShowCropTipsEvent -= DisOnShowCropTipsEvent;
    }

    private void DisOnShowCropTipsEvent(string tipsID)
    {
        var obj = tipsList.Find(item => item.name == tipsID);
        if (obj)
        {
            tipsList.Remove(obj);
            Destroy(obj);
        }
    }

    private void OnShowCropTipsEvent(string tipsID)
    {
        var obj = Instantiate(prefab,tipsParent);
        obj.name = tipsID;
        tipsList.Add(obj);
    }


}
