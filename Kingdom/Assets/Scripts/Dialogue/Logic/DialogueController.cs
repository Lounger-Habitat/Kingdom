using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class DialogueController : MonoBehaviour
{
    public UnityEvent OnFinishEvent;

    public List<DialoguePiece> dialogueList = new();

    public bool autoTalk;
    private Stack<DialoguePiece> dialogueStack;

    private bool canTalk;
    private bool isTalking;
    //private GameObject uiSign;
    //[SerializeField]private Transform transformPos;
    private string gameobjectID;
    void Awake()
    {
        //uiSign = transform.GetChild(1).gameObject;
        FillDialogueStack();
        gameobjectID = GetInstanceID().ToString();
    }

    /// <summary>
    /// 构建对话栈，根据先进后出，应该从后开始压入栈
    /// </summary>
    void FillDialogueStack()
    {
        dialogueStack = new();
        for (int i = dialogueList.Count - 1; i > -1; i--)
        {
            dialogueList[i].isDone = false;
            dialogueStack.Push(dialogueList[i]);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTalk = true;
            Debug.Log("玩家进入");
            EventHandler.CallShowTipsEvent(new TipsData("按下<color=\"black\">空格</color>对话", transform.position + Vector3.up * 1.8f, gameobjectID));
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTalk = false;
            Debug.Log("玩家退出");
            EventHandler.CallDisShowTipsEvent(gameobjectID);
        }
    }

    void Update()
    {
        //uiSign.SetActive(canTalk);
        if (canTalk & Input.GetKeyDown(KeyCode.Space) && !isTalking)
        {
            StopAllCoroutines();
            StartCoroutine(DailogueRoutine());
        }
    }

    private IEnumerator DailogueRoutine()
    {
        isTalking = true;
        do
        {
            if (dialogueStack.TryPop(out DialoguePiece result))
            {
                //传递到UI显示
                EventHandler.CallShowDialogueEvent(result);
                yield return new WaitUntil(() => result.isDone);
                isTalking = false;
            }
            else
            {
                isTalking = false;
                EventHandler.CallShowDialogueEvent(null);
                FillDialogueStack();

                if (OnFinishEvent != null)
                {
                    OnFinishEvent.Invoke();
                    canTalk = false;
                }
                yield break;
            }
            if (autoTalk)
            {
                yield return new WaitForSeconds(1.5f);
            }

        } while (autoTalk && dialogueStack.Count > 0);

        if (autoTalk && dialogueStack.Count <= 0)
        {
            isTalking = false;
            EventHandler.CallShowDialogueEvent(null);
            FillDialogueStack();

            if (OnFinishEvent != null)
            {
                OnFinishEvent.Invoke(); 
                canTalk = false;
            }
        }

    }
}
