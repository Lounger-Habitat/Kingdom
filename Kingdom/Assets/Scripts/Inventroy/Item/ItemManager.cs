using Unity.VisualScripting;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public Item itemPrefab;

    private Transform itemParent;

    private void OnEnable()
    {
        EventHandler.instantiateItemInScene += OnInstantiateItemInScene;
    }

    void OnDisable()
    {
        EventHandler.instantiateItemInScene += OnInstantiateItemInScene;
    }

    void Start()
    {
        itemParent = transform;//目前先把自己当作父节点
    }

    private void OnInstantiateItemInScene(int ID, Vector3 pos)
    {
        var item = Instantiate(itemPrefab, pos, Quaternion.identity, itemParent);
        item.itemID = ID;
    }

}
