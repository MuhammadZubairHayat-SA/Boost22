using System.Collections.Generic;
using UnityEngine;
//using Facebook.Unity;
//using Facebook.MiniJSON;
using Managers.API;

public class LoginFacebookButton : MonoBehaviour
{
	public LoginRegisterMenu loginRegisterMenu;
    public Menus menus;


	/*public void Clicked ()
	{
		if (loginRegisterMenu.termsAccepted)
		{
			Debug.Log("Logging in with Facebook");
			CallLogin();
		}
		else
		{
			Debug.Log("Cannot log in with Facebook; Terms not accepted");
            if (loginRegisterMenu != null)
            {
			    loginRegisterMenu.DisplayErrorMessage("You must accept the terms to register", -1);
            }
		}
	}

    public void CallLogin()
    {
        if (FB.IsInitialized)
        {
            //SoundManager.PlaySound(SoundType.Button);

            if (loginRegisterMenu != null)
            {
                loginRegisterMenu.ShowSpinner();
            }

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
                    //UIManager.ShowNotification(Constants.alert1ButtonName, 0, false, I2.Loc.ScriptLocalization.Alert_Facebook_Login___Could_not_connect_title, I2.Loc.ScriptLocalization.Alert_Facebook_Login___Could_not_connect_message);
                    errorText = "Could not connect to Facebook";
                }
                else
                {
                    //UIManager.ShowNotification(Constants.alert1ButtonName, 0, false, I2.Loc.ScriptLocalization.Alert_Facebook_Login___No_net_title, I2.Loc.ScriptLocalization.Alert_Facebook_Login___No_net_message);
					errorText = "No internet connection";
                }

                if (loginRegisterMenu != null)
                {
                    loginRegisterMenu.HideSpinner();
                    loginRegisterMenu.DisplayErrorMessage(errorText, -1);
                }
            });
        }
    }

    private void HandleLoginResult(ILoginResult result)
    {
        if (result == null || !string.IsNullOrEmpty(result.Error) || result.Cancelled)
        {
            if (loginRegisterMenu != null)
            {
                loginRegisterMenu.HideSpinner();
                loginRegisterMenu.DisplayErrorMessage(result.Error, -1);
            }
            //UIManager.ShowNotification(Constants.alert1ButtonName, 0, false, I2.Loc.ScriptLocalization.Alert_Facebook_Login___Not_allowed_to_connect_to_a_2, I2.Loc.ScriptLocalization.Alert_Facebook_Login___Not_allowed_to_connect_to_a_1);
			
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            FB.API("/me?fields=id,first_name,last_name,email", HttpMethod.GET, HandleInfoResult);
        }
        else
        {
            if (loginRegisterMenu != null)
            {
                loginRegisterMenu.HideSpinner();
            }

            Api.HasNet(hasNet =>
            {
                string errorText = "";
                if (hasNet)
                {
                    //UIManager.ShowNotification(Constants.alert1ButtonName, 0, false, I2.Loc.ScriptLocalization.Alert_Facebook_Login___Could_not_connect_title, I2.Loc.ScriptLocalization.Alert_Facebook_Login___Could_not_connect_message);
					errorText = "Could not connect to Facebook";
                }
                else
                {
                    //UIManager.ShowNotification(Constants.alert1ButtonName, 0, false, I2.Loc.ScriptLocalization.Alert_Facebook_Login___No_net_title, I2.Loc.ScriptLocalization.Alert_Facebook_Login___No_net_message);
					errorText = "No internet connection";
                }

                if (loginRegisterMenu != null)
                {
                    loginRegisterMenu.HideSpinner();
                    loginRegisterMenu.DisplayErrorMessage(errorText, -1);
                }
            });
        }

    }

    private void HandleInfoResult(IResult result)
    {
        if (result == null || !string.IsNullOrEmpty(result.Error) || result.Cancelled)
        {
            if (loginRegisterMenu != null)
            {
                loginRegisterMenu.HideSpinner();
                loginRegisterMenu.DisplayErrorMessage(result.Error, -1);
            }
            //UIManager.ShowNotification(Constants.alert1ButtonName, 0, false, I2.Loc.ScriptLocalization.Alert_Facebook_Login___Not_allowed_to_connect_to_a_2, I2.Loc.ScriptLocalization.Alert_Facebook_Login___Not_allowed_to_connect_to_a_1);
			
            return;
        }

        if (!string.IsNullOrEmpty(result.RawResult))
        {
            var dict = Json.Deserialize(result.RawResult) as Dictionary<string, object>;
            if (dict == null)
            {
                if (loginRegisterMenu != null)
                {
                    loginRegisterMenu.HideSpinner();
                }

                Api.HasNet(hasNet =>
                {
                    if (hasNet)
                    {
                        //UIManager.ShowNotification(Constants.alert1ButtonName, 0, false, I2.Loc.ScriptLocalization.Alert_Facebook_Login___Could_not_connect_title, I2.Loc.ScriptLocalization.Alert_Facebook_Login___Could_not_connect_message);
                        if (loginRegisterMenu != null)
                        {
                            loginRegisterMenu.HideSpinner();
                            loginRegisterMenu.DisplayErrorMessage("Could not connect to Facebook", -1);
                        }
                    }
                    else
                    {
                        //UIManager.ShowNotification(Constants.alert1ButtonName, 0, false, I2.Loc.ScriptLocalization.Alert_Facebook_Login___No_net_title, I2.Loc.ScriptLocalization.Alert_Facebook_Login___No_net_message);
                        if (loginRegisterMenu != null)
                        {
                            loginRegisterMenu.HideSpinner();
                            loginRegisterMenu.DisplayErrorMessage("No internet connection", -1);
                        }
                    }
                });
                return;
            }
            
            var username = (string) dict["first_name"];

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

            UserApi.Create(username, email, "", facebookId, Functions.GetInviteTokenFromClipboard(), (user, errorType) =>
			{
				Debug.Log("CREATE");
				if (errorType != Enums.NetworkErrorType.NoError)
				{
					errorText = Api.ErrorMessage(errorType);
					Debug.Log(errorType);
                    if (loginRegisterMenu != null)
                    {
                        loginRegisterMenu.HideSpinner();
                        loginRegisterMenu.DisplayErrorMessage(errorText, -1);
                    }
                    return;
                } else
				{
					UserManager.LoggedInUser = user;
				}

                if (menus != null)
                {
                    menus.screensAnimator.SetBool("toLobby", true);
                    if (user.numberOfGamesPlayed == 0)
                    {
                        menus.SelectLobbyMenu(2);
                    }
                    else
                    {
                        menus.SelectLobbyMenu(0);
                    }
                }

                if (loginRegisterMenu != null)
                {
                    loginRegisterMenu.HideSpinner();
                    loginRegisterMenu.DisplayErrorMessage(errorText, -1);
                    loginRegisterMenu.LogCompleteRegistrationEvent("In-App");
					//loginRegisterMenu.ProceedToLobby();
                }

            });
        }
        else
        {
            if (loginRegisterMenu != null)
            {
                loginRegisterMenu.HideSpinner();
                loginRegisterMenu.DisplayErrorMessage("No internet connection", -1);
            }
            //UIManager.ShowNotification(Constants.alert1ButtonName, 0, false, I2.Loc.ScriptLocalization.Alert_Facebook_Login___No_net_title, I2.Loc.ScriptLocalization.Alert_Facebook_Login___No_net_message);
        }
    }*/
}
