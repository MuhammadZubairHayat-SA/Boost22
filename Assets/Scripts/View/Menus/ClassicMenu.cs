using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClassicMenu : MonoBehaviour 
{
	[SerializeField] private Image playNowClassicButton;
	[SerializeField] private Image playNowClassicButtonLight;
	[SerializeField] private Color rushColor = Color.red;
	[SerializeField] private Color rushLightColor = Color.yellow;
	[SerializeField] private Color classicColor = Color.green;
	[SerializeField] private Color classicLightColor = Color.yellow;
	

	void OnAwake () 
	{
		UpdatePlayNowButtonColor();
	}
	

	private float timer = 1.0f;
	void FixedUpdate () 
	{
		if (UserManager.LoggedInUser.GetNextRushEvent() != null && timer >= 1)
		{
			UpdatePlayNowButtonColor();			
			timer = 0.0f;	
		}

		timer += Time.fixedDeltaTime;
	}


	public void UpdatePlayNowButtonColor ()
	{
        var nextRushEvent = UserManager.LoggedInUser.GetNextRushEvent();
        if (nextRushEvent != null)
		{
			DateTime nextRushEventDateTime = nextRushEvent.LocalStartTime();

			if (nextRushEvent.TimeUntilEvent().TotalMinutes < 0 && nextRushEvent.TimeUntilEvent().TotalMinutes > -nextRushEvent.eventTimeInMinutes)
			{
				playNowClassicButton.color = rushColor;
				playNowClassicButtonLight.color = rushLightColor;
			}
			else
			{
				playNowClassicButton.color = classicColor;
				playNowClassicButtonLight.color = classicLightColor;
			}
		}
		else
		{
			playNowClassicButton.color = classicColor;
			playNowClassicButtonLight.color = classicLightColor;
		}
	}
}
