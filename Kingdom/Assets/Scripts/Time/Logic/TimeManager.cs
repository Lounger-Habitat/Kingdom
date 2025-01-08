using UnityEngine;
using UnityEngine.PlayerLoop;

public class TimeManager : MonoBehaviour
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season gameSeason = Season.春天;

    private int mounthInSeason = 3;

    //时间暂停
    public bool gameClockPause;

    private float tikTime;

    void Awake()
    {
        NewGameTime();
    }

    void Start()
    {
        EventHandler.CallGameMinuteEvent(gameMinute,gameHour);
        EventHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
    }

    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 9;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 2025;
        gameSeason = Season.春天;
    }

    void Update(){
        if (!gameClockPause)
        {
            tikTime +=Time.deltaTime;
            if (tikTime>=Settings.secondThreshold)
            {
                tikTime=0;
                UpdateGameTime();
            }
        }

        if(Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < 10; i++)
            {
                UpdateGameTime();
            }
            
        }
        if (Input.GetKeyDown(KeyCode.G))//快速过1天
        {
            gameDay++;
            EventHandler.CallGameDayEvent(gameDay, gameSeason);
            EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        }
    }

    private void UpdateGameTime()
    {
        gameSecond++;
        if (gameSecond > Settings.secondHold)
        {
            gameMinute++;
            gameSecond = 0;

            if (gameMinute > Settings.minuteHold)
            {
                gameHour++;
                gameMinute = 0;

                if (gameHour > Settings.hourHold)
                {
                    gameDay++;
                    gameHour = 0;

                    if (gameDay > Settings.dayHold)
                    {
                        gameDay = 1;
                        gameMonth++;

                        if (gameMonth > 12)
                        {
                            gameMonth = 1;
                        }

                        mounthInSeason--;
                        if (mounthInSeason == 0)
                        {
                            mounthInSeason = 3;

                            int seasonNumber =(int)gameSeason;
                            seasonNumber++;

                            if (seasonNumber>Settings.seasonHold)
                            {
                                seasonNumber = 0;
                                gameYear++;
                            }
                            gameSeason = (Season)seasonNumber;

                            if(gameYear>9999)gameYear =2025;
                        }
                    }
                    //用来刷新地图和农作物生长
                    EventHandler.CallGameDayEvent(gameDay, gameSeason);
                }
                EventHandler.CallGameDateEvent(gameHour,gameDay,gameMonth,gameYear,gameSeason);
            }

            EventHandler.CallGameMinuteEvent(gameMinute,gameHour);
        }

        //Debug.Log($"Hour:{gameHour},minute:{gameMinute},second:{gameSecond}");
    }
}
