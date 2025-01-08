using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage;

    public RectTransform clockParent;

    public Image seasonImage;

    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;

    public Sprite[] seasonSprites;

    private List<GameObject> clockBlocks = new();

    void Awake()
    {
        for (int i = 0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.GameDateEvent += OnGameDateEvent;
    }
    void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.GameDateEvent -= OnGameDateEvent;
    }

    private void OnGameMinuteEvent(int minute, int hour)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }

    private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        dateText.text = year + "年" + month.ToString("00") + "月" + day.ToString("00") + "日";
        seasonImage.sprite = seasonSprites[(int)season];
        SwitchHourImage(hour);
        DayNightImageRatate(hour);
    }

    private void SwitchHourImage(int hour)
    {
        int index = hour / 4;

        for (int i = 0; i < clockBlocks.Count; i++)
        {
            if (i <= index)
            {
                clockBlocks[i].SetActive(true);
            }
            else
            {
                clockBlocks[i].SetActive(false);
            }
        }

    }

    private void DayNightImageRatate(int hour)
    {
        var target = new Vector3(0, 0, hour  * 15 - 90);
        dayNightImage.DORotate(target, 1.0f, RotateMode.Fast);
    }
}
