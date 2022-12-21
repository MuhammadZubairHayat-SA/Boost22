using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Facebook.Unity;
using Managers.API;

public class AccountView : MonoBehaviour
{
	public CanvasGroup lobbyGroup;
	public AnimationCurve timerFlashCurve;
	public Image barUserImage;
	public TextMeshProUGUI barUsernameText;
	public TextMeshProUGUI barBalanceText;
	public TextMeshProUGUI barLoyaltyText;
	public TextMeshProUGUI barExtraGamesText;
	public GameObject payoutButton;
	public GameObject noPayoutButton;
    public TextMeshProUGUI versionText;
	public TextMeshProUGUI usernameText;
	public TextMeshProUGUI emailText;
	public TextMeshProUGUI phoneNumberText;
    public TextMeshProUGUI gamesLeftTodayText;
    public TextMeshProUGUI rushCountdownText1;
    public TextMeshProUGUI rushCountdownText2;
	public Animator rushStartingPrompt;
	public CanvasGroup startGameRushWarningPrompt; 
    public Button facebookButton;
    public GameObject facebookConnected;
	public float payoutThreshold = 50.0f;

	private Animator animator;
	private bool hasPromptedForRush = false;


	void Start ()
	{
		animator = GetComponent<Animator>();
	}


	float rushCountdownTimer = 1.0f;
	double midnightCheckTimer = 1.0f;
	private void FixedUpdate ()
	{
        var nextRushEvent = UserManager.LoggedInUser != null ? UserManager.LoggedInUser.GetNextRushEvent() : null;

        if (nextRushEvent != null)
		{
			rushCountdownText1.text = "Next RUSH22 starts in";

			TimeSpan timeToNextRushEvent = nextRushEvent.TimeUntilEvent();		
			rushCountdownText2.text = "<b><mspace=17>" + ((int)timeToNextRushEvent.TotalHours).ToString("00") + "</mspace>:<mspace=17>" + ((int)timeToNextRushEvent.Minutes).ToString("00") + "</mspace>:<mspace=17>" + ((int)timeToNextRushEvent.Seconds).ToString("00") + "</mspace></b>";
			
			if (timeToNextRushEvent.TotalMinutes <= 2 && timeToNextRushEvent.TotalMinutes >= 0 && !hasPromptedForRush && !startGameRushWarningPrompt.gameObject.activeInHierarchy)
			{
				rushStartingPrompt.SetTrigger("open");
				hasPromptedForRush = true;
			}
			else if (timeToNextRushEvent.TotalMinutes < 0)
			{
				if (timeToNextRushEvent.TotalMinutes > -nextRushEvent.eventTimeInMinutes)
				{
					rushCountdownText1.text = "Current RUSH22 ends in";
					TimeSpan timeToEnd = TimeSpan.FromMinutes(nextRushEvent.eventTimeInMinutes) + timeToNextRushEvent;
					rushCountdownText2.text = "<b><mspace=17>" + ((int)timeToEnd.TotalHours).ToString("00") + "</mspace>:<mspace=17>" + ((int)timeToEnd.Minutes).ToString("00") + "</mspace>:<mspace=17>" + ((int)timeToEnd.Seconds).ToString("00") + "</mspace></b>";

					if (timeToEnd.TotalMinutes < 2)
					{
						float alpha = timerFlashCurve.Evaluate(rushCountdownTimer);
						rushCountdownText2.color = new Color(1, 1, 1, alpha);
					}
				}
				else
				{
					rushCountdownText1.text = "Latest RUSH22 has ended";
					rushCountdownText2.text = string.Empty;
				}
			}
			else if (timeToNextRushEvent.TotalMinutes <= 15)
			{
				float alpha = timerFlashCurve.Evaluate(rushCountdownTimer);
				rushCountdownText2.color = new Color(1, 1, 1, alpha);
			}
			else if (rushCountdownText2.color != Color.white)
			{
				rushCountdownText2.color = Color.white;
			}
				
			rushCountdownTimer += Time.fixedDeltaTime;
			if (rushCountdownTimer >= 1)
			{
				rushCountdownTimer = 0.0f;
			}
		}
		else if (rushCountdownTimer <= 1)
		{
			rushCountdownText1.text = "No upcoming RUSH22";
			rushCountdownText2.text = string.Empty;
			rushCountdownTimer = 2.0f;
		}
		
		midnightCheckTimer -= Time.fixedDeltaTime;
		if (midnightCheckTimer <= 0)
		{
			StartCoroutine(DelayedMidnightUpdate(2.0f));
			StartCoroutine(DelayedMidnightUpdate(10.0f));
			UpdateTimeUntilMidnight();
		}
	}

	private float updateTimer = 0.0f;
	private void Update ()
	{
		updateTimer += Time.deltaTime;

		if (updateTimer >= 15)
		{
			UpdateAccount();
		}
	}


	private IEnumerator DelayedMidnightUpdate (float delay)
	{
		yield return new WaitForSeconds(delay);
		UpdateAccount();
	}


	private void OnEnable () 
	{
		UpdateAccount();
		UpdateNextEvent();
		UpdateTimeUntilMidnight();

        var nextRushEvent = UserManager.LoggedInUser.GetNextRushEvent();

        if (nextRushEvent != null)
		{
			TimeSpan timeToNextRushEvent = nextRushEvent.TimeUntilEvent();	
			if (timeToNextRushEvent.TotalMinutes <= 2 && timeToNextRushEvent.TotalMinutes >= 0 && !hasPromptedForRush)
			{
				rushStartingPrompt.SetTrigger("open");
				hasPromptedForRush = true;
			}
		}
		
		versionText.text = "Version " + Application.version;

		if (PlayerPrefs.HasKey("KilledDuringGame"))
		{
			if (PlayerPrefs.GetInt("KilledDuringGame") > 0 && PlayerPrefs.GetString("KilledOnDate") == System.DateTime.Today.ToString("dd/MM/yyyy"))
			{
				if (UserManager.LoggedInUser != null)
				{
					ScoreApi.KilledAppPunishment(UserManager.LoggedInUser, PlayerPrefs.GetInt("KilledDuringEventId"), (success, type) =>
					{
						if (success || type == Enums.NetworkErrorType.EventCanOnlyBePlayedFromOneDevice)
                        {
							Debug.Log("App was killed; punishment successful");
                            PlayerPrefsManager.ClearPlayerPrefsForKilledDuringGame();
						}
						else
						{
							Debug.Log("App was killed; punishment not successful: " + type);
   						}
					});
				}				
			}
			else
			{
				PlayerPrefsManager.ClearPlayerPrefsForKilledDuringGame();
			}
		}
	}


	public void Open ()
	{
		ResetAnimatorTriggers();
		animator.SetBool("open", true);
	}


	public void Close ()
	{
		ResetAnimatorTriggers();
		animator.SetBool("open", false);
	}


	public void UpdateAccount ()
	{
		Debug.Log("Updating account info in account view");
        float balance = UserManager.LoggedInUser.realMoneyAmount;
        int loyalty = UserManager.LoggedInUser.loyaltyPoints;
        int gamesLeftToday = UserManager.LoggedInUser.gamesLeftToday;
		int extraGamesLeftToday = UserManager.LoggedInUser.extraGamesLeftToday;
		int extraGames = UserManager.LoggedInUser.extraGames;

		UserApi.GetDynamicUserData((dynamicUserData, error) => 
		{ 
			if (dynamicUserData != null)
			{
				balance = dynamicUserData.realMoneyAmount;
				loyalty = dynamicUserData.loyaltyPoints;
				gamesLeftToday = dynamicUserData.gamesLeftToday;
				extraGamesLeftToday = dynamicUserData.extraGamesLeftToday;
				extraGames = dynamicUserData.extraGames;

				UserManager.LoggedInUser.realMoneyAmount = balance;
                UserManager.LoggedInUser.loyaltyPoints = loyalty;
                UserManager.LoggedInUser.gamesLeftToday = gamesLeftToday;
				UserManager.LoggedInUser.extraGamesLeftToday = extraGamesLeftToday;
				UserManager.LoggedInUser.extraGames = extraGames;

				//barBalanceText.text = "Balance\n<b>DKK " + balance.ToString("F2") + "</b>";
                //barLoyaltyText.text = "Member score\n<b>" + loyalty + (loyalty == 1 ? " point" : " points") + "</b>";
				//barExtraGamesText.text = "Extra LEAGUE games\n<b><color=#FECF4F>" + extraGames + "</color></b>";
				gamesLeftTodayText.text = "Games in THE LEAGUE\n<b>" + gamesLeftToday.ToString() + "<color=#FECF4F>" + (extraGamesLeftToday > 0 ? " +" + extraGamesLeftToday : string.Empty) + "</color></b>";

                /*if (balance >= payoutThreshold)
                {
                    payoutButton.SetActive(true);
                    noPayoutButton.SetActive(false);
                }
                else
                {
                    payoutButton.SetActive(false);
					noPayoutButton.SetActive(true);
                }*/
            }
			else
			{
				gamesLeftTodayText.text = "Games in THE LEAGUE\n<b>" + gamesLeftToday.ToString() + "<color=#FECF4F>" + (extraGamesLeftToday > 0 ? " +" + extraGamesLeftToday : string.Empty) + "</color></b>";
				//gamesLeftTodayText.text = error.ToString();
			}
		});

        //barBalanceText.text = "Balance\n<b>DKK " + balance.ToString("F2") + "</b>";
        //barLoyaltyText.text = "<b>Member score:</b> " + loyalty + (loyalty == 1 ? " point" : " points");
		//barExtraGamesText.text = "Extra LEAGUE games\n<b><color=#FECF4F>" + extraGames + "</color></b>";

        /*if (balance >= payoutThreshold)
        {
            payoutButton.SetActive(true);
			noPayoutButton.SetActive(false);
        }
        else
        {
            payoutButton.SetActive(false);
			noPayoutButton.SetActive(true);
        }*/

		#if UNITY_ANDROID
			if (!string.IsNullOrEmpty(UserManager.LoggedInUser.facebookId))
			{
				FB.API(UserManager.LoggedInUser.facebookId + "/picture?type=square&height=128&width=128", HttpMethod.GET, FbGetPicture);
				facebookButton.gameObject.SetActive(false);
			}
			else
			{
				facebookButton.gameObject.SetActive(true);
				barUserImage.gameObject.SetActive(false);
			}
		#endif

		#if UNITY_IOS
			facebookButton.gameObject.SetActive(false);
			facebookConnected.SetActive(false);
		#endif

        barUsernameText.text = UserManager.LoggedInUser.username ?? "";
		usernameText.text = UserManager.LoggedInUser.username ?? "";
		emailText.text = UserManager.LoggedInUser.email ?? "";

        if (!string.IsNullOrEmpty(UserManager.LoggedInUser.phoneNumber))
		{
			phoneNumberText.text = "+" + UserManager.LoggedInUser.countryCode + " " + UserManager.LoggedInUser.phoneNumber;
		}
		else
		{
			phoneNumberText.text = "No phone number";
		}	

		updateTimer = 0.0f;	
	}


	public void UpdatePhoneNumber ()
	{
		phoneNumberText.text = "+" + UserManager.LoggedInUser.countryCode + " " + UserManager.LoggedInUser.phoneNumber;
	}


	public void UpdateUsername ()
	{
		usernameText.text = UserManager.LoggedInUser.username;
	}


	public void OpenSupport ()
    {
		string emailAddress = "support@boost22.com";
		string subject = "Boost22 Support";
		string body = "Sent from Boost22 app";

    	Application.OpenURL("mailto:" + emailAddress + "?subject=" + subject + "&body=" + body);
    }


	public void ResetAnimatorTriggers ()
	{
		animator.ResetTrigger("open");
		animator.ResetTrigger("close");
		animator.ResetTrigger("openChangePassword");
		animator.ResetTrigger("closeChangePassword");
		animator.ResetTrigger("openChangeUsername");
		animator.ResetTrigger("closeChangeUsername");
		animator.ResetTrigger("openChangePhoneNumber");
		animator.ResetTrigger("closeChangePhoneNumber");
		animator.ResetTrigger("openLogout");
		animator.ResetTrigger("closeLogout");
		animator.ResetTrigger("openTerms");
		animator.ResetTrigger("closeTerms");
		animator.ResetTrigger("openFacebook");
		animator.ResetTrigger("closeFacebook");
	}


	public void UpdateNextEvent ()
	{
		Debug.Log("Updating next event in account view");
		if (UserManager.LoggedInUser != null && UserManager.LoggedInUser.GetNextRushEvent() != null)
		{
			//rushPlayNowButton.interactable = true;
		}
		else
		{
			//rushPlayNowButton.interactable = false;
			ScoreApi.GetNextRushEvent((nextEvent, type) =>
			{
				UserManager.LoggedInUser.SetNextRushEvent(nextEvent);
				//rushPlayNowButton.interactable = true;
			});
		}
	}

	
	private void OnApplicationFocus(bool focusStatus) 
	{
		UpdateAccount();
		UpdateNextEvent();	
		UpdateTimeUntilMidnight();
	}


    private void FbGetPicture(IGraphResult result)
    {
        if (result.Texture != null)
        {
            barUserImage.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
            barUserImage.gameObject.SetActive(true);
        }
        else
        {
            barUserImage.gameObject.SetActive(false);
        }
    }


	private void UpdateTimeUntilMidnight ()
	{
		TimeSpan timeUntilMidnight = DateTime.Today.AddDays(1.0) - DateTime.Now;
		midnightCheckTimer = timeUntilMidnight.TotalSeconds;
	}
}
