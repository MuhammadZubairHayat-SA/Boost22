using System.Collections;
using System.Collections.Generic;
using Helpers;
using Managers.API;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangePhoneNumberMenu : MonoBehaviour 
{

	public Animator accountOverlay;
	public TMP_InputField countryCodeField; 
	public TMP_InputField phoneNumberField; 
	public TextMeshProUGUI errorText;
	public TextMeshProUGUI successText;
	public Button changePhoneNumberButton;
	public Button cancelButton;
	public CanvasGroup spinnner;
	public int minPhoneNumberLength = 4;
	public int minCountryCodeLength = 2;

	private bool phoneNumberTyped = false;
	private bool phoneNumberValid = false;
	private bool countryCodeTyped = false;
	private bool countryCodeValid = false;


	public void ChangePhoneNumber ()
	{
		ShowSpinner();
		// Send phone number and country code to server
		UserApi.ChangeProfileData(UserManager.LoggedInUser.username, phoneNumberField.text, countryCodeField.text, (success, errorType) =>
		{
			if (!success)
			{
				errorText.text = Api.ErrorMessage(errorType);
				Debug.Log(errorType);
			} 
			else
			{
				successText.text = "Phone number successfully updated";
				UserManager.LoggedInUser.countryCode = countryCodeField.text;
				UserManager.LoggedInUser.phoneNumber = phoneNumberField.text;
				accountOverlay.GetComponent<AccountView>().UpdatePhoneNumber();
				StartCoroutine(CloseFrame());
			}
			
			HideSpinner();
		});
	}


	public void FrameOpened ()
	{
		if (!string.IsNullOrEmpty(UserManager.LoggedInUser.phoneNumber))
		{
			phoneNumberField.text = UserManager.LoggedInUser.phoneNumber;
		}

		if (!string.IsNullOrEmpty(UserManager.LoggedInUser.countryCode))
		{
			countryCodeField.text = UserManager.LoggedInUser.countryCode;
		}
	}


	private IEnumerator CloseFrame ()
	{
		changePhoneNumberButton.interactable = false;
		cancelButton.interactable = false;
		yield return new WaitForSeconds(2.0f);
		accountOverlay.SetTrigger("closeChangePhoneNumber");
		cancelButton.interactable = true;
		ClearAll();
	}

	
	public void CountryCodeValid (bool displayError)
	{
		if (countryCodeTyped == false && countryCodeField.text.Length < 1)
		{
			countryCodeValid = false;
		}
		else if (countryCodeField.text.Length < minCountryCodeLength)
		{
			if (displayError)
			{
				errorText.text = "Country code must be at least " + minCountryCodeLength + " digits";
			}
			countryCodeValid = false;
			countryCodeTyped = true;
		}
		else
		{
			ClearMessages();
			
			countryCodeValid = true;
			countryCodeTyped = true;
		}

		UpdateButtonState();
	}


	public void PhoneNumberValid (bool displayError)
	{
		if (phoneNumberTyped == false && phoneNumberField.text.Length < 1)
		{
			phoneNumberValid = false;
		}
		else if (phoneNumberField.text.Length < minPhoneNumberLength)
		{
			if (displayError)
			{
				errorText.text = "Phone number must be at least " + minPhoneNumberLength + " digits";
			}
			phoneNumberValid = false;
			phoneNumberTyped = true;
		}
		else
		{
			ClearMessages();
			
			phoneNumberValid = true;
			phoneNumberTyped = true;
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
		phoneNumberValid = false;
		phoneNumberTyped = false;
		countryCodeTyped = false;
		countryCodeValid = false;
		phoneNumberField.text = string.Empty;
		countryCodeField.text = string.Empty;
		changePhoneNumberButton.interactable = false;
		ClearMessages();
	}


	public void ClearMessages ()
	{
		errorText.text = string.Empty;
		successText.text = string.Empty;
	}


	private void UpdateButtonState ()
	{
		if (phoneNumberValid && countryCodeValid)
		{
			changePhoneNumberButton.interactable = true;
		}
		else
		{
			changePhoneNumberButton.interactable = false;
		}
	}
}
