using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Managers.API;


public class LobbyGameModes : MonoBehaviour 
{
	[SerializeField] private GameStarter gameStarter;
	[SerializeField] private Animator noGamesLeftPrompt;
	[SerializeField] private Animator startGameRushWarningPrompt;
	[SerializeField] private Animator startGameRushInProgressPrompt;
	[SerializeField] private Animator startPlaygroundGameRushWarningPrompt;
	[SerializeField] private Animator startPlaygroundGameRushInProgressPrompt;
    [SerializeField] private Animator sceneLoadingScreen;
    [SerializeField] private RectTransform spinner;
    [SerializeField] private RectTransform playButton;

    private bool hasPromptedForRush = false;

    public void StartGame(bool isTurbo, Enums.GameModeType gameModeType)
    {
        PlayerPrefsManager.DeleteRush22Ranking();
        sceneLoadingScreen.SetTrigger("open");

        UserApi.GetDynamicUserData((dynamicUserData, error) => 
		{ 
			if (dynamicUserData != null)
			{
				var extraGamesLeftToday = dynamicUserData.extraGamesLeftToday;
                UserManager.LoggedInUser.extraGamesLeftToday = extraGamesLeftToday;
            }

            if (!UserManager.HasLeagueGamesLeftToday())
            {
                noGamesLeftPrompt.SetTrigger("open");
                sceneLoadingScreen.SetTrigger("close");
            } 
            else
            {
                var nextRushEvent = UserManager.LoggedInUser.GetNextRushEvent();
                if (nextRushEvent != null)
                {
                    double minutesUntilRush = nextRushEvent.TimeUntilEvent().TotalMinutes;

                    if (minutesUntilRush <= 10 && minutesUntilRush > 0)
                    {
                        startGameRushWarningPrompt.SetTrigger("open");
                        sceneLoadingScreen.SetTrigger("close");
                        return;
                    }
                    else if (minutesUntilRush <= 0 && minutesUntilRush >= -nextRushEvent.eventTimeInMinutes)
                    {
                        startGameRushInProgressPrompt.SetTrigger("open");
                        sceneLoadingScreen.SetTrigger("close");
                        return;
                    }
                }

                gameStarter.SetGameModeType(gameModeType);
                gameStarter.SetGameType(isTurbo: isTurbo);
                StartCoroutine(LoadYourAsyncScene(1));
            }
        });
    }


    public void StartClassicGame ()
	{
        StartGame(isTurbo: false, gameModeType: Enums.GameModeType.League); // TODO check if it needs to take Rush into account
	}


	public void StartTurboGame ()
	{
        StartGame(isTurbo: true, gameModeType: Enums.GameModeType.League); // TODO check if it needs to take Rush into account
    }


	public void StartGameImmediate (int sceneIndex)
	{
        bool isTurbo = false;
        gameStarter.SetGameModeType(Enums.GameModeType.Rush22);
		PlayerPrefsManager.DeleteRush22Ranking();
		gameStarter.SetGameType(isTurbo);
        StartCoroutine(LoadYourAsyncScene(sceneIndex));
    }


	public void StartWaitingForRush22 (bool isTurbo)
	{
        gameStarter.SetGameModeType(Enums.GameModeType.Rush22);
		PlayerPrefsManager.DeleteRush22Ranking();
		gameStarter.StartWaitingForRush22();
		gameStarter.SetGameType(isTurbo);
        StartCoroutine(LoadYourAsyncScene(2));
    }


    public void StartPlaygroundGame (bool isTurbo)
    {
        var nextRushEvent = UserManager.LoggedInUser.GetNextRushEvent();
        if (nextRushEvent != null && !hasPromptedForRush)
        {
            hasPromptedForRush = true;

            double minutesUntilRush = nextRushEvent.TimeUntilEvent().TotalMinutes;

            if (minutesUntilRush <= 10 && minutesUntilRush > 0)
            {
                startPlaygroundGameRushWarningPrompt.SetTrigger("open");
                return;
            }
            else if (minutesUntilRush <= 0 && minutesUntilRush >= -nextRushEvent.eventTimeInMinutes)
            {
                startPlaygroundGameRushInProgressPrompt.SetTrigger("open");
                return;
            }
        }

        gameStarter.SetGameModeType(Enums.GameModeType.Playground);
        gameStarter.SetGameType(isTurbo: isTurbo);
        StartCoroutine(LoadYourAsyncScene(3));
    }


    private IEnumerator LoadYourAsyncScene(int sceneIndex)
    {
        float playButtonHeight = playButton.sizeDelta.y * playButton.localScale.y;
        spinner.localScale = playButton.localScale;
        spinner.anchoredPosition = playButton.anchoredPosition;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
