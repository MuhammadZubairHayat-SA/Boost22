using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Helpers;
using Managers.API;

public class InviteNewPlayerMenu : MonoBehaviour 
{
	public Animator accountOverlay;
	public CanvasGroup spinnner;
	public ToggleGroup toggleGroup;
	public string englishInvitationString;
	public string danishInvitationString;
	public Button sendEmailButton;
	public Button cancelButton;
	public TextMeshProUGUI errorText;
	public TextMeshProUGUI successText;

	private Coroutine closeFrameRoutine;
	private bool danishSelected = false;
	private bool emailIsValid = false;
	private bool emailTyped = false;


	/*private void OnEnable ()
	{
		if (UserManager.LoggedInUser != null)
		{
			inviterNameField.text = UserManager.LoggedInUser.username;
		}
	}


	public void SendInvite ()
	{
		InvitationApi.Send(emailField.text, inviterNameField.text, friendNameField.text, (success, type) =>
		{
			if (!success)
			{
				DisplayErrorMessage(Api.ErrorMessage(type));
			}
			else
			{
				successText.text = "Invitation successfully sent to " + emailField.text;
				StartCoroutine(CloseFrame());
			}
			
			HideSpinner();
		});
	}*/
	
	
	public void SelectEnglish ()
	{
		//sendEmailButton.interactable = true;
		danishSelected = false;
	}


	public void SelectDanish ()
	{
		//sendEmailButton.interactable = true;
		danishSelected = true;
	}


	public void Share()
    {
        NativeShare nativeShare = new NativeShare();
		string shareText = string.Empty;
		
        nativeShare.SetSubject("BOOST22 Invitation"); // Email
		#if UNITY_IOS
        	shareText = (danishSelected ? danishInvitationString : englishInvitationString); // Not Facebook
#elif UNITY_ANDROID
        	nativeShare.SetTitle("BOOST22 Invitation"); // Not iOS
        	shareText = danishSelected ? danishInvitationString : englishInvitationString; // Not Facebook
#endif

		// shareText += " http://ec2-3-121-201-86.eu-central-1.compute.amazonaws.com:9191/invite/" + (danishSelected ? "da/" : "en/") + UserManager.LoggedInUser.inviteToken; // development
		//shareText += " https://api2.cardgame.boost22.com/invite/" + (danishSelected ? "da/" : "en/") + UserManager.LoggedInUser.inviteToken; // production
		shareText += " play.boost22.com/?invite=" + (danishSelected ? "da" : "en") + UserManager.LoggedInUser.inviteToken; // production

		nativeShare.SetText(shareText);
        //nativeShare.AddFile(Path.Combine(Application.streamingAssetsPath, "image.png"));
        //nativeShare.SetTarget( string androidPackageName, string androidClassName = null ): shares content on a specific application on Android platform. If androidClassName is left null, list of activities in the share dialog will be narrowed down to the activities in the specified androidPackageName that can handle this share action (if there is only one such activity, it will be launched directly). Note that androidClassName, if provided, must be the full name of the activity (with its package). This function has no effect on iOS

        nativeShare.Share();
    }


	/*public void ShowSpinner ()
	{
		spinnner.alpha = 1;
		spinnner.blocksRaycasts = true;
	}


	public void HideSpinner ()
	{
		spinnner.alpha = 0;
		spinnner.blocksRaycasts = false;
	}*/


	/*public void EmailValid (bool displayError)
	{
		if (emailTyped == false && emailField.text.Length < 1)
		{
			emailIsValid = false;
		}
		else if (!emailField.text.Contains("@") || !emailField.text.Contains("."))
		{
			if (displayError)
			{
				DisplayErrorMessage("Please, input a valid e-mail address");
			}
			emailIsValid = false;
			emailTyped = true;
		}
		else
		{
			ClearMessages();
			
			emailIsValid = true;
			emailTyped = true;
		}

		UpdateSendButtonState(displayError);
	}*/

	
	private IEnumerator CloseFrame ()
	{
		//sendEmailButton.interactable = false;
		cancelButton.interactable = false;
		yield return new WaitForSeconds(2.0f);
		accountOverlay.SetTrigger("closeInvite");
		cancelButton.interactable = true;
		ClearAll();
	}


	private void UpdateSendButtonState (bool displayError)
	{
		if (emailIsValid)
		{
			sendEmailButton.interactable = true;
		}
		else
		{
			sendEmailButton.interactable = false;
		}
	}


	public void ClearAll ()
	{
		//emailField.text = string.Empty;
		//inviterNameField.text = string.Empty;
		//friendNameField.text = string.Empty;
		foreach (Toggle toggle in toggleGroup.ActiveToggles())
		{
			toggle.isOn = false;
		}
		danishSelected = false;
		sendEmailButton.interactable = false;
		ClearMessages();
	}


	public void ClearMessages ()
	{
		errorText.text = string.Empty;
		successText.text = string.Empty;
	}


	public void DisplayErrorMessage (string errorMessage)
	{
		errorText.text = errorMessage;
	}


	public void OpenSupport ()
    {
		string emailAddress = "support@boost22.com";
		string subject = "Boost22 Support - Extra games";
		string body = "Sent from Boost22 app";
		
    	Application.OpenURL("mailto:" + emailAddress + "?subject=" + subject + "&body=" + body);
    }
}
