using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Facebook.Unity;

public class LobbyGuide : MonoBehaviour 
{
	private Animator animator;


	public void OpenGuideView ()
	{
		StartCoroutine(OpenGuideViewDelayed());
	}


	public void CloseGuideView ()
	{
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}

		animator.SetTrigger("close");
	}


	private IEnumerator OpenGuideViewDelayed ()
	{
		yield return new WaitForSeconds(1.0f);

		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}

		animator.SetTrigger("open");
	}


	public void Play()
	{
		Handheld.PlayFullScreenMovie("boost22_how_to_play.mp4", Color.black, FullScreenMovieControlMode.Full);
	}


/**
 * Include the Facebook namespace via the following code:
 * using Facebook.Unity;
 *
 * For more details, please take a look at:
 * developers.facebook.com/docs/unity/reference/current/FB.LogAppEvent
 */
    public void LogCompleteTutorialEvent(string contentType /*string contentData, string contentId, bool success*/)
    {
        var parameters = new Dictionary<string, object>();
        //parameters[AppEventParameterName.Content] = contentData;
        parameters[AppEventParameterName.ContentID] = contentType; //contentId;
        parameters[AppEventParameterName.Success] = 1; /* success ? 1 : 0;*/
        FB.LogAppEvent(AppEventName.CompletedTutorial, 0.0f, parameters);
    }


    public void LogViewContentEvent (string contentType) 
	{
		var parameters = new Dictionary<string, object>();
    	//parameters[AppEventParameterName.Content] = contentData;
		parameters[AppEventParameterName.ContentType] = contentType;
    	parameters[AppEventParameterName.ContentID] = contentType;
		parameters[AppEventParameterName.Currency] = "0 USD";

		FB.LogAppEvent(AppEventName.ViewedContent, 0.0f, parameters);
	}
}
