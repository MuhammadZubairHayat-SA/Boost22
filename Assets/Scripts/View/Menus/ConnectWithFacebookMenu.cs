using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Facebook.Unity;
using Facebook.MiniJSON;
using Managers.API;

public class ConnectWithFacebookMenu : MonoBehaviour 
{
	public AccountView accountView;
	public Animator accountOverlay;
	public TextMeshProUGUI messageText;
	public TextMeshProUGUI errorText;
	public Button cancelButton;
	public Button okayButton;
	public CanvasGroup spinnner;

	
	public void ShowSpinner ()
	{
		spinnner.alpha = 1;
		spinnner.blocksRaycasts = true;
		ClearMessages();
	}


	public void HideSpinner ()
	{
		spinnner.alpha = 0;
		spinnner.blocksRaycasts = false;
	}


	public void ClearMessages ()
	{
		errorText.text = string.Empty;
	}


	public void DisplayErrorMessage (string errorMessage, int errorType)
	{
		errorText.text = errorMessage;
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

		ClearMessages();
		messageText.text = "Successfully connected to Facebook account";
		accountView.UpdateAccount();
	}


	public void CallLogin()
    {
        if (FB.IsInitialized)
        {
            //SoundManager.PlaySound(SoundType.Button);

            ShowSpinner();

            FB.LogInWithReadPermissions(new List<string>()
                                        {
                                            "public_profile",
                                            "email"
                                        }, HandleLoginResult);
        }
        else
        {
            Api.HasNet(hasNet =>
            {
                string errorText = "";

                if (hasNet)
                {
                    errorText = "Could not connect to Facebook";
                }
                else
                {
					errorText = "No internet connection";
                }

				HideSpinner();
				DisplayErrorMessage(errorText, -1);
            });
        }
    }

    private void HandleLoginResult(ILoginResult result)
    {
        if (result == null || !string.IsNullOrEmpty(result.Error) || result.Cancelled)
        {
            HideSpinner();
        	DisplayErrorMessage("Not allowed to connect to Facebook (login result)", -1);
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            FB.API("/me?fields=id,first_name,last_name,email", HttpMethod.GET, HandleInfoResult);
        }
        else
        {
            HideSpinner();

            Api.HasNet(hasNet =>
            {
                string errorText = "";
                if (hasNet)
                {
					errorText = "Could not connect to Facebook";
                }
                else
                {
					errorText = "No internet connection";
                }

				HideSpinner();
				DisplayErrorMessage(errorText, -1);
            });
        }

    }

    private void HandleInfoResult(IResult result)
    {
        if (result == null || !string.IsNullOrEmpty(result.Error) || result.Cancelled)
        {
            HideSpinner();
        	DisplayErrorMessage("Not allowed to connect to Facebook (info result)", -1);

            return;
        }

        if (!string.IsNullOrEmpty(result.RawResult))
        {
            var dict = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
            if (dict == null)
            {
                HideSpinner();

                Api.HasNet(hasNet =>
                {
                    if (hasNet)
                    {
                        HideSpinner();
                        DisplayErrorMessage("Could not connect to Facebook", -1);
                    }
                    else
                    {
                        HideSpinner();
                        DisplayErrorMessage("No internet connection", -1);
                    }
                });
                return;
            }
            
            var username = (string) dict["first_name"];

            if(dict.ContainsKey("last_name")){
                username += " ";
                username += (string) dict["last_name"];
            }

            if(dict.ContainsKey("last_name")){
                username += " ";
                username += (string) dict["last_name"];
            }

            string email = null;
            if (dict.ContainsKey("email"))
            {
                email = (string) dict["email"];
            }

            var aToken = AccessToken.CurrentAccessToken;
            var facebookId = aToken.UserId;

            string errorText = "";

			UserApi.Create(UserManager.LoggedInUser.username, UserManager.LoggedInUser.email, "", facebookId, null, (user, errorType) =>
			{
				Debug.Log("CREATE");
				if (errorType != Enums.NetworkErrorType.NoError)
				{
					errorText = Api.ErrorMessage(errorType);
					Debug.Log(errorType);
				} else
				{
					UserManager.LoggedInUser = user;
				}

                HideSpinner();
                DisplayErrorMessage(errorText, -1);
                LogCompleteRegistrationEvent("In-App");
			});
        }
        else
        {
            HideSpinner();
            DisplayErrorMessage("No internet connection", -1);
        }
    }
}
