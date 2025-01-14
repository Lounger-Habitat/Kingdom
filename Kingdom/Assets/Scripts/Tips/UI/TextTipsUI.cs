using Unity.VisualScripting;
using UnityEngine;

public class TextTipsUI : MonoBehaviour
{
    public TextTipsItem prefab;

    public Transform parentTransform;


    void OnEnable()
    {
        EventHandler.ShowTextTipsEvent += OnShowTextTipsEvent;
    }

    void OnDisable()
    {
        EventHandler.ShowTextTipsEvent -= OnShowTextTipsEvent;
    }

    private void OnShowTextTipsEvent(string msg)
    {
        var tips = Instantiate(prefab,parentTransform);
        tips.OnSetUpItem(msg);
    }
}
