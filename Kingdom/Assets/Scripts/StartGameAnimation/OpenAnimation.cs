using System.Collections;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class OpenAnimation : MonoBehaviour
{
    //管理简单的开场动画
    [SerializeField]
    CinemachineSplineDolly cinemachineSplineDolly;
    public Text tipsText;
    //开场简单对话提示
    public DialoguePiece dialoguePiece;

    void Awake()
    {
        cinemachineSplineDolly.CameraPosition = 0;
    }

    public void StartOpenAnimation(){
        StartCoroutine(OpenAnimation_tor());
    }

    IEnumerator OpenAnimation_tor()
    {
        yield return tipsText.DOText("平凡的城镇刚刚经历一场战争",2.5f).WaitForCompletion();
        EventHandler.CallFadePanelEvent(0);
        yield return DOTween.To(()=>cinemachineSplineDolly.CameraPosition,value=>cinemachineSplineDolly.CameraPosition=value,1f,25f).SetEase(Ease.Linear).WaitForCompletion();
        tipsText.text = string.Empty;
        EventHandler.CallFadePanelEvent(1);
        yield return new WaitForSeconds(1.5f);
        cinemachineSplineDolly.gameObject.SetActive(false);
        yield return tipsText.DOText("本想尽快离开这的你，意外听到这里可能有大赚一笔的机会",3.5f).WaitForCompletion();
        EventHandler.CallFadePanelEvent(0);
        yield return new WaitForSeconds(0.5f);
        EventHandler.CallShowDialogueEvent(dialoguePiece);
        yield return new WaitUntil(()=>dialoguePiece.isDone);
        isTalk = true;
        //EventHandler.call

    }

    private bool isTalk;
    void Update()
    {
        if(isTalk && Input.GetKeyDown(KeyCode.F)){
            EventHandler.CallShowDialogueEvent(null);
            isTalk = false;
        }
    }
}
