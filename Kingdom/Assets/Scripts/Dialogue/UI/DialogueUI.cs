using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueBox;

    public Text dialogueText;

    public Image faceRight, faceLeft;
    public Text nameRight, nameLeft;

    public GameObject continueBG;

    void Awake()
    {
        //dialogueBox.SetActive(false);
        continueBG.SetActive(false);
    }
    void Start()
    {
        dialogueBox.SetActive(false);
    }
    void OnEnable()
    {
        EventHandler.ShowDialogueEvent += OnShowDialogueEvent;
    }


    void OnDisable()
    {
        EventHandler.ShowDialogueEvent -= OnShowDialogueEvent;
    }

    private void OnShowDialogueEvent(DialoguePiece piece)
    {
        StartCoroutine(ShowDialogue(piece));
    }

    private IEnumerator ShowDialogue(DialoguePiece piece)
    {

        if (piece != null)
        {
            piece.isDone = false;

            dialogueBox.SetActive(true);
            continueBG.SetActive(false);

            dialogueText.text = string.Empty;

            if (piece.onLeft)
            {
                faceRight.gameObject.SetActive(false);
                faceLeft.gameObject.SetActive(true);
                //TODO:图标与名字添加
                nameLeft.text = piece.name;
            }
            else
            {
                faceRight.gameObject.SetActive(true);
                faceLeft.gameObject.SetActive(false);
                nameRight.text = piece.name;
            }
            yield return dialogueText.DOText(piece.dialogueText, 1.5f).WaitForCompletion();

            piece.isDone = true;

            if (piece.hasToPause && piece.isDone)
            {
                continueBG.SetActive(true);
            }
            else
            {
                continueBG.SetActive(false);
            }
        }
        else
        {
            dialogueBox.SetActive(false);
            yield break;
        }
    }
}
