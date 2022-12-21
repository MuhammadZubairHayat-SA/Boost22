using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Managers.API;
using System.Linq;

public class RushLeaderboardView : MonoBehaviour
{
    [SerializeField] private ScoreboardView scoreboardView;
    [SerializeField] private Transform[] listItems;
    [SerializeField] private Transform playerItem;
    [SerializeField] private float updateInterval = 15.0f;
    [SerializeField] private Color rushColor = Color.red;
    [SerializeField] private Color playerColor = Color.yellow;
    
    private Animator animator;
    private float timer = 0.0f;


    /*
        - Player is part of the list if in top 10, otherwise posted below the list
        
        - If player has same score as first position but is not in first position, make sure that he is
    */


    void Start()
    {
        animator = GetComponent<Animator>();
        timer = updateInterval;
    }


    void Update()
    {
        if (timer >= updateInterval)
        {
            UpdateList();
            timer = 0.0f;
        }
        else if (animator.GetBool("open") == true)
        {
            timer += Time.deltaTime;
        }
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
            UpdateList();
            scoreboardView.Close();
		}
    }


    public void Close ()
    {
        if (animator == null)
        {
            return;
        }

        if (animator.GetBool("open") == true)
		{
			animator.SetBool("open", false);
            timer = 0.0f;
		}
    }


    private void UpdateList()
    {
        if (GameManager.shared.currentEvent == null)
        {
            return;
        }

        ClearList();

        bool playerIsOnList = false;
        ScoreApi.GetRunningEventLeaders(GameManager.shared.currentEvent.id, (results, type) =>
        {
            if (results != null)
            {
                var leadersGroupedByScore = results.leaders.OrderByDescending(x => x.score).GroupBy(x => x.score);

                var position = 1;
                int i = 0;
                foreach (IEnumerable<MinimalEventLeader> leaderGroup in leadersGroupedByScore)
                {
                    foreach (MinimalEventLeader leaderItem in leaderGroup)
                    {
                        if (i >= 10)
                        {
                            goto NoMoreItems;
                        }
                        HighscoreCell cell = listItems[i].GetComponent<HighscoreCell>();
                        cell.nameText.text = leaderItem.username;
                        cell.pointText.text = leaderItem.score.ToString();
                        cell.numberText.text = position.ToString() + ".";
                        if (leaderItem.userId == UserManager.LoggedInUser.id)
                        {
                            listItems[i].GetComponent<Image>().color = playerColor;
                            playerItem.gameObject.SetActive(false);
                            playerIsOnList = true;
                        }
                        else
                        {
                            listItems[i].GetComponent<Image>().color = rushColor;
                        }
                        i++;
                    }
                    position += leaderGroup.Count();
                }
            }

        NoMoreItems:
            HighscoreCell playerCell = playerItem.GetComponent<HighscoreCell>();
            if (!playerIsOnList)
            {
                playerItem.gameObject.SetActive(true);
                if (UserManager.LoggedInUser != null && PlayerPrefs.HasKey("Rush22Ranking_position"))
                {
                    playerCell.nameText.text = UserManager.LoggedInUser.username;
                    playerCell.pointText.text = PlayerPrefs.GetInt("Rush22Ranking_score").ToString();
                    playerCell.numberText.text = PlayerPrefs.GetInt("Rush22Ranking_position").ToString() + ".";
                }
            }
            else
            {
                playerCell.nameText.text = string.Empty;
                playerCell.pointText.text = string.Empty;
                playerCell.numberText.text = string.Empty;
            }
        });
    }


    private void ClearList ()
    {
        foreach (Transform item in listItems)
        {
            HighscoreCell playerCell = item.GetComponent<HighscoreCell>();
            playerCell.nameText.text = string.Empty;
            playerCell.pointText.text = string.Empty;
            playerCell.numberText.text = string.Empty;
            item.GetComponent<Image>().color = rushColor;
        }

        playerItem.gameObject.SetActive(false);
    }
}
