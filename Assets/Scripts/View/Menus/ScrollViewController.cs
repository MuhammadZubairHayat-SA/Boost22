using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour 
{
	[SerializeField] private RectTransform content;
	

	private void OnEnable ()
	{
		if (content != null)
		{
			content.anchoredPosition = Vector2.zero;
		}
	}
}
