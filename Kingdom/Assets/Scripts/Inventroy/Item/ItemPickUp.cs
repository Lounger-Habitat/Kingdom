using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);
        Item item = other.transform.parent.GetComponent<Item>();
        if (item!=null)
        {
            InventoryManager.Instance.AddItem(item,false);
        }
    }
}
