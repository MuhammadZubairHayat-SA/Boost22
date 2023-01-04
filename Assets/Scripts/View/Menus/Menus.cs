using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
//using Facebook.Unity;
using Managers.API;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using TMPro;

public class Menus : MonoBehaviour 
{
	public Animator screensAnimator;
	public GameObject titleScreenSpinner;
	public CanvasGroup titleScreenErrorPrompt;
	public RectTransform buttonsParent;
	public RectTransform rightPanel;
	public GameObject extraGamesTextObject;
	public GameObject rushCountdownObject;

	private bool showErrorPrompt = false;


	void Awake ()
	{
		/*if (FB.IsInitialized) {
			FB.ActivateApp();
		} else {
			//Handle FB.Init
			FB.Init( () => {
				FB.ActivateApp();
			});
		}*/
	}

	void Start () 
	{
        float aspectRatio = Screen.width / (float)Screen.height;
		int offsetTop = 0;

		if (aspectRatio < 1.34f) // 4:3
		{
			offsetTop = 460;
		}
		else
		{
			offsetTop = 180;
		}
		
		buttonsParent.offsetMax = new Vector2(0, -offsetTop);
		buttonsParent.offsetMin = new Vector2(0, 40);
	}


	private float errorPromptTimer = 0.0f;
	private void Update ()
	{
		if (showErrorPrompt && errorPromptTimer <= 1)
		{
			errorPromptTimer += Time.deltaTime;
			titleScreenErrorPrompt.alpha = errorPromptTimer;
		}
		else if (!showErrorPrompt && errorPromptTimer >= 0)
		{
			errorPromptTimer -= Time.deltaTime;
			titleScreenErrorPrompt.alpha = errorPromptTimer;
		}
	}


	public void SelectLobbyMenu (int menuIndex)
	{
		for (int i = 0; i < rightPanel.childCount; i++)
		{
            SetMenuAndButtonActive(i, setActive: i == menuIndex);
		}

		//DisplayExtraGames(menuIndex == 0);
	}


	private void SetMenuAndButtonActive (int menuIndex, bool setActive)
	{
		rightPanel.GetChild(menuIndex).gameObject.SetActive(setActive);
		buttonsParent.GetChild(menuIndex).GetComponent<Gradient2>().enabled = setActive;
		
		Color buttonColor = setActive ? Color.white : Color.white * 0.25f;
		buttonsParent.GetChild(menuIndex).GetComponent<Image>().color = buttonColor;
	}


	private void DisplayExtraGames (bool displayExtraGames)
	{
		if (displayExtraGames)
		{
			extraGamesTextObject.SetActive(true);
			rushCountdownObject.SetActive(false);
		}
		else
		{
			extraGamesTextObject.SetActive(false);
			rushCountdownObject.SetActive(true);
		}
	}


	public void AttemptLogin ()
	{
		titleScreenErrorPrompt.gameObject.SetActive(false);
		showErrorPrompt = false;
		
		if (UserManager.HasLoggedInUser())
		{
			titleScreenSpinner.SetActive(true);

			UserApi.Update((u, e) =>
			{
				if (u != null)
				{
					screensAnimator.SetBool("proceedFromTitleScreen", true);
					screensAnimator.SetBool("toLobby", true);
					SelectLobbyMenu(0);

					if (!PlayerPrefs.HasKey("DeviceLanguage") || (PlayerPrefs.HasKey("DeviceLanguage") && PlayerPrefs.GetString("DeviceLanguage") != Application.systemLanguage.ToString()))
					{
						UserApi.ChangeProfileData(u.username, u.phoneNumber, u.countryCode, (success, errorType) => {});
					}
				}
				else
				{
					titleScreenErrorPrompt.transform.Find("Frame").Find("Text").GetComponent<TextMeshProUGUI>().text = Api.ErrorMessage(e);
					titleScreenErrorPrompt.gameObject.SetActive(true);
					showErrorPrompt = true;
				}

				titleScreenSpinner.SetActive(false);
			});
		}
		else
		{
			screensAnimator.SetBool("toLobby", false);
			screensAnimator.SetBool("proceedFromTitleScreen", true);
			SelectLobbyMenu(0);
		}
	}
	
	// Unity will call OnApplicationPause(false) when an app is resumed
// from the background
	void OnApplicationPause (bool pauseStatus)
	{
		// Check the pauseStatus to see if we are in the foreground
		// or background
		if (!pauseStatus) {
			//app resume
			/*if (FB.IsInitialized) {
				FB.ActivateApp();
			} else {
				//Handle FB.Init
				FB.Init( () => {
					FB.ActivateApp();
				});
			}*/
		}
	}

	public void Logout ()
	{
		UserManager.LogOut();
		screensAnimator.SetBool("toLobby", false);
	}


	public void OpenStoreToUpdate ()
	{
		#if UNITY_ANDROID
			Application.OpenURL("https://play.google.com/store/apps/details?id=com.boost22.cardgame");
			return;
		#endif
		
		#if UNITY_IOS
			Application.OpenURL("https://apps.apple.com/dk/app/boost22-the-card-game/id1460689932");
			return;
		#endif
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}


	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}


	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "Menus" && PlayerPrefs.HasKey("ComingFromGameScene") && PlayerPrefs.GetInt("ComingFromGameScene") > 0 && PlayerPrefs.HasKey("KilledDuringGame") && PlayerPrefs.GetInt("KilledDuringGame") == 0)
		{
			screensAnimator.SetBool("showIntroScreen", false);
			screensAnimator.SetBool("toLobby", true);
			screensAnimator.Play("MenuScreens_lobbyFadeIn", -1, 0);
			SelectLobbyMenu(0);
			PlayerPrefsManager.SetComingFromGameScene(false);
		}
		else if (scene.name == "Menus")
		{
			AttemptLogin();
		}
	}
}
