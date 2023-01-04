using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

[Serializable]
public class User
{
	public int id;
	public string username;
	public string email;
    public string facebookId;
    public string countryCode;
    public string phoneNumber;
	public string avatarUrl;
	public string token;
	public Enums.RobotType? robotType;
    public int dayScore;
	public int weekScore;
	public int monthScore;
    public int numberOfGamesPlayed;
    public int numberOfGamesPlayedForFacebook;
    public int award1Gold;
    public int award1Silver;
    public int award1Bronze;
    public int award2Gold;
    public int award2Silver;
    public int award2Bronze;

    public int dayPosition;
    public int weekPosition;
    public int monthPosition;

    public int loyaltyPoints;
    public float realMoneyAmount;
    public int realMoneyCurrencyType;

    public int gamesLeftToday;
    public int extraGames;
    public int extraGamesLeftToday;

    public string inviteToken;

    private Event nextRushEvent;

    public List<Enums.RobotType> robotTypes;

    public bool IsAi()
	{
		return robotType != null;
	}

    public Event GetNextRushEvent()
    {
        if (nextRushEvent != null && nextRushEvent.eventType != Enums.GameEventType.NoType)
        {
	        Debug.Log("A" + nextRushEvent);
            return nextRushEvent;
        }
        Debug.Log("A" + nextRushEvent);
        return null;
    }

    public void SetNextRushEvent(Event gameEvent)
    {
        nextRushEvent = gameEvent;
    }

}
