using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Event
{
    public int id;
    public string title;
    public string subtitle;
    public float prizePool;
    public int currencyType;
    public Enums.GameEventType eventType;
    public double startTime;
    public int eventTimeInMinutes;
    public int secondsToEventStart;
    public string prizeDistribution;

    private TimeSpan timeCorrectionInMilliseconds;
    private bool timeCorrectionHasBeenPrepared = false;

    /* PrepareTimeCompensation should be called immediately after a call to the server that fetches an event "startTime" and "secondsToEventStart".
     * timeCorrectionInMilliseconds will be updated, also compensating for Constants.expectedDelayOnTimeFromServerInMilliseconds.
     */
    public void PrepareTimeCompensation ()
    {
        if (!timeCorrectionHasBeenPrepared)
        {
            timeCorrectionHasBeenPrepared = true;
            TimeSpan timeToNextRushEvent = LocalStartTime() - DateTime.Now;
            timeCorrectionInMilliseconds = TimeSpan.FromSeconds(secondsToEventStart) - timeToNextRushEvent - TimeSpan.FromMilliseconds(Helpers.Constants.expectedDelayOnTimeFromServerInMilliseconds);
            if (timeCorrectionInMilliseconds.Milliseconds < 3000) 
            {
                timeCorrectionInMilliseconds = TimeSpan.FromSeconds(0);
            }
            var secsToEventStart = TimeSpan.FromSeconds(secondsToEventStart);
        }
    }

    public TimeSpan TimeUntilEvent ()
    {
        PrepareTimeCompensation();
        TimeSpan timeToNextRushEvent = LocalStartTime() - DateTime.Now - timeCorrectionInMilliseconds;
        return timeToNextRushEvent;
    }


    public DateTime LocalStartTime (bool timeCorrection = true)
    {
        if (timeCorrection) {
            PrepareTimeCompensation();
            return TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(startTime), TimeZoneInfo.Local) - timeCorrectionInMilliseconds;
        } else
        {
            return TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(startTime), TimeZoneInfo.Local);
        }
    }


    public DateTime LocalEndTime (bool timeCorrection = true)
    {
        PrepareTimeCompensation();
        DateTime localEndTime = LocalStartTime(timeCorrection) + TimeSpan.FromMinutes(eventTimeInMinutes);
        return localEndTime;
    }


    public float PrizePool ()
    {
        prizePool = 0.0f;
        string[] prizeStrings = prizeDistribution.Split(',');
        int[] prizes = new int[prizeStrings.Length];
        for (int i = 0; i < prizeStrings.Length; i++)
        {
            int.TryParse(prizeStrings[i].Trim(), out prizes[i]);
            prizePool += prizes[i];
        }

        return prizePool;
    }
}
