using TMPro;
using UnityEngine;
using DG.Tweening;

public class ActionItem : MonoBehaviour
{
    public TMP_Text actionMsg_Text;
    public GameObject loadingIcon;
    //完成Icon
    public GameObject completehIcon;
    public GameObject arrowhIcon;

    public void SetActionItem(string actionName)
    {
        actionMsg_Text.text = actionName;
        completehIcon.SetActive(false);
        loadingIcon.SetActive(false);
        arrowhIcon.SetActive(false);
    }

    public void SetLoadingIcon(bool isShow)
    {
        loadingIcon.SetActive(isShow);
        completehIcon.SetActive(!isShow);
        if (isShow)
        {
            loadingIcon.transform.DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360).SetLoops(-1);
        }
    }

    public void SetCompleteIcon(bool isShow)
    {
        completehIcon.SetActive(isShow);
        loadingIcon.SetActive(!isShow);
    }

    public void SetArrowIcon(bool isShow)
    {
        arrowhIcon.SetActive(isShow);
    }
    
}
