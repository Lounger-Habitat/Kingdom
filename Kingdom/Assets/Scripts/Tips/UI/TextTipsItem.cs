using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class TextTipsItem : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TMP_Text tMP_Text;
    public void OnSetUpItem(string msg){
        tMP_Text.text = msg;
        StartCoroutine(ShowTextTips());
    }

    IEnumerator  ShowTextTips(){

        yield return new WaitForSeconds(1.5f);
        canvasGroup.DOFade(0,1f);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
