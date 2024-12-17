using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemID;

    private ItemDetails itemDetails;

    private void Start()
    {
        if (itemID!=0)
        {
            Init(itemID);
        }
    }

    public void Init(int ID)
    {
        itemID = ID;

        itemDetails = InventoryManager.Instance.GetItemDetails(itemID);
        if (itemDetails!=null)
        {
            //生成资源物体
            var go = Instantiate(itemDetails.itemPrefab, transform);
            go.transform.localPosition = Vector3.zero;
        }
    }
}
