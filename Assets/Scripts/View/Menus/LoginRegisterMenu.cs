using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Helpers;
using Managers.API;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Facebook.Unity;

public class LoginRegisterMenu : MonoBehaviour 
{
	[SerializeField] private Animator screensAnimator;
	[SerializeField] private Animator animator;
	[SerializeField] private CanvasGroup spinnner;
	[SerializeField] private TMP_InputField emailField;
	[SerializeField] private TMP_InputField userNameRegisterField;
	[SerializeField] private TMP_InputField passwordRegisterField;
	[SerializeField] private TMP_InputField passwordLoginField;
	[SerializeField] private TextMeshProUGUI shownRegisterPasswordText;
	[SerializeField] private TextMeshProUGUI hiddenRegisterPasswordText;
	[SerializeField] private TextMeshProUGUI shownLoginPasswordText;
	[SerializeField] private TextMeshProUGUI hiddenLoginPasswordText;
	[SerializeField] private Toggle acceptTermsToggle;
	[SerializeField] private Button loginButton;
	[SerializeField] private Button registerButton;
	[SerializeField] private Button submitEmailButton;
	[SerializeField] private Button facebookButton;
	[SerializeField] private TextMeshProUGUI loginErrorText;
	[SerializeField] private TextMeshProUGUI registerErrorText;
	[SerializeField] private TextMeshProUGUI forgotPasswordErrorText;
	[SerializeField] private TextMeshProUGUI forgotPasswordSuccessText;
	[SerializeField] private GameObject leagueLobbyObject;
	[SerializeField] private GameObject rush22LobbyObject;
	[SerializeField] private GameObject playgroundLobbyObject;
	[SerializeField] private GameObject winnersLobbyObject;
	[SerializeField] private LobbyGuide guideView;
	[SerializeField] private int minUsernameLength = 4;
	[SerializeField] private int minPasswordLength = 8;

	private Coroutine closeFrameRoutine;
	private bool emailIsValid = false;
	private bool userNameIsValid = false;
	private bool registerPasswordIsValid = false;
	private bool loginPasswordIsValid = false;
	private bool emailTyped = false;
	private bool userNameTyped = false;
	private bool registerPasswordTyped = false;
	private bool loginPasswordTyped = false;
	public bool termsAccepted = false;

	void Start ()
	{
		#if UNITY_IOS
			facebookButton.gameObject.SetActive(false);
		#endif
	}
	

	void Update () 
	{
		
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


	public void Register ()
	{
		UserApi.Create(userNameRegisterField.text, emailField.text, passwordRegisterField.text, Functions.GetInviteTokenFromClipboard(), (user, errorType) =>
		{
			Debug.Log("CREATE");
			UserManager.LoggedInUser = user;
			LogCompleteRegistrationEvent("In-App");
			ProceedToLobby(showGuide : true);
	//		if (errorType != Enums.NetworkErrorType.NoError)
	//		{
	//			DisplayErrorMessage(Api.ErrorMessage(errorType), 0);
	//			Debug.Log(errorType);
	//			HideSpinner();
	//		} else
	//		{
	//			UserManager.LoggedInUser = user;
	//			LogCompleteRegistrationEvent("In-App");
	//			ProceedToLobby(showGuide : true);
	//		}
		});
	}


	public void Login ()
	{
		UserApi.Login(emailField.text, passwordLoginField.text, (user, errorType) =>
		{
			if (errorType != Enums.NetworkErrorType.NoError)
			{
				ProceedToLobby();
				DisplayErrorMessage(Api.ErrorMessage(errorType), 1);
				Debug.Log(errorType);
				HideSpinner();
			} else
			{
				UserManager.LoggedInUser = user;
                ProceedToLobby();
			}
		});
	}


	public void ForgotPassword()
	{
		ShowSpinner();

		UserApi.ForgotPassword(emailField.text, (success, type) =>
		{
			if (success)
			{
				// Success
				// TODO: Vis en alert? Der er sendt en mail og gå til log ind visning.
				forgotPasswordSuccessText.gameObject.SetActive(true);
				closeFrameRoutine = StartCoroutine(CloseForgotPasswordFrameDelayed());
				HideSpinner();
			}
			else
			{
				// Email not found
				DisplayErrorMessage(Api.ErrorMessage(Enums.NetworkErrorType.EmailNotFound), 2);
				HideSpinner();
			}
		});
	}


	public void ShowForgotPasswordFrame ()
	{
		HideSpinner();
		registerPasswordTyped = false;
		loginPasswordTyped = false;
		ClearErrorMessages();
		ClearPasswordFields();
		forgotPasswordSuccessText.gameObject.SetActive(false);
	}


	public void CloseForgotPasswordFrame ()
	{
		if (closeFrameRoutine != null)
		{
			StopCoroutine(closeFrameRoutine);
		}
		forgotPasswordSuccessText.gameObject.SetActive(false);
	}


	private IEnumerator CloseForgotPasswordFrameDelayed ()
	{
		yield return new WaitForSeconds(2.0f);
		forgotPasswordSuccessText.gameObject.SetActive(false);
		animator.SetTrigger("closeForgotPassword");
	}


	public void ShowRegisterPassword (bool show)
	{
		if (show)
		{
			shownRegisterPasswordText.text = passwordRegisterField.text;

			shownRegisterPasswordText.enabled = true;
			hiddenRegisterPasswordText.enabled = false;
		}
		else
		{
			shownRegisterPasswordText.text = string.Empty;
			
			shownRegisterPasswordText.enabled = false;
			hiddenRegisterPasswordText.enabled = true;
		}
	}


	public void ShowLoginPassword (bool show)
	{
		if (show)
		{
			shownLoginPasswordText.text = passwordLoginField.text;

			shownLoginPasswordText.enabled = true;
			hiddenLoginPasswordText.enabled = false;
		}
		else
		{
			shownLoginPasswordText.text = string.Empty;
			
			shownLoginPasswordText.enabled = false;
			hiddenLoginPasswordText.enabled = true;
		}
	}


	public void SwitchTab ()
	{
		animator.SetBool("isOnLoginTab", !animator.GetBool("isOnLoginTab"));
		emailTyped = false;
		userNameTyped = false;
		registerPasswordTyped = false;
		loginPasswordTyped = false;
		ClearErrorMessages();
		ClearPasswordFields();
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
				DisplayErrorMessage("Please, input a valid e-mail address", 3);
			}
			emailIsValid = false;
			emailTyped = true;
		}
		else
		{
			ClearErrorMessages();
			if (loginPasswordIsValid == false)
			{
				LoginPasswordValid(displayError);
			}
			else if (registerPasswordIsValid == false)
			{
				RegisterPasswordValid(displayError);
			}
			else if (userNameIsValid == false)
			{
				UserNameValid(displayError);
			}
			emailIsValid = true;
			emailTyped = true;
		}

		UpdateRegisterButtonState(displayError);
		UpdateLoginButtonState(displayError);
		UpdateSubmitButtonState(displayError);
	}


	public void UserNameValid (bool displayError)
	{
		userNameRegisterField.text = Regex.Replace(userNameRegisterField.text, @"\p{Cs}", "");

		if (userNameTyped == false && userNameRegisterField.text.Length < 1)
		{
			userNameIsValid = false;
		}
		else if (userNameRegisterField.text.Length < minUsernameLength)
		{
			if (displayError)
			{
				DisplayErrorMessage("User name must be at least " + minUsernameLength + " characters long", 0);
			}
			userNameIsValid = false;
			userNameTyped = true;
		}
		else
		{
			ClearErrorMessages();
			if (registerPasswordIsValid == false)
			{
				RegisterPasswordValid(displayError);
			}
			else if (emailIsValid == false)
			{
				EmailValid(displayError);
			}
			userNameIsValid = true;
			userNameTyped = true;
		}

		print("User name typed: " + userNameTyped);
		UpdateRegisterButtonState(displayError);
	}


	public void LoginPasswordValid (bool displayError)
	{
		if (loginPasswordTyped == false && passwordLoginField.text.Length < 1)
		{
			loginPasswordIsValid = false;
		}
		else if (passwordLoginField.text.Length < minPasswordLength)
		{
			if (displayError)
			{
				DisplayErrorMessage("Password must be at least " + minPasswordLength + " characters long", 1);
			}
			loginPasswordIsValid = false;
			loginPasswordTyped = true;
		}
		else
		{
			ClearErrorMessages();
			if (emailIsValid == false)
			{
				EmailValid(displayError);
			}
			loginPasswordIsValid = true;
			loginPasswordTyped = true;
		}

		UpdateLoginButtonState(displayError);
	}


	public void RegisterPasswordValid (bool displayError)
	{
		if (registerPasswordTyped == false && passwordRegisterField.text.Length < 1)
		{
			registerPasswordIsValid = false;
		}
		else if (passwordRegisterField.text.Length < minPasswordLength)
		{
			if (displayError)
			{
				DisplayErrorMessage("Password must be at least " + minPasswordLength + " characters long", 0);
			}
			registerPasswordIsValid = false;
			registerPasswordTyped = true;
		}
		else
		{
			ClearErrorMessages();
			if (userNameIsValid == false)
			{
				UserNameValid(displayError);
			}
			else if (emailIsValid == false)
			{
				EmailValid(displayError);
			}
			registerPasswordIsValid = true;
			registerPasswordTyped = true;
		}

		UpdateRegisterButtonState(displayError);
	}


	public void TermsAccepted (bool displayError)
	{
		termsAccepted = acceptTermsToggle.isOn;

		if (acceptTermsToggle.isOn == false)
		{
			DisplayErrorMessage("You must accept the terms to register", -1);
		}
		else
		{
			ClearErrorMessages();
			if (loginPasswordIsValid == false)
			{
				LoginPasswordValid(displayError);
			}
			else if (registerPasswordIsValid == false)
			{
				RegisterPasswordValid(displayError);
			}
			else if (userNameIsValid == false)
			{
				UserNameValid(displayError);
			}
			else if (emailIsValid == false)
			{
				EmailValid(displayError);
			}
		}

		UpdateRegisterButtonState(displayError);
		UpdateLoginButtonState(displayError);
	}


	public void TrimUsername ()
	{
		char[] charsToTrim = { ' '};
		userNameRegisterField.text = userNameRegisterField.text.Trim(charsToTrim);
	}


	public void ProceedToLobby (bool showGuide = false)
	{
		screensAnimator.SetTrigger("toLobby");
		HideSpinner();
		registerPasswordTyped = false;
		loginPasswordTyped = false;
		ClearAll();

		if (showGuide)
		{
			guideView.OpenGuideView();
            screensAnimator.GetComponent<Menus>().SelectLobbyMenu(2);
        }
		else
		{
            screensAnimator.GetComponent<Menus>().SelectLobbyMenu(0);
        }

		PlayerPrefs.SetInt("NotFirstAppLaunch", 1);
	}

	private void UpdateLoginButtonState (bool displayError)
	{
		if (emailIsValid && loginPasswordIsValid && termsAccepted)
		{
			loginButton.interactable = true;
		}
		else
		{
			loginButton.interactable = false;
		}
	}


	private void UpdateRegisterButtonState (bool displayError)
	{
		if (emailIsValid && userNameIsValid && registerPasswordIsValid && acceptTermsToggle.isOn)
		{
			registerButton.interactable = true;
		}
		else
		{
			registerButton.interactable = false;
		}
	}


	private void UpdateSubmitButtonState (bool displayError)
	{
		if (emailIsValid)
		{
			submitEmailButton.interactable = true;
		}
		else
		{
			submitEmailButton.interactable = false;
		}
	}


	private void ClearAll ()
	{
		emailField.text = string.Empty;
		userNameRegisterField.text = string.Empty;
		acceptTermsToggle.isOn = false;
		ClearErrorMessages();
		ClearPasswordFields();
	}
	

	private void ClearPasswordFields ()
	{
		passwordLoginField.text = string.Empty;
		passwordRegisterField.text = string.Empty;
	}


	private void ClearErrorMessages ()
	{
		registerErrorText.text = string.Empty;
		loginErrorText.text = string.Empty;
		forgotPasswordErrorText.text = string.Empty;
	}


	public void DisplayErrorMessage (string errorMessage, int errorType)
	{
		switch (errorType)
		{
			case 0:
				registerErrorText.text = errorMessage;
				loginErrorText.text = string.Empty;
				forgotPasswordErrorText.text = string.Empty;
				break;
			case 1:
				loginErrorText.text = errorMessage;
				registerErrorText.text = string.Empty;
				forgotPasswordErrorText.text = string.Empty;
				break;
			case 2:
				forgotPasswordErrorText.text = errorMessage;
				loginErrorText.text = string.Empty;
				registerErrorText.text = string.Empty;
				break;
			case 3:
				forgotPasswordErrorText.text = errorMessage;
				loginErrorText.text = errorMessage;
				registerErrorText.text = errorMessage;
				break;
			default:
				registerErrorText.text = errorMessage;
				loginErrorText.text = errorMessage;
				forgotPasswordErrorText.text = string.Empty;
				break;
		}
	}


	public void LogCompleteRegistrationEvent (string registrationMethod) 
	{
    	var registrationParameters = new Dictionary<string, object>();
    	registrationParameters[AppEventParameterName.RegistrationMethod] = registrationMethod;
    	FB.LogAppEvent(AppEventName.CompletedRegistration, 0.0f, registrationParameters);

		Debug.Log("Level Achieved: Sign-up");
		var levelParameters = new Dictionary<string, object>();
    	levelParameters[AppEventParameterName.Level] = "Sign-up";
    	FB.LogAppEvent(AppEventName.AchievedLevel, 0.0f, levelParameters);
	}
}
