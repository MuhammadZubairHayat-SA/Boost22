using System.Collections;
using System.Collections.Generic;
using Helpers;
using Managers.API;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangePasswordMenu : MonoBehaviour 
{
	public Animator accountOverlay;
	public TMP_InputField oldPasswordField; 
	public TMP_InputField newPasswordField; 
	public TextMeshProUGUI shownOldPasswordText;
	public TextMeshProUGUI hiddenOldPasswordText;
	public TextMeshProUGUI shownNewPasswordText;
	public TextMeshProUGUI hiddenNewPasswordText;
	public TextMeshProUGUI errorText;
	public TextMeshProUGUI successText;
	public Button changePasswordButton;
	public Button cancelButton;
	public CanvasGroup spinnner;
	public int minPasswordLength = 6;

	private bool oldPasswordTyped = false;
	private bool newPasswordTyped = false;
	private bool oldPasswordValid = false;
	private bool newPasswordValid = false;


	public void ChangePassword ()
	{
		ShowSpinner();
		UserApi.ChangePassword(oldPasswordField.text, newPasswordField.text, (user, errorType) =>
		{
			if (errorType != Enums.NetworkErrorType.NoError)
			{
				errorText.text = Api.ErrorMessage(errorType);
				Debug.Log(errorType);
				HideSpinner();
			} 
			else
			{
				successText.text = "Password successfully updated";
				StartCoroutine(CloseFrame());
				HideSpinner();
			}
		});
	}


	private IEnumerator CloseFrame ()
	{
		changePasswordButton.interactable = false;
		cancelButton.interactable = false;
		yield return new WaitForSeconds(2.0f);
		accountOverlay.SetTrigger("closeChangePassword");
		cancelButton.interactable = true;
		ClearAll();
	}


	public void OldPasswordValid ()
	{
		if (oldPasswordTyped == false && oldPasswordField.text.Length < 1)
		{
			oldPasswordValid = false;
		}
		else
		{
			oldPasswordValid = true;
			oldPasswordTyped = true;
		}

		UpdateButtonState();
	}


	public void NewPasswordValid (bool displayError)
	{
		if (newPasswordTyped == false && newPasswordField.text.Length < 1)
		{
			newPasswordValid = false;
		}
		else if (newPasswordField.text.Length < minPasswordLength)
		{
			if (displayError)
			{
				errorText.text = "The new password must be at least " + minPasswordLength + " characters long";
			}
			newPasswordValid = false;
			newPasswordTyped = true;
		}
		else
		{
			newPasswordValid = true;
			newPasswordTyped = true;
			ClearMessages();
		}

		UpdateButtonState();
	}


	public void ShowOldPassword (bool show)
	{
		if (show)
		{
			shownOldPasswordText.text = oldPasswordField.text;

			shownOldPasswordText.enabled = true;
			hiddenOldPasswordText.enabled = false;
		}
		else
		{
			shownOldPasswordText.text = string.Empty;
			
			shownOldPasswordText.enabled = false;
			hiddenOldPasswordText.enabled = true;
		}
	}


	public void ShowNewPassword (bool show)
	{
		if (show)
		{
			shownNewPasswordText.text = newPasswordField.text;

			shownNewPasswordText.enabled = true;
			hiddenNewPasswordText.enabled = false;
		}
		else
		{
			shownNewPasswordText.text = string.Empty;
			
			shownNewPasswordText.enabled = false;
			hiddenNewPasswordText.enabled = true;
		}
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
		oldPasswordValid = false;
		newPasswordValid = false;
		oldPasswordTyped = false;
		newPasswordTyped = false;
		oldPasswordField.text = string.Empty;
		newPasswordField.text = string.Empty;
		changePasswordButton.interactable = false;
		ClearMessages();
	}


	public void ClearMessages ()
	{
		errorText.text = string.Empty;
		successText.text = string.Empty;
	}


	private void UpdateButtonState ()
	{
		if (oldPasswordValid && newPasswordValid)
		{
			changePasswordButton.interactable = true;
		}
		else
		{
			changePasswordButton.interactable = false;
		}
	}


	public void DisplayErrorMessage (string errorMessage)
	{
		errorText.text = errorMessage;
	}
}
