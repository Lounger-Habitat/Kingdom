using UnityEngine;

public class NPCFuction : MonoBehaviour
{
    public InventoryBag_SO shopData;

    private bool isOpen;

    void Update(){
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseShop();
        }
    }

    public void OpenShop()
    {   
        isOpen = true;
        EventHandler.CallBaseBagOpenEvent(SlotType.Shop,shopData);
    }

    public void CloseShop(){
        isOpen = false;
        EventHandler.CallBaseBagCloseEvent(SlotType.Shop);
    }
}
