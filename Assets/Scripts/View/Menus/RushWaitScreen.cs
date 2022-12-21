using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Helpers;
using Managers.API;

public class RushWaitScreen : MonoBehaviour 
{
	[HideInInspector] public bool isWaiting = false;

	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI countdownText;
	[SerializeField] private TextMeshProUGUI prizePoolText;
	[SerializeField] private TextMeshProUGUI prizeDistributionText;
	[SerializeField] private TextMeshProUGUI participantsText;
	[SerializeField] private AnimationCurve timerFlashCurve;
	[SerializeField] private GameObject guideCanvas;

	private GameStarter gameStarter;
	private AudioSource audioSource;


	private void OnEnable ()
	{
        var nextRushEvent = UserManager.LoggedInUser.GetNextRushEvent();

        if (nextRushEvent != null && nextRushEvent.TimeUntilEvent().TotalMilliseconds > 0)
		{
			StartWaiting();
		}
		else
		{
			StartCoroutine(StartGame(delay: 0));
		}

		audioSource = GetComponent<AudioSource>();
	}


	float timer = 1.0f;
	float timerEventRankingUpdate = Constants.timeBetweenEventRankingUpdatesInSeconds;
	private void Update ()
	{
        var nextRushEvent = UserManager.LoggedInUser.GetNextRushEvent();

        if (nextRushEvent != null && isWaiting)
		{
			TimeSpan timeToNextRushEvent = nextRushEvent.TimeUntilEvent();
            var hourText = (int)timeToNextRushEvent.TotalHours > 0 ? "<mspace=28>" + ((int)timeToNextRushEvent.TotalHours).ToString("00") + "</mspace>:" : "";

            countdownText.text = hourText + "<mspace=28>" + ((int)timeToNextRushEvent.Minutes).ToString("00") + "</mspace>:<mspace=28>" + ((int)timeToNextRushEvent.Seconds).ToString("00") + "</mspace>";
			
			if (timeToNextRushEvent.TotalSeconds <= 0)
			{
				StartCoroutine(StartGame(delay: 1));
			}
			else if (timeToNextRushEvent.TotalSeconds <= 10)
			{
				titleText.text = "RUSH22 is starting...";
				if (!audioSource.isPlaying)
				{
					audioSource.Play();
				}
				
				float titleAlpha = timerFlashCurve.Evaluate(timer);
				titleText.color = new Color(1, 1, 1, titleAlpha);
				
				float countdownAlpha = Mathf.Lerp(0, 1, ((float)timeToNextRushEvent.TotalSeconds - 5) * 0.2f);
				countdownText.color = new Color(1, 1, 1, countdownAlpha);

				timer += Time.deltaTime;
				if (timer >= 1)
				{
					timer = 0.0f;
				}
			}
			else
			{
				titleText.text = "Waiting for RUSH22 to start...";
				countdownText.color = Color.white;
			}

			timerEventRankingUpdate += Time.deltaTime;
			if (timerEventRankingUpdate >= Constants.timeBetweenEventRankingUpdatesInSeconds)
			{
				timerEventRankingUpdate = 0;
				ScoreApi.GetRunningEventResults(GameManager.shared.currentEvent.id, (results, type) =>
				{
					if (results != null)
					{
						if (!participantsText.isActiveAndEnabled)
						{
							participantsText.gameObject.SetActive(true);
						}
						int participantCount = results.participantCount < 1 ? 1 : results.participantCount;
						participantsText.text = participantCount + (participantCount == 1 ? " player is ready" : " players are ready");
					}
				});
			}
		}
		else if (isWaiting)
		{
			titleText.text = "No upcoming RUSH22";
			countdownText.text = string.Empty;
			isWaiting = false;
		}
	}


	public void Hide ()
	{
		isWaiting = false;
		transform.parent.GetComponent<Animator>().SetTrigger("close");
		guideCanvas.SetActive(false);
	}


	private void StartWaiting ()
	{
		isWaiting = true;

		transform.parent.GetComponent<Animator>().SetTrigger("open");

        var nextRushEvent = UserManager.LoggedInUser.GetNextRushEvent();

        if (nextRushEvent != null)
		{			

			if (nextRushEvent.prizeDistribution != null && nextRushEvent.prizePool > 0)
			{
				prizePoolText.text = "Prize pool: DKK " + nextRushEvent.PrizePool().ToString("F2");
				int numberOfWinners = nextRushEvent.prizeDistribution.Split(',').Length;
				if (numberOfWinners > 1)
				{
					prizeDistributionText.text = "The top " + nextRushEvent.prizeDistribution.Split(',').Length + " players win (DKK " + nextRushEvent.prizeDistribution.Replace(",", "/") + ")";
   				}
				else
				{
					prizeDistributionText.text = "The winner takes it all";
				}
			}			
		}

		guideCanvas.SetActive(true);
	}


	private IEnumerator StartGame (float delay)
	{
		isWaiting = false;

		yield return new WaitForSeconds(delay);

		gameStarter = FindObjectOfType(typeof(GameStarter)) as GameStarter;
		GameManager.shared.NewGame(gameStarter.gameType);
		
		Hide();
	}
}
