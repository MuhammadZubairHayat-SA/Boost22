using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Rush22Prompt : MonoBehaviour 
{
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private string title = "Get ready to win";
	private bool isEnabled = false;
	

	private void Update ()
	{
        var nextRushEvent = UserManager.LoggedInUser.GetNextRushEvent();

        if (isEnabled && nextRushEvent != null)
		{
			TimeSpan timeToNextRushEvent = nextRushEvent.TimeUntilEvent();
			
			if (timeToNextRushEvent.TotalMinutes <= -nextRushEvent.eventTimeInMinutes)
			{
				transform.parent.GetComponent<Animator>().SetTrigger("close");
			}
		}
		else if (nextRushEvent == null)
		{
			transform.parent.GetComponent<Animator>().SetTrigger("close");
		}
	}


	private void OnEnable() 
	{
        /*var nextRushEvent = UserManager.LoggedInUser.GetNextRushEvent();

        if (nextRushEvent != null)
		{
			titleText.text = title;
			float prizePool = nextRushEvent.PrizePool();
			
			if (prizePool > 0)
			{
				titleText.text += " DKK " + prizePool.ToString("F2");
			}
		}*/

		isEnabled = true;
	}


	private void OnDisable ()
	{
		isEnabled = false;
	}
}
