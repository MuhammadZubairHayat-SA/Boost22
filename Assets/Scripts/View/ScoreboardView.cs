using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreboardView : MonoBehaviour 
{
	public RushLeaderboardView rushLeaderboardView;
	public CanvasGroup rushLeaderboardButtonCanvasGroup;
	public RectTransform[] positions;
	public Animator[] entries;
	public TextMeshProUGUI[] nameTexts;
	public TextMeshProUGUI[] scoreTexts;
	public TextMeshProUGUI headline;
	public Button playAgainButton;

	private Animator animator;
	private int[] scores;


	void Start () 
	{
		animator = GetComponent<Animator>();
		scores = new int[4];
	}
	

	void Update () 
	{
		
	}


	public void SetPlayerScore (int playerIndex, Player[] players, float delay)
	{
		scores[playerIndex] = players[playerIndex].score;
		StartCoroutine(UpdatePlayerRanks(players, delay));
		StartCoroutine(DisplayPlayerScore(playerIndex, players[playerIndex], delay));
	}


	private IEnumerator DisplayPlayerScore (int playerIndex, Player player, float delay)
	{
		yield return new WaitForSeconds(delay);
		nameTexts[playerIndex].text = player.user.username;
		scoreTexts[playerIndex].text = player.score.ToString();
	}


	public void CrossOutPlayer (int playerIndex)
	{
		entries[playerIndex].SetBool("crossedOut", true);
	}


	public IEnumerator CrossOutPlayerDelayed (int playerIndex, float delay)
	{
		yield return new WaitForSeconds(delay);

		entries[playerIndex].SetBool("crossedOut", true);
	}


	public IEnumerator DisplayCardAtPlayer (CardAnimator cardAnimator, float delay)
	{
		yield return new WaitForSeconds(delay);
		RectTransform cardArea = entries[cardAnimator.playedByPlayerIndex].transform.Find("CardArea").GetComponent<RectTransform>();
		int numberOfCardsAtPlayer = cardArea.childCount;

		cardAnimator.transform.SetParent(cardArea);
		cardAnimator.transform.localScale = Vector2.one * 0.4f;
		cardAnimator.GetComponent<RectTransform>().anchoredPosition = Vector2.zero - (Vector2.right * 40 * numberOfCardsAtPlayer);
	}


	public IEnumerator DisplayCleanSheet (int playerIndex, float delay)
	{
		yield return new WaitForSeconds(delay);
		entries[playerIndex].transform.Find("CleanSheetLogo").gameObject.SetActive(true);
	}


	public void OpenClose ()
	{
		if (animator.GetBool("open") == false)
		{
			Open();
		}
		else
		{
			Close();
		}
	}


	public void Open ()
	{
		if (animator.GetBool("open") == false)
		{
			animator.SetBool("open", true);
			rushLeaderboardView.Close();
			rushLeaderboardButtonCanvasGroup.alpha = 0.5f;
		}
	}


	public void Close ()
	{
		if (animator.GetBool("open") == true)
		{
			animator.SetBool("open", false);
			rushLeaderboardButtonCanvasGroup.alpha = 1.0f;
		}
	}


	public IEnumerator WonGame (float delay)
	{
		yield return new WaitForSeconds(delay);
		
		Open();
		
		headline.text = "Congrats! You won the game!";
		headline.gameObject.SetActive(true);

		playAgainButton.gameObject.SetActive(true);
		MatchController.instance.UpdateData();
        MatchController.instance.TryToSubmitScore();
	}


	public IEnumerator LostGame (int rank, float delay)
	{
		yield return new WaitForSeconds(delay);

		Open();
		
		switch (rank)
		{
			case 1:
				headline.text = "Well done. You got 2nd place";
				break;
			case 2:
				headline.text = "Sorry, you got 3rd place";
				break;
			case 3:
				headline.text = "Sorry, you only got 4th place";
				break;
		}

		headline.gameObject.SetActive(true);
		MatchController.instance.UpdateData();
		MatchController.instance.TryToSubmitScore();
		playAgainButton.gameObject.SetActive(true);
	}


	private IEnumerator UpdatePlayerRanks (Player[] players, float delay)
	{
		yield return new WaitForSeconds(delay);
		
		var scoreboardPlayers = GameManager.shared.GetPlayersByRank();

		for (int i = 0; i < scoreboardPlayers.Count; i++)
		{
			RectTransform rt = entries[scoreboardPlayers[i].tablePosition].GetComponent<RectTransform>();

			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, positions[i].anchoredPosition.y);
		}
	}

	public void GameWonBtn()
	{
		MatchController.instance.UpdateData();
		MatchController.instance.TryToSubmitScore();
	}
}
