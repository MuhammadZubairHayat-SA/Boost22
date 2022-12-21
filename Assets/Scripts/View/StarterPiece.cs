using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterPiece : MonoBehaviour 
{
	public AnimationCurve moveCurve;
	public AnimationCurve arcCurve;
	public RectTransform shadow;
	public float speed = 1.0f;

	private AudioSource audioSource;
	private CanvasGroup canvasGroup;
	private Vector3 fromPosition;
	private Vector3 toPosition;
	private float distance;
	private float dstTravelled;
	private float alpha = 0.0f;
	private bool move = false;
	private bool show = false;


	void Start () 
	{
		audioSource = GetComponent<AudioSource>();
		canvasGroup = GetComponent<CanvasGroup>();
	}
	

	void Update () 
	{
		if (move == true && dstTravelled <= 1)
		{
			dstTravelled += Time.deltaTime * speed;
			Vector3 moveDirection = Vector3.Lerp(fromPosition, toPosition, moveCurve.Evaluate(dstTravelled));
			moveDirection +=  transform.right * arcCurve.Evaluate(dstTravelled) * distance * 0.1f;
			transform.position = moveDirection;
			transform.up = (fromPosition - toPosition).normalized;

			shadow.eulerAngles = Vector3.zero;
		}

		if (show == true && canvasGroup.alpha < 1)
		{
			alpha += Time.deltaTime;
			canvasGroup.alpha = alpha;
		}
		else if (show == false && canvasGroup.alpha > 0)
		{
			alpha -= Time.deltaTime;
			canvasGroup.alpha = alpha;
		}
	}


	public IEnumerator Show (float delay)
	{
		yield return new WaitForSeconds(delay);
		show = true;
	}


	public IEnumerator Hide (float delay)
	{
		yield return new WaitForSeconds(delay);
		show = false;
	}


	public IEnumerator Move (Vector3 from, Vector3 to, float delay = 0.0f)
	{
		yield return new WaitForSeconds(delay);

		if (from != to)
		{
			move = true;
			fromPosition = from;
			toPosition = to;
			dstTravelled = 0;
			distance = Vector3.Distance(from, to);
			audioSource.Play();
		}
	}
}
