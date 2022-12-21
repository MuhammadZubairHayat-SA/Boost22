using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class LocalNotificationManager : MonoBehaviour
{
    public int hourToNotify = 19;
    public string weeklyTitle = "BOOST22 Weekly";
    public string[] weeklyMessages;
    public int numberOfWeeksToNotify = 4;
    public DayOfWeek weekdayToNotify = DayOfWeek.Sunday;
    public string monthlyTitle = "BOOST22 Monthly";
    public string[] monthlyMessages;
    public int numberOfMonthsToNotify = 2;
    public bool notifyOnLastDayOfMonth = true;
    public string weekAndMonthlyMessage;
    public string reminderTitle = "BOOST22 Reminder";
    public string[] reminderMessages;
    public int remindAfterDays = 2;
    public string rushFirstTitle = "RUSH22 Reminder";
    public string rushFirstMessage = "Next RUSH22 starts in 30 minutes";
    public string rushSecondTitle = "RUSH22 Reminder";
    public string rushSecondMessage = "RUSH22 is about to start!";
    public int numberOfDaysToNotifyAboutRush = 14;
//#if UNITY_IOS
//    private List<string> notificationIdentifiers = new List<string>();
//#endif


    void Start ()
    {
        DontDestroyOnLoad(gameObject);
#if UNITY_ANDROID
        CreateNotificationChannel();
#endif
    }


#if UNITY_ANDROID
    private AndroidNotificationChannel androidNotificationChannel;
    private const string channelId = "0";
 
    private void CreateNotificationChannel()
    {
        androidNotificationChannel = new AndroidNotificationChannel();
        androidNotificationChannel.Id = channelId;
        androidNotificationChannel.Name = "Default Channel";
        androidNotificationChannel.Importance = Importance.High;
        androidNotificationChannel.Description = "Generic notifications";
        AndroidNotificationCenter.RegisterNotificationChannel(androidNotificationChannel);
    }
#endif


    private void OnApplicationPause (bool pause) 
    {
        if (pause)
        {
            ScheduleWeeklyNotifications();        
            ScheduleMonthlyNotifications();
            ScheduleReminder();
            ScheduleRushReminders();
            //ScheduleTestNotifications();
        }
        else
        {
#if UNITY_IOS
                UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
#endif
#if UNITY_ANDROID
                AndroidNotificationCenter.CancelAllScheduledNotifications();
#endif
        }
    }


    private void ScheduleTestNotifications ()
    {
        DateTime triggerTime = DateTime.Today;
        triggerTime += new TimeSpan(11, 0, 0);

        //for (int i = 0; i < 4; i++)
        //{
            SendNotification("BOOST22 Test", "Test test test", triggerTime);
        //}
    }


    private void ScheduleWeeklyNotifications ()
    {
        int daysUntilWeek = ((int) weekdayToNotify - (int) DateTime.Today.DayOfWeek + 7) % 7;
        DateTime nextWeek = DateTime.Today.AddDays(daysUntilWeek);
        nextWeek += new TimeSpan(hourToNotify, 0, 0);

        string title = weeklyTitle;

        for (int i = 0; i < numberOfWeeksToNotify; i++)
        {
            DateTime triggerTime = nextWeek.AddDays(7 * i);
            SendNotification(weeklyTitle, "Challenge your friends in THE LEAGUE. Play Daily or Weekly. You decide the bet!", triggerTime + TimeSpan.FromHours(2.0f));
            
            int daysInMonth = DateTime.DaysInMonth(triggerTime.Year, triggerTime.Month);
            
            if (triggerTime.Day == daysInMonth)
            {
                SendNotification("BOOST22 Weekly & Monthly", weekAndMonthlyMessage, triggerTime);
            }
            else
            {
                int messageIndex = UnityEngine.Random.Range(0, weeklyMessages.Length - 1);
                string message = weeklyMessages[messageIndex];
                SendNotification(title, message, triggerTime);
            }
        }
    }


    private void ScheduleMonthlyNotifications ()
    {
        for (int i = 0; i < numberOfMonthsToNotify; i++)
        {
            DateTime newMonth = DateTime.Today.AddMonths(i);
            newMonth += new TimeSpan(hourToNotify, 0, 0);
            int daysInMonth = DateTime.DaysInMonth(newMonth.Year, newMonth.Month);
            DateTime triggerTime = new DateTime(newMonth.Year, newMonth.Month, daysInMonth);
            if (!notifyOnLastDayOfMonth)
            {
                triggerTime = new DateTime(newMonth.Year, newMonth.Month, 1);
            }

            if (triggerTime.DayOfWeek != weekdayToNotify)
            {
                string title = monthlyTitle;
                int messageIndex = UnityEngine.Random.Range(0, monthlyMessages.Length - 1);
                string message = monthlyMessages[messageIndex];
                SendNotification(title, message, triggerTime);
            }
        }
    }


    private void ScheduleReminder ()
    {
        DateTime triggerTime = DateTime.Today.AddDays(remindAfterDays);
        triggerTime += new TimeSpan(hourToNotify, 0, 0);
        int daysInMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);


        if (((notifyOnLastDayOfMonth && triggerTime.Day != daysInMonth) || (!notifyOnLastDayOfMonth && triggerTime.Day != 1)) && triggerTime.DayOfWeek != weekdayToNotify)
        {
            SendNotification(reminderTitle, reminderMessages[UnityEngine.Random.Range(0, reminderMessages.Length - 1)], triggerTime);
        }
    }


    private void ScheduleRushReminders()
    {
        if (UserManager.LoggedInUser != null && UserManager.LoggedInUser.GetNextRushEvent() != null) 
        {
            for (int i = 0; i < numberOfDaysToNotifyAboutRush; i++)
            {
                DateTime eventTime = DateTime.Today + TimeSpan.FromDays(i);
                
                SendNotification(rushFirstTitle, rushFirstMessage, eventTime + TimeSpan.FromHours(20.5f) - TimeSpan.FromMinutes(30));
                SendNotification(rushSecondTitle, rushSecondMessage, eventTime + TimeSpan.FromHours(20.5f) - TimeSpan.FromMinutes(2));

                SendNotification(rushFirstTitle, rushFirstMessage, eventTime + TimeSpan.FromHours(12) - TimeSpan.FromMinutes(30));
                SendNotification(rushSecondTitle, rushSecondMessage, eventTime + TimeSpan.FromHours(12) - TimeSpan.FromMinutes(2));
            }
        }
    }


    private void SendNotification (string title, string message, DateTime triggerTime)
    {
        //Debug.Log("Notification: " + title + ", Date time: " + triggerTime.ToString());
#if UNITY_ANDROID
            AndroidNotification androidNotification = new AndroidNotification();
            androidNotification.Title = title;
            androidNotification.Text = message;
            androidNotification.FireTime = triggerTime;
            androidNotification.SmallIcon = "icon_0";
            androidNotification.LargeIcon = "icon_1";
    
            int id = AndroidNotificationCenter.SendNotification(androidNotification, channelId);
            Debug.Log("Notification status = " + AndroidNotificationCenter.CheckScheduledNotificationStatus(id));
#endif

            

#if UNITY_IOS
            UnityEngine.iOS.LocalNotification iOSNotification = new UnityEngine.iOS.LocalNotification();
            iOSNotification.alertTitle = title;
            iOSNotification.alertBody = message;
            iOSNotification.fireDate = triggerTime;
            UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(iOSNotification);

            /* var timeTrigger = new iOSNotificationCalendarTrigger()
            {
                Year = triggerTime.Year;
                Month = triggerTime.Month;
                Day = triggerTime.Day;
                Hour = triggerTime.Hour;
                Minute = triggerTime.Minute;
                Second = triggerTime.Second;
            }

            var notification = new iOSNotification()
            {
                Identifier = "_notification" + triggerTime,
                notificationIdentifiers.Add(Identifier);
                Title = title,
                Body = message,
                Subtitle = string.Empty,
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.NotificationPresentationOptionAlert | PresentationOption.NotificationPresentationOptionSound),
                CategoryIdentifier = "category_a",
                ThreadIdentifier = "thread1",
                Trigger = timeTrigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification); */
#endif
    }
}