using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaygroundMenu : MonoBehaviour
{
    [SerializeField] private RectTransform playNowButtonFrame;
    [SerializeField] private Image playTab;
    [SerializeField] private Image howToPlayTab;
    [SerializeField] private Image gameModesTab;
    [SerializeField] private Image pointsTab;
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject howToPlayMenu;
    [SerializeField] private GameObject gameModesMenu;
    [SerializeField] private GameObject pointsMenu;
    [SerializeField] private Color selectedColor = Color.cyan;
    [SerializeField] private Color unselectedColor = Color.blue;


    void Start()
    {
        float aspectRatio = Screen.width / (float)Screen.height;
		playNowButtonFrame.localScale *= 1.25f / ((aspectRatio / 1.778f) * 1.33f);
    }


    public void SelectTab (int tab)
	{
		if (playTab != null)
		{
			if (tab == 0)
			{
				playTab.color = selectedColor;
                playMenu.SetActive(true);
			}
			else
			{
				playTab.color = unselectedColor;
                playMenu.SetActive(false);
			}
		}

		if (howToPlayTab != null)
		{
			if (tab == 1)
			{
				howToPlayTab.color = selectedColor;
                howToPlayMenu.SetActive(true);
			}
			else
			{
				howToPlayTab.color = unselectedColor;
                howToPlayMenu.SetActive(false);
			}
		}

		if (gameModesTab != null)
		{
			if (tab == 2)
			{
				gameModesTab.color = selectedColor;
                gameModesMenu.SetActive(true);
			}
			else
			{
				gameModesTab.color = unselectedColor;
                gameModesMenu.SetActive(false);
			}
		}

		if (pointsTab != null)
		{
			if (tab == 3)
			{
				pointsTab.color = selectedColor;
                pointsMenu.SetActive(true);
			}
			else
			{
				pointsTab.color = unselectedColor;
                pointsMenu.SetActive(false);
			}
		}
    }
}
