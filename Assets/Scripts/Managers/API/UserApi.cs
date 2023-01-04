using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers.API
{
    public class UserApi
    {
        public static void Create(string username, string email, string password, string inviteToken, Action<User, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["username"] = username;
            data["email"] = email;
            data["passwordHash"] = password;
            data["inviteToken"] = inviteToken;
            data["deviceLanguage"] = Application.systemLanguage.ToString();

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "users/register", data);
            Api.SendMessage(serverMessage, (userJson, error) =>
            {
                if (userJson == null)
                {
                    callback(null, error);
                    return;
                }

                if (userJson.Contains("email already exists") || userJson.Contains("email_unique_idx"))
                {
                    callback(null, Enums.NetworkErrorType.EmailNotUnique);
                    return;
                }

                if (userJson.Contains("email invalid"))
                {
                    callback(null, Enums.NetworkErrorType.EmailInvalid);
                    return;
                }

                if (userJson.Contains("username_unique_idx") || userJson.Contains("username not unique"))
                {
                    callback(null, Enums.NetworkErrorType.UsernameNotUnique);
                    return;
                }

                var user = JsonUtility.FromJson<User>(userJson);
                UserManager.LoggedInUser = user;
                callback(!string.IsNullOrEmpty(user.token) ? user : null, error);
            });
        }


        public static void Create(string name, string email, string password, string facebookId, string inviteToken, Action<User, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["username"] = name;
            data["facebookId"] = facebookId;
            data["email"] = email;
            data["passwordHash"] = password;
            data["inviteToken"] = inviteToken;
            data["deviceLanguage"] = Application.systemLanguage.ToString();

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "users/register", data);
            Api.SendMessage(serverMessage, (userJson, error) =>
            {
                if (userJson == null)
                {
                    callback(null, error);
                    return;
                }

                if (userJson.Contains("email already exists") || userJson.Contains("email_unique_idx"))
                {
                    callback(null, Enums.NetworkErrorType.EmailNotUnique);
                    return;
                }

                if (userJson.Contains("email invalid"))
                {
                    callback(null, Enums.NetworkErrorType.EmailInvalid);
                    return;
                }

                if (userJson.Contains("username_unique_idx") || userJson.Contains("username not unique"))
                {
                    callback(null, Enums.NetworkErrorType.UsernameNotUnique);
                    return;
                }
                
                var user = JsonUtility.FromJson<User>(userJson);
                callback(!string.IsNullOrEmpty(user.token) ? user : null, error);
            });
        }


        public static void ForgotPassword(string email, Action<bool, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["email"] = email;

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "users/forgot", data);
            Api.SendMessage(serverMessage,
                (userJson, error) =>
                {
                    callback(error == Enums.NetworkErrorType.NoError, error);
                });
        }

        public static void ChangePassword(string oldPassword, string newPassword, Action<bool, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["oldPassword"] = oldPassword;
            data["newPassword"] = newPassword;
            data["deviceLanguage"] = Application.systemLanguage.ToString();

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "users/password", data);
            Api.SendMessage(serverMessage,
                (json, error) =>
                {
                    if (json.Contains("wrong old password"))
                    {
                        callback(false, Enums.NetworkErrorType.WrongOldPassword);
                    }
                    else
                    {
                        callback(error == Enums.NetworkErrorType.NoError, error);
                    }
                });
        }

        public static void Login(string email, string password, Action<User, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["email"] = email;
            data["password"] = password;
            data["deviceLanguage"] = Application.systemLanguage.ToString();

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "users/login", data);
            Api.SendMessage(serverMessage, (userJson, error) =>
            {
                if (userJson == null)
                {
                    callback(null, error);
                    return;
                }

                if (userJson.Contains("Unauthorized") || userJson == "" || userJson == "timeout")
                {
                    callback(null, Enums.NetworkErrorType.AccountNotFound);
                    return;
                }

                var user = JsonUtility.FromJson<User>(userJson);
                UserManager.LoggedInUser = user;
                callback(!string.IsNullOrEmpty(user.token) ? user : null, error);
            });
        }

        public static void Update(Action<User, Enums.NetworkErrorType> callback)
        {
            //var data = new Dictionary<string, object>();

            var serverMessage = new ServerMessage(ServerMessageType.GetType, "users/updateUser" /*, data*/);
            Api.SendMessage(serverMessage, (userJson, error) =>
            {
                if (userJson == null)
                {
                    callback(null, error);
                    return;
                }

                if (userJson.Contains("Unauthorized") || userJson == "" || userJson == "timeout")
                {
                    callback(null, Enums.NetworkErrorType.EmailNotFound);
                    return;
                }

                var user = JsonUtility.FromJson<User>(userJson);
                UserManager.LoggedInUser = user;
                callback(!string.IsNullOrEmpty(user.token) ? user : null, error);
            });
        }


        public static void GetDynamicUserData (Action<DynamicUserData, Enums.NetworkErrorType> callback)
        {
            //var data = new Dictionary<string, object>();

            var serverMessage = new ServerMessage(ServerMessageType.GetType, "users/dynamic_user_data" /*, data*/);
            Api.SendMessage(serverMessage, (userJson, error) =>
            {
                if (userJson == null)
                {
                    callback(null, error);
                    return;
                }

                if (userJson.Contains("Unauthorized") || userJson == "" || userJson == "timeout")
                {
                    callback(null, Enums.NetworkErrorType.EmailNotFound);
                    return;
                }

                var dynamicUserData = JsonUtility.FromJson<DynamicUserData>(userJson);
                callback(dynamicUserData, error);
            });
        }


        public static void ChangeProfileData (string username, string phoneNumber, string countryCode, Action<bool, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["username"] = username;
            data["phoneNumber"] = phoneNumber;
            data["countryCode"] = countryCode;
            data["deviceLanguage"] = Application.systemLanguage.ToString();

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "users/change_profile_data", data);
            Api.SendMessage(serverMessage, (userJson, error) =>
            {
                if (userJson == null)
                {
                    callback(false, error);
                    return;
                }

                /*if (userJson.Contains("Unauthorized") || userJson == "" || userJson == "timeout")
                {
                    callback(false, Enums.NetworkErrorType.EmailNotFound);
                    return;
                }*/

                if (error != Enums.NetworkErrorType.NoError)
                {
                    callback(false, error);
                    return;
                }
                
                callback(true, error);
            });
        }

        public static void RequestPayment(Enums.PaymentMethodType paymentMethodType, string email, string countryCode, string phoneNumber, Action<bool, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["paymentMethodType"] = (int)paymentMethodType;
            data["email"] = email;
            data["countryCode"] = countryCode;
            data["phoneNumber"] = phoneNumber;

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "payment/request", data);
            Api.SendMessage(serverMessage, (userJson, error) =>
            {
                if (userJson == null)
                {
                    callback(false, error);
                    return;
                }

                /*if (userJson.Contains("Unauthorized") || userJson == "" || userJson == "timeout")
                {
                    callback(false, Enums.NetworkErrorType.EmailNotFound);
                    return;
                }*/

                if (error != Enums.NetworkErrorType.NoError)
                {
                    callback(false, error);
                    return;
                }

                callback(true, error);
            });
        }


        public static void GetServerStats (Action<DashboardStats, Enums.NetworkErrorType> callback)
        {
            //var data = new Dictionary<string, object>();

            var serverMessage = new ServerMessage(ServerMessageType.GetType, "statistics/dashboard" /*, data*/);
            Api.SendMessage(serverMessage, (userJson, error) =>
            {
                if (userJson == null)
                {
                    callback(null, error);
                    return;
                }

                var serverStats = JsonUtility.FromJson<DashboardStats>(userJson);
                callback(serverStats, error);
            });
        }


        public static void GetStatisticsDetails (string dateString, Action<string, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["dateString"] = dateString;

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "statistics/details", data);
            Api.SendMessage(serverMessage, (userJson, error) =>
            {
                if (userJson == null)
                {
                    callback(null, error);
                    return;
                }

                var statisticsDetails = JsonUtility.FromJson<DashboardDetails>(userJson);
                callback(statisticsDetails.statisticsDetails, error);
            });
        }
    }
}