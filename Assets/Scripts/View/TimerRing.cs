using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerRing : MonoBehaviour 
{
	public Color normalColor = Color.white;
	public Color runningOutColor = Color.red;
	public bool tickWhenRunningOut = false;

	private AudioSource audioSource;
	private Coroutine timerRoutine;
	private Image timerImage;
	private float time;
	private float timer;
	private bool countDown = false;


	void Start () 
	{
		if (tickWhenRunningOut)
		{
			audioSource = GetComponent<AudioSource>();
		}

		timerImage = GetComponent<Image>();
		timerImage.fillAmount = 0;
	}
	

	void Update () 
	{
		/*if (countDown == true && timer > 0)
		{
			timer -= Time.deltaTime;
			timerImage.fillAmount = timer / time;

			if (timer < time * 0.25f)
			{
				timerImage.color = runningOutColor;
				if (tickWhenRunningOut)
				{
					if (audioSource.isPlaying == false)
					{
						audioSource.Play();
					}
				}	
			}
		}
		else if (countDown == true)
		{
			countDown = false;
			timer = 0;
			if (tickWhenRunningOut)
			{
				audioSource.Stop();
			}			
		}*/
	}


	public void StartTimer (float duration, float delay)
	{
		timerRoutine = StartCoroutine(StartTimerDelayed(duration, delay));
	}


	public IEnumerator StartTimerDelayed (float duration, float delay)
	{
		yield return new WaitForSeconds(delay);

		countDown = true;
		time = duration;
		timer = duration;
		timerImage.color = normalColor;
	}


	public void StopTimer ()
	{
		if (timerRoutine != null)
		{
			StopCoroutine(timerRoutine);
		}

		countDown = false;
		time = 0;
		timer = 0;
		timerImage.fillAmount = 0;
		timerImage.color = normalColor;
		if (tickWhenRunningOut)
		{
			audioSource.Stop();
		}
	}
}
