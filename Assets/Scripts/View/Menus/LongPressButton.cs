using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongPressButton : MonoBehaviour 
{
	[SerializeField] private float longPressDuration = 1.0f;
	[SerializeField] private Animator animatorToActivate;

	private float timer = 0.0f;
	private bool isPressing = false;


	void Update () 
	{
		if (timer >= longPressDuration && isPressing)
		{
			EndLongPress();
		}
		
		if (isPressing)
		{
			timer += Time.deltaTime;
		}
	}


	public void StartLongPress ()
	{
		isPressing = true;
		timer = 0.0f;
	}


	public void EndLongPress ()
	{
		if (timer > longPressDuration)
		{
			animatorToActivate.SetTrigger("open");
		}

		isPressing = false;
		timer = 0.0f;
	}
}
