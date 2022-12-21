using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;
using TMPro;
using Managers.API;

public class WinnersMenu : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
	[SerializeField] private GameObject listView;
	[SerializeField] private GameObject spinner;
	[SerializeField] private CanvasGroup refreshGraphics;

    private bool isRefreshing = false;

    
    private void OnEnable ()
    {
        isRefreshing = true;
        spinner.SetActive(true);
        ClearList();

        ScoreApi.GetWinners((winners, type) =>
        {
            SetupList(winners);
        });
    }


    private void SetupList(Winners winners)
	{
		ClearList();

		bool userIsOnList = false;
        char decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];

        foreach (Winner winner in winners.winners)
        {				
            var cell = Instantiate(cellPrefab, listView.transform);
            TextMeshProUGUI numberText = cell.transform.Find("NumberText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI nameText = cell.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI prizeText = cell.transform.Find("PrizeText").GetComponent<TextMeshProUGUI>();
            
            numberText.text = winner.ranking + ".";
            nameText.text = winner.username;
            string[] gameScore = winner.gameScore.ToString("F2").Split(decimalSeparator);
            prizeText.text = "<mspace=22>" + gameScore[0] + "</mspace>" + decimalSeparator + "<mspace=22>" + gameScore[1] + "</mspace>";

            if (winner.userId == UserManager.LoggedInUser.id)
            {
                userIsOnList = true;
                ProceduralImage cellBG = cell.transform.Find("CellBackground").GetComponent<ProceduralImage>();
                //nameText.color = cellBG.color;
                //prizeText.color = cellBG.color;
                //cellBG.color = new Color32(0xF4, 0xB7, 0x3A, 0xFF);
            }
        }

        if (!userIsOnList)
        {
            var emptyCell = Instantiate(cellPrefab, listView.transform);
            emptyCell.transform.Find("NumberText").GetComponent<TextMeshProUGUI>().text = string.Empty;
            emptyCell.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = "...";
            emptyCell.transform.Find("PrizeText").GetComponent<TextMeshProUGUI>().text = string.Empty;

            var cell = Instantiate(cellPrefab, listView.transform);
            TextMeshProUGUI numberText = cell.transform.Find("NumberText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI nameText = cell.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI prizeText = cell.transform.Find("PrizeText").GetComponent<TextMeshProUGUI>();
            
            numberText.text = winners.userWinner.ranking + ".";
            nameText.text = winners.userWinner.username;
            string[] gameScore = winners.userWinner.gameScore.ToString("F2").Split(decimalSeparator);
            prizeText.text = "<mspace=22>" + gameScore[0] + "</mspace>" + decimalSeparator + "<mspace=22>" + gameScore[1] + "</mspace>";

            ProceduralImage cellBG = cell.transform.Find("CellBackground").GetComponent<ProceduralImage>();
            //nameText.color = cellBG.color;
            //prizeText.color = cellBG.color;
            //cellBG.color = new Color32(0xF4, 0xB7, 0x3A, 0xFF);
        }

        GameObject invisibleCell = Instantiate(cellPrefab, listView.transform);
        foreach (Transform child in invisibleCell.transform)
        {
            child.gameObject.SetActive(false);
        }
		
		spinner.SetActive(false);
		isRefreshing = false;
	}


	private void ClearList()
	{
		var listChildCount = listView.transform.childCount;
		for (var i = listChildCount - 1; i >= 0; i--)
		{
			Destroy(listView.transform.GetChild(i).gameObject);
		}
	}


    private RectTransform listViewTransform;
	private void Update ()
	{
		if (listViewTransform == null)
		{
			listViewTransform = listView.GetComponent<RectTransform>();
		}

		if (Input.GetMouseButton(0) && listViewTransform.anchoredPosition.y < -25)
		{
			refreshGraphics.alpha = Mathf.Clamp01(-1 * (listViewTransform.anchoredPosition.y + 25) / 100.0f);

			if (isRefreshing)
			{
				refreshGraphics.transform.eulerAngles = Vector3.zero;
			}
			else
			{
				refreshGraphics.transform.eulerAngles = new Vector3(0, 0, 180 * (1 - refreshGraphics.alpha));
			}
		}
		else if (refreshGraphics.alpha != 0)
		{
			refreshGraphics.alpha = 0;
			refreshGraphics.transform.eulerAngles = Vector3.zero;
		}

		if (Input.GetMouseButtonUp(0) && listViewTransform.anchoredPosition.y < -100)
		{
            isRefreshing = true;
            spinner.SetActive(true);
            ClearList();
			ScoreApi.GetWinners((winners, type) =>
            {
                SetupList(winners);
            });
		}
	}
}
