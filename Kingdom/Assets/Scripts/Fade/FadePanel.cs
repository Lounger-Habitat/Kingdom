using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class FadePanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    void OnEnable()
    {
        EventHandler.FadePanelEvent += OnFadePanelEvent;
    }
    void OnDisable()
    {
        EventHandler.FadePanelEvent += OnFadePanelEvent;
    }

    private void OnFadePanelEvent(float value)
    {
        canvasGroup.DOFade(value,1.5f);
    }
}
