using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Managers.API;

public class PayoutMenu : MonoBehaviour 
{
	[SerializeField] private Animator frameAnimator;
	[SerializeField] private Animator successPrompt;
	[SerializeField] private Animator animator;
	[SerializeField] private TMP_InputField countryCodeField;
	[SerializeField] private TMP_InputField phonenumberField;
	[SerializeField] private TMP_InputField emailField;
	[SerializeField] private Button sendRequestButton;
	[SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI descriptionText;
	[SerializeField] private TextMeshProUGUI errorText;
	[SerializeField] private TextMeshProUGUI successText;
	[SerializeField] private TextMeshProUGUI accountBarBalanceText;
	[SerializeField] private int minPhoneNumberLength = 4;
	[SerializeField] private bool minPhoneNumberLengthIsMax = true;
	[SerializeField] private int minCountryCodeLength = 2;
	[SerializeField] private CanvasGroup spinner;
	[SerializeField] private GameObject payoutButton;
	[SerializeField] private GameObject noPayoutButton;

	private CanvasGroup canvasGroup;
	private bool phoneNumberTyped = false;
	private bool phoneNumberValid = false;
	private bool countryCodeTyped = false;
	private bool countryCodeValid = false;

	private bool emailTyped = false;
	private bool emailIsValid = false;


	void Start () 
	{
		canvasGroup = GetComponent<CanvasGroup>();
	}
	

	void Update () 
	{
		
	}


	public void SendRequest ()
	{
        ShowSpinner();
        var email = emailField.text;
        var paymentMethodType = emailIsValid ? Enums.PaymentMethodType.AmazonGiftCard : Enums.PaymentMethodType.MobilePay;
        var countryCode = countryCodeField.text;
        var phoneNumber = phonenumberField.text;

		sendRequestButton.interactable = false;
		cancelButton.interactable = false;
		canvasGroup.interactable = false;

        // Send payment request
        UserApi.RequestPayment(paymentMethodType, email, countryCode, phoneNumber, (success, errorType) =>
        {
            if (!success)
            {
                errorText.text = Api.ErrorMessage(errorType);
                Debug.Log(errorType);
				sendRequestButton.interactable = true;
				cancelButton.interactable = true;
				canvasGroup.interactable = true;
            }
            else
            {
				errorText.text = string.Empty;
                successPrompt.SetTrigger("open");

                if (paymentMethodType == Enums.PaymentMethodType.MobilePay)
				{
					//successText.text = "We have received your request for payout of DKK " + UserManager.LoggedInUser.realMoneyAmount.ToString("F2") + " through MobilePay. You should see the money in your account within 24 hours.";
					successText.text = "We got your request for a payout. You should have the money on your account within 24 hours.\n\nDidn’t get your money? Write to us at info@boost22.com.";
				}
				else
				{
					//successText.text = "We have received your request for payout of DKK " + UserManager.LoggedInUser.realMoneyAmount.ToString("F2") + " with Amazon Gift Card. You should receive the gift card within 24 hours.";
					successText.text = "We got your request for a payout. You should have your Amazon Gift Card within 24 hours.\n\nDidn’t you get it? Write to us at info@boost22.com.";

				}
				CloseFrame();
				
				accountBarBalanceText.text = "Balance\n<b>DKK " + 0.ToString("F2") + "</b>";
				UserManager.LoggedInUser.realMoneyAmount = 0.0f;
				payoutButton.SetActive(false);
				noPayoutButton.SetActive(true);
            }

            HideSpinner();
        });
    }


    public void OpenFrame ()
	{
		descriptionText.text = "Choose how you want to get your <b>DKK " + UserManager.LoggedInUser.realMoneyAmount.ToString("F2") + "</b>:";
	}


	public void CloseFrame ()
	{
		frameAnimator.SetBool("openGiftCard", false);
		frameAnimator.SetBool("openMobilePay", false);
		animator.SetTrigger("close");
		ClearAll();
	}


	public void OpenMobilePay ()
	{
		ClearAll();
		errorText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
		//successText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
		frameAnimator.SetBool("openGiftCard", false);
		frameAnimator.SetBool("openMobilePay", true);
	}


	public void OpenGiftCard ()
	{
		ClearAll();
		errorText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -66);
		//successText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -66);
		frameAnimator.SetBool("openMobilePay", false);
		frameAnimator.SetBool("openGiftCard", true);
	}


	public void CountryCodeValid (bool displayError)
	{
		errorText.text = string.Empty;

		if (countryCodeTyped == false && countryCodeField.text.Length < 1)
		{
			countryCodeValid = false;
		}
		else if (countryCodeField.text.Length < minCountryCodeLength)
		{
			if (displayError)
			{
				errorText.text = "Country code must be at least " + minCountryCodeLength + " numbers long";
			}
			countryCodeValid = false;
			countryCodeTyped = true;
		}
		else if (countryCodeField.text != "45" && countryCodeField.text != "045")
		{
			if (displayError)
			{
				errorText.text = "Only Danish phone numbers are eligible for payout through MobilePay. Please request an Amazon.com gift card instead.";
			}
			countryCodeValid = false;
			countryCodeTyped = true;
		}
		else
		{	
			countryCodeValid = true;
			countryCodeTyped = true;

			if (!phoneNumberValid)
			{
				PhoneNumberValid(displayError);
			}
		}

		UpdateButtonState(displayError);
	}


	public void PhoneNumberValid (bool displayError)
	{
		errorText.text = string.Empty;

		if (phoneNumberTyped == false && phonenumberField.text.Length < 1)
		{
			phoneNumberValid = false;
		}
		else if (minPhoneNumberLengthIsMax && phonenumberField.text.Length != minPhoneNumberLength)
		{
			if (displayError)
			{
				errorText.text = "Phone number must be " + minPhoneNumberLength + " numbers long";
			}
			phoneNumberValid = false;
			phoneNumberTyped = true;
		}
		else if (phonenumberField.text.Length < minPhoneNumberLength)
		{
			if (displayError)
			{
				errorText.text = "Phone number must be at least " + minPhoneNumberLength + " numbers long";
			}
			phoneNumberValid = false;
			phoneNumberTyped = true;
		}
		else
		{	
			phoneNumberValid = true;
			phoneNumberTyped = true;

			if (!countryCodeValid)
			{
				CountryCodeValid(displayError);
			}
		}
	
		UpdateButtonState(displayError);
	}


	public void EmailValid (bool displayError)
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
			emailIsValid = true;
			emailTyped = true;
		}

		UpdateButtonState(displayError);
	}


	public void ShowSpinner ()
	{
		spinner.alpha = 1;
		spinner.blocksRaycasts = true;
	}


	public void HideSpinner ()
	{
		spinner.alpha = 0;
		spinner.blocksRaycasts = false;
	}



	public void ClearAll ()
	{
		ClearEmail();
		ClearMobilePay();	
		errorText.text = string.Empty;	
	}


	private void ClearEmail ()
	{
		emailField.text = string.Empty;
	}


	private void ClearMobilePay ()
	{
		countryCodeField.text = string.Empty;
		phonenumberField.text = string.Empty;
	}


	private void UpdateButtonState (bool displayError)
	{
		if (emailIsValid || (countryCodeValid && phoneNumberValid))
		{
			sendRequestButton.interactable = true;
		}
		else
		{
			sendRequestButton.interactable = false;
		}
	}


	public void DisplayErrorMessage (string errorMessage)
	{
		errorText.text = errorMessage;
	}


	private IEnumerator CloseMenuDelayed ()
	{
		yield return new WaitForSeconds(2.0f);
		CloseFrame();

		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}	
		animator.SetTrigger("close");
	}
}
