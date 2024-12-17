using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> updateInventoryUI;

    public static void CallUpdateInventoryUI(InventoryLocation location,List<InventoryItem> list)
    {
        updateInventoryUI?.Invoke(location,list);
    }
}
