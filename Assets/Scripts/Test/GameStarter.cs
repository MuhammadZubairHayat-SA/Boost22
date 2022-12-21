using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Facebook.Unity;
using Managers.API;

public class GameStarter : MonoBehaviour 
{
	public Enums.GameType gameType;
	
	public bool setHandsManually = false;
	public List<Enums.CardType> player0CardTypes;
	public List<Enums.CardType> player1CardTypes;
	public List<Enums.CardType> player2CardTypes;
	public List<Enums.CardType> player3CardTypes;
	public int gameStartingPlayerIndex = 0;
	public bool? hasAcceptedRush22 = null;

	private bool waitForRush22 = false;
	private Enums.GameModeType gameModeType = Enums.GameModeType.League;
	
	private void Awake ()
	{
		if (FB.IsInitialized) 
		{
			FB.ActivateApp();
		} 
		else 
		{
			//Handle FB.Init
			FB.Init( () => {
			FB.ActivateApp();
			});
		}

		UpdateRushEvent();
		Application.targetFrameRate = Screen.currentResolution.refreshRate;
	}


	private float eventUpdateTimer = 595.0f;
	private void Update ()
	{
		if (eventUpdateTimer > 600)
		{
			UpdateRushEvent();
			eventUpdateTimer = 0.0f;
		}
		else
		{
			eventUpdateTimer += Time.deltaTime;
		}
	}


	void OnApplicationPause (bool pauseStatus)
	{
		// Check the pauseStatus to see if we are in the foreground
		// or background
		if (!pauseStatus) 
		{
			//app resume
			if (FB.IsInitialized) 
			{
				FB.ActivateApp();
			} 
			else 
			{
				//Handle FB.Init
				FB.Init( () => {
					FB.ActivateApp();
				});
			}
		}
	}
	

	private void OnEnable()
	{
		DontDestroyOnLoad(this.gameObject);
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}


	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}


	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex > 0)
		{
			PlayerPrefsManager.SetComingFromGameScene(true);

			if (waitForRush22)
			{
				StartCoroutine(WaitForRush22());
			}
			else if (gameModeType == Enums.GameModeType.Playground)
			{
				StartCoroutine(StartPlaygroundGame());
			}
			else
			{
				StartCoroutine(StartGame());
			}
		}
		else
		{
			PlayerPrefsManager.ClearIsReadyForRushEvent();
			gameModeType = Enums.GameModeType.Playground;
		}
	}


	void OnApplicationFocus(bool hasFocus)
    {
		if (hasFocus)
		{
			UpdateRushEvent();
		}
    }


	public void UpdateRushEvent ()
	{
		ScoreApi.GetNextRushEvent((nextEvent, type) =>
		{
			Debug.Log("Getting next event...");

			if (nextEvent != null && UserManager.LoggedInUser != null)
			{
				UserManager.LoggedInUser.SetNextRushEvent(nextEvent);
			}
		});
	}


	public void SetGameType (bool isTurbo)
	{
		if (isTurbo)
		{
			gameType = Enums.GameType.Turbo;
		}
		else
		{
			gameType = Enums.GameType.Boost22;
		}
	}


	public bool IsPlaygroundGame ()
	{
		return gameModeType == Enums.GameModeType.Playground;
	}

	public bool IsLeagueGame()
	{
		return gameModeType == Enums.GameModeType.League;
	}

	public void SetGameModeType (Enums.GameModeType value)
	{
		gameModeType = value;
	}

	public void StartWaitingForRush22 ()
	{
		waitForRush22 = true;
	}


	public IEnumerator WaitForRush22 ()
	{
		yield return new WaitForEndOfFrame();

		AcceptRush22();
		List<List<Enums.CardType>> playerCardTypes = new List<List<Enums.CardType>>();
		GameViewController gameViewController = FindObjectOfType(typeof(GameViewController)) as GameViewController;
		gameViewController.ShowWaitScreen();
	}

	
	public IEnumerator StartGame ()
	{
		UpdateRushEvent();
		yield return new WaitForEndOfFrame();
		
		List<List<Enums.CardType>> playerCardTypes = new List<List<Enums.CardType>>();
		if (setHandsManually)
		{
			playerCardTypes.Add(player0CardTypes);
			playerCardTypes.Add(player1CardTypes);
			playerCardTypes.Add(player2CardTypes);
			playerCardTypes.Add(player3CardTypes);
			GameManager.shared.NewRiggedGame(gameType, playerCardTypes, gameStartingPlayerIndex);
		}
		else
		{
			GameManager.shared.NewGame(gameType);
		}
	}


	private IEnumerator StartPlaygroundGame ()
	{
		yield return new WaitForEndOfFrame();
		GameManager.shared.NewGame(gameType);
	}


	public void AcceptRush22 ()
	{
		hasAcceptedRush22 = true;
	}
	
	
	public void DeclineRush22 ()
	{
		hasAcceptedRush22 = true;
	}


	public void ClearRush22Accept ()
	{
		hasAcceptedRush22 = null;
	}


	private IEnumerator PresentPlayers ()
	{
		GameManager.shared.gameViewController.PresentPlayers();
		yield return new WaitForSeconds(4);
		GameManager.shared.NewGame(gameType);
	}
}
