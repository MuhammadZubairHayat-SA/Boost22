using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Helpers;
using Managers.API;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeUsernameMenu : MonoBehaviour 
{
	public Animator accountOverlay;
	public TMP_InputField usernameField; 
	public TextMeshProUGUI errorText;
	public TextMeshProUGUI successText;
	public Button changeUsernameButton;
	public Button cancelButton;
	public CanvasGroup spinnner;
	public int minUsernameLength = 4;

	private bool usernameTyped = false;
	private bool usernameValid = false;


	public void ChangeUsername ()
	{
		ShowSpinner();
		UserApi.ChangeProfileData(usernameField.text, UserManager.LoggedInUser.phoneNumber, UserManager.LoggedInUser.countryCode, (success, errorType) =>
		{
			if (!success)
			{
				errorText.text = Api.ErrorMessage(errorType);
				Debug.Log(errorType);
			} 
			else
			{
				successText.text = "Username successfully updated";
				UserManager.LoggedInUser.username = usernameField.text;
				accountOverlay.GetComponent<AccountView>().UpdateUsername();
				StartCoroutine(CloseFrame());
			}
			
			HideSpinner();
		});
	}


	public void FrameOpened ()
	{
		usernameField.text = UserManager.LoggedInUser.username;
	}


	private IEnumerator CloseFrame ()
	{
		changeUsernameButton.interactable = false;
		cancelButton.interactable = false;
		yield return new WaitForSeconds(2.0f);
		accountOverlay.SetTrigger("closeChangeUsername");
		cancelButton.interactable = true;
		ClearAll();
	}


	public void UserNameValid (bool displayError)
	{
		usernameField.text = Regex.Replace(usernameField.text, @"\p{Cs}", "");

		if (usernameTyped == false && usernameField.text.Length < 1)
		{
			usernameValid = false;
		}
		else if (usernameField.text.Length < minUsernameLength)
		{
			if (displayError)
			{
				errorText.text = "User name must be at least " + minUsernameLength + " characters long";
			}
			usernameValid = false;
			usernameTyped = true;
		}
		else
		{
			ClearMessages();
			
			usernameValid = true;
			usernameTyped = true;
		}

		UpdateButtonState();
	}


	public void ShowSpinner ()
	{
		spinnner.alpha = 1;
		spinnner.blocksRaycasts = true;
	}


	public void HideSpinner ()
	{
		spinnner.alpha = 0;
		spinnner.blocksRaycasts = false;
	}


	public void ClearAll ()
	{
		usernameValid = false;
		usernameTyped = false;
		usernameField.text = string.Empty;
		changeUsernameButton.interactable = false;
		ClearMessages();
	}


	public void ClearMessages ()
	{
		errorText.text = string.Empty;
		successText.text = string.Empty;
	}


	private void UpdateButtonState ()
	{
		if (usernameValid)
		{
			changeUsernameButton.interactable = true;
		}
		else
		{
			changeUsernameButton.interactable = false;
		}
	}
}
