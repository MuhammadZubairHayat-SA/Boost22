using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Managers.API;

public class Rush22ResultsScreen : MonoBehaviour 
{
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI subtitleText;
	[SerializeField] private TextMeshProUGUI positionText;
	[SerializeField] private TextMeshProUGUI usernameText;
	[SerializeField] private TextMeshProUGUI prizeText;
	[SerializeField] private TextMeshProUGUI scoreText;
	[SerializeField] private HighscoreView highscoreView;
	[SerializeField] private Animator animator;


	void Start () 
	{
		ScoreApi.GetLatestRushEventResults(Enums.GameType.Boost22, UserManager.LoggedInUser.id, (results, type) =>
		{
			if (results != null)
			{
				if (results.userPosition < 4)
				{
					titleText.text = "Congratulations!";
				}
				else
				{
					titleText.text = "This RUSH22 is over!";
				}

				subtitleText.text = "You finished " + results.userPosition + " of " + results.participantCount;

				positionText.text = results.userPosition.ToString() + ".";
				usernameText.text = UserManager.LoggedInUser.username;

				if (results.userPosition - 1 >= results.scores.Length)
				{
					scoreText.text = results.scores[results.scores.Length - 1].ToString();
				}
				else
				{
					scoreText.text = results.scores[results.userPosition - 1].score.ToString();
				}				

				if (results.scores[results.userPosition - 1].prize > 0)
				{
					prizeText.text = "DKK " + results.scores[results.userPosition - 1].prize.ToString("F2");
				}
				else
				{
					prizeText.text = string.Empty;
				}
			}
		});
		
		highscoreView.UpdateLatestEventList();
		animator.SetTrigger("open");
	}
	

	public void Close ()
	{
		GameViewController gameViewController = FindObjectOfType(typeof(GameViewController)) as GameViewController;
		gameViewController.GoToLobbyFree();
	}


	private IEnumerator DelayedDestroy ()
	{
		yield return new WaitForSeconds(1.0f);
		Destroy(this.gameObject);
	}
}
