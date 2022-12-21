using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardsLayoutGroup : MonoBehaviour 
{
	//public RectTransform[] elements;
	public float spacing = 4.0f;
	public float resizeSpeed = 2.0f;

	private List<Vector2> positions;
	private List<RectTransform> activeElements;
	private float timer = 0.0f;
	private bool resize = false;


	void Start () 
	{

	}
	
	void LateUpdate () 
	{
		if (resize == true && timer <= 1)
		{
			timer += Time.deltaTime * resizeSpeed;

			for (int i = 0; i < activeElements.Count; i++)
			{
				if (activeElements[i] != null)
				{
					activeElements[i].anchoredPosition = Vector2.Lerp(activeElements[i].anchoredPosition, positions[i], timer);
				}
			}
		}
		else if (timer > 1)
		{
			resize = false;
		}
	}


	public void ResizeSmooth ()
	{
		activeElements = GetActiveElements();
		positions = CalculatePositions(activeElements.Count, transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x);

		timer = 0.0f;
		resize = true;
	}


	public void ResizeImmediate ()
	{
		activeElements = GetActiveElements();
		positions = CalculatePositions(activeElements.Count, transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x);

		for (int i = 0; i < activeElements.Count; i++)
		{
			activeElements[i].anchoredPosition = positions[i];
		}
	}


	public List<Vector2> CalculatePositions (int numberOfElements, float elementWidth)
	{
		List<Vector2> newPositions = new List<Vector2>();

		float elementOffset = elementWidth + spacing;
		float firstXPos = numberOfElements / -2.0f * elementOffset + elementOffset * 0.5f;

		for (int i = 0; i < numberOfElements; i++)
		{
			float xPos = firstXPos + elementOffset * i;
			Vector2 newPos = new Vector2(xPos, 0);
			newPositions.Add(newPos);
		}

		return newPositions;
	}


	public List<Vector2> CalculatePositionsLeft (int numberOfElements, float elementWidth)
	{
		List<Vector2> newPositions = new List<Vector2>();

		float elementOffset = elementWidth + spacing;
		float firstXPos = elementOffset; //numberOfElements / -2.0f * elementOffset + elementOffset * 0.5f;

		for (int i = 0; i < numberOfElements; i++)
		{
			float xPos = elementOffset * i;
			Vector2 newPos = new Vector2(xPos, 0);
			newPositions.Add(newPos);
		}

		return newPositions;
	}


	public List<Vector2> CalculatePositionsRight (int numberOfElements, float elementWidth)
	{
		List<Vector2> newPositions = new List<Vector2>();

		float elementOffset = elementWidth + spacing;
		float firstXPos = -elementOffset * numberOfElements; //numberOfElements / -2.0f * elementOffset + elementOffset * 0.5f;

		for (int i = 0; i < numberOfElements; i++)
		{
			float xPos = -elementOffset * i;
			Vector2 newPos = new Vector2(xPos, 0);
			newPositions.Add(newPos);
		}

		return newPositions;
	}


	private List<RectTransform> GetActiveElements ()
	{
		List<RectTransform> newElements = new List<RectTransform>();

		foreach (RectTransform element in transform)
		{
			if (element.gameObject.activeSelf == true)
			{
				newElements.Add(element);
			}
		}

		return newElements;
	}
}
