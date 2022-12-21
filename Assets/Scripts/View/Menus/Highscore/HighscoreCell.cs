using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighscoreCell : MonoBehaviour
{
	public bool rush22 = false;
	public TextMeshProUGUI numberText;
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI pointText;
	public TextMeshProUGUI prizeText;
	public GameObject award1Gold;
	public GameObject award1Silver;
	public GameObject award1Bronze;
	public GameObject award2Gold;


	public void Setup(int index, Score score, bool isPrizeWinner = false)
	{
		numberText.text = index + ".";
		nameText.text = score.user.username;
		pointText.text = score.score + "";

		if (rush22)
		{
			/*if (score.prize > 0)
			{
				prizeText.text = "DKK " + score.prize.ToString("F2");
			}
			else
			{
				prizeText.text = string.Empty;
			}*/
			
			award1Bronze.SetActive(false);
			award1Silver.SetActive(false);
			award1Gold.SetActive(false);
			award2Gold.SetActive(false);
		}
		else
		{
			if (score.user.award1Gold > 0)
			{
				award1Gold.SetActive(true);
				award1Gold.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = score.user.award1Gold.ToString();
			}
			if (score.user.award1Silver > 0)
			{
				award1Silver.SetActive(true);
				award1Silver.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = score.user.award1Silver.ToString();
			}
			if (score.user.award1Bronze > 0)
			{
				award1Bronze.SetActive(true);
				award1Bronze.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = score.user.award1Bronze.ToString();
			}

			int numberOfWeeklyAwards = score.user.award2Gold + score.user.award2Silver + score.user.award2Bronze;

			if (numberOfWeeklyAwards > 0)
			{
				award2Gold.SetActive(true);
				award2Gold.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = numberOfWeeklyAwards.ToString();
			}
		}

		/*if (isPrizeWinner)
		{
			transform.Find("MoneyBag").gameObject.SetActive(true);
		}*/

		//Debug.Log("Monthly gold: " + score.user.award1Gold);
		//Debug.Log("Monthly silver: " + score.user.award1Silver);
		//Debug.Log("Monthly bronze: " + score.user.award1Bronze);
		//Debug.Log("Weekly awards: " + numberOfWeeklyAwards);
	}
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
