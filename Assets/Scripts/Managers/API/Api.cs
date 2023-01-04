using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Helpers;
using Managers.API;
using UnityEngine;
using UnityEngine.Networking;

public class Api : MonoBehaviour
{
    public static bool forceUpdate;

    private string apiAddress = "https://production.cdwurmlfeuqe.eu-central-1.rds.amazonaws.com"; // Production

    private const string privateKey = "ttoH2oioHqdqjT+d3aWgNBedxVm730SbMRHZhBeVLs=";

    private static Api mInstance;

    public static string ErrorMessage(Enums.NetworkErrorType errorType)
    {
        switch (errorType)
        {
            case Enums.NetworkErrorType.NoTimestampReturned:
                return "No connection to the server.\nPlease check your internet connection."; //"Ingen forbindelse til serveren. Kontrollér din internetforbindelse.";
            case Enums.NetworkErrorType.ApiCallError:
                return "Unable to connect to the server."; 
            case Enums.NetworkErrorType.TimeOut:
                return "Connection to server timed out."; 
            case Enums.NetworkErrorType.WrongApiVersion:
                return "Wrong app version. Please update the app."; 
            case Enums.NetworkErrorType.UsernameNotUnique:
                return "The username already exists. Please try another."; //"Brugernavnet findes i forvejen. Prøv med et andet.";
            case Enums.NetworkErrorType.EmailInvalid:
                return "The e-mail address has an invalid format."; //"E-mailadressen har ikke det rigtige format.";
            case Enums.NetworkErrorType.EmailNotFound:
                return "The e-mail address wasn't found."; //"Der findes ikke en bruger med den e-mailadresse.";
            case Enums.NetworkErrorType.EmailNotUnique:
                return "The e-mail address already exists. Try logging in with it."; //"E-mailadressen findes i forvejen. Prøv at logge ind med den.";
            case Enums.NetworkErrorType.AccountNotFound:
                return "The e-mail address or password is incorrect.";
            case Enums.NetworkErrorType.WrongOldPassword:
                return "The old password is incorrect.";
            case Enums.NetworkErrorType.EmailAlreadyInvited:
                return "The e-mail address has already been invited.";
            case Enums.NetworkErrorType.EventIsClosed:
                return "The event is closed.";
            case Enums.NetworkErrorType.EventCanOnlyBePlayedFromOneDevice:
                return "This event can only be played from one device.\nYou already entered this table from another device.";
            case Enums.NetworkErrorType.EventNotFound:
                return "The event could not be found.";
            case Enums.NetworkErrorType.EventTypeIsNotSupported:
                return "The event type is not supported.";
            case Enums.NetworkErrorType.NoGamesLeftToday:
                return "No THE LEAGUE games left today.";
        }

        return "An unknown error occurred."; //"Der skete en ukendt fejl.";
    }

    private static Dictionary<string, Enums.NetworkErrorType> errorTypeFromJsonMap = new Dictionary<string, Enums.NetworkErrorType>()
    {
        { "event is closed", Enums.NetworkErrorType.EventIsClosed },
        { "event can only be played from one device", Enums.NetworkErrorType.EventCanOnlyBePlayedFromOneDevice },
        { "event not found", Enums.NetworkErrorType.EventNotFound },
        { "event type is not supported", Enums.NetworkErrorType.EventTypeIsNotSupported },
        { "no games left today", Enums.NetworkErrorType.NoGamesLeftToday }

    };

    public static Enums.NetworkErrorType getErrorTypeFromJson(string json, Enums.NetworkErrorType error)
    {
        foreach (var key in errorTypeFromJsonMap.Keys)
        {
            if (json.Contains(key))
            {
                return errorTypeFromJsonMap[key];
            }
        }
        return error;
    }


    private static Api Instance
    {
        get
        {
            if (mInstance != null)
            {
                return mInstance;
            }

            mInstance = (new GameObject("Api")).AddComponent<Api>();

            mInstance.apiAddress = "https://api2.cardgame.boost22.com/api/"; // Production
            //mInstance.apiAddress = "http://ec2-3-121-201-86.eu-central-1.compute.amazonaws.com:9191/api/"; // Development server

            //#if UNITY_EDITOR
            //// Localhost for test
            //mInstance.apiAddress = "http://0.0.0.0:9191/api/"; // Development localhost
            //#endif

            DontDestroyOnLoad(mInstance);
            return mInstance;
        }
    }

    public static void SendMessage(ServerMessage serverMessage, Action<string, Enums.NetworkErrorType> callback)
    {
        Instance.StartCoroutine(SendMessageCoroutine(serverMessage, callback));
    }

    public static void HasNet(Action<bool> callback)
    {
        var www = new WWW("https://google.com");
        callback(www.error == null);
    }

    private static IEnumerator SendMessageCoroutine(ServerMessage serverMessage, Action<string, Enums.NetworkErrorType> callback)
    {
        var timestampService = Instance.apiAddress + "timestamp";

        var retries = 0;
        var timestamp = 0;
        var minimumApiVersion = 0;
        var maintenanceUntilTimestamp = 0;

        UnityWebRequest www;

        while (retries <= 2 && timestamp == 0) // Retries timestamp 3 times else show maintain message
        {
            retries += 1;

            www = UnityWebRequest.Get(timestampService);
            www.timeout = 10;
            yield return www.SendWebRequest();

            if (string.IsNullOrEmpty(www.downloadHandler.text))
            {
                continue;
            }

            var timestampResponseJson = JsonUtility.FromJson<TimestampResponse>(www.downloadHandler.text);
            timestamp = timestampResponseJson.timestamp;
            minimumApiVersion = timestampResponseJson.minimumAPIVersion;
            maintenanceUntilTimestamp = timestampResponseJson.maintenanceUntilTimestamp;
        }

        if (maintenanceUntilTimestamp > 0)
        {
            callback(null, Enums.NetworkErrorType.Maintenance);
            yield break;
        }

        if (Constants.apiVersion < minimumApiVersion)
        {
            forceUpdate = true;
            UserManager.LogOut();
            GameObject.Find("WrongApiVersionErrorPrompt").GetComponent<Animator>().SetTrigger("open");
            yield break;
        }

        if (timestamp == 0)
        {
            HasNet((hasNet) => { callback(null, hasNet ? Enums.NetworkErrorType.NoTimestampReturned : Enums.NetworkErrorType.NoNet); });
            yield break;
        }

        var pData = Encoding.ASCII.GetBytes(serverMessage.data.ToJson());

        var headers = GenerateHeaders(timestamp);

        if (serverMessage.type == ServerMessageType.PostType)
        {
            var api = new WWW(Instance.apiAddress + serverMessage.url, pData, headers);
            float timer = 0;
            const float timeOut = 10;
            var didTimeOut = false;

            while (!api.isDone)
            {
                if (timer > timeOut)
                {
                    didTimeOut = true; break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            if (didTimeOut || api.error != null || !string.IsNullOrEmpty(api.error))
            {
                HasNet((hasNet) => { callback(didTimeOut ? "timeout" : api.text, hasNet ? Enums.NetworkErrorType.TimeOut : Enums.NetworkErrorType.NoNet); });
                api.Dispose();
                yield break;
            }

            if (api.text != null && !api.text.Contains("\"error\":true"))
            {
                callback(api.text, Enums.NetworkErrorType.NoError);
            }
            else if (api.text != null && api.text.Contains("\"need_update_app\""))
            {
                var errorClass = JsonUtility.FromJson<ErrorClass>(api.text);
                callback(errorClass.text, Enums.NetworkErrorType.WrongApiVersion);
            }
            else
            {
                HasNet((hasNet) => { callback(null, hasNet ? Enums.NetworkErrorType.ApiCallError : Enums.NetworkErrorType.NoNet); });
            }
        }
        else
        {
            switch (serverMessage.type)
            {
                case ServerMessageType.GetType:
                    www = UnityWebRequest.Get(Instance.apiAddress + serverMessage.url);
                    break;
                case ServerMessageType.DeleteType:
                    www = UnityWebRequest.Delete(Instance.apiAddress + serverMessage.url);
                    break;
                default:
                    callback(null, Enums.NetworkErrorType.ApiCallError);
                    yield break;
            }

            foreach (var header in headers)
            {
                www.SetRequestHeader(header.Key, header.Value);
            }

            www.timeout = 10;

            yield return www.SendWebRequest();

            if (www.downloadHandler.text != null && !www.downloadHandler.text.Contains("\"error\":true") && string.IsNullOrEmpty(www.error))
            {
                callback(www.downloadHandler.text, Enums.NetworkErrorType.NoError);
            }
            else
            {
                HasNet((hasNet) => { callback(null, hasNet ? Enums.NetworkErrorType.ApiCallError : Enums.NetworkErrorType.NoNet); });
            }
        }
    }

    private static Dictionary<string, string> GenerateHeaders(int timestamp)
    {
        var headers = new Dictionary<string, string> {{"key", GenerateServerToken(timestamp)}};
        if (UserManager.LoggedInUser != null && !string.IsNullOrEmpty(UserManager.LoggedInUser.token))
        {
            headers.Add("Authorization", "Bearer " + UserManager.LoggedInUser.token);
        }
        headers.Add("Content-Type", "application/json");
        headers.Add("Accept", "application/json");
        return headers;
    }

    private static string GenerateServerToken(int timestamp)
    {
        //var date = UnixTimeStampToDateTime(timestamp);
        //var dateString = string.Format("{0:dd}", date);
        var key = privateKey + timestamp;// + dateString;
        return sha256_hash(key);
    }

    private static string sha256_hash(string value)
    {
        var sb = new StringBuilder();

        using (var hash = SHA256.Create())
        {
            var enc = Encoding.UTF8;
            var result = hash.ComputeHash(enc.GetBytes(value));

            foreach (var b in result)
                sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }

//    private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
//    {
//        // Unix timestamp is seconds past epoch
//        var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
//        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
//        return dtDateTime;
//    }
}

internal static class Utilities
{
    public static string ToJson(this IDictionary<string, object> dictionary)
    {
        return Json.Serialize((object) dictionary);
    }
}

[Serializable]
public class ErrorClass
{
    public bool error;
    public string text;
    public string errorCode;
}

[Serializable]
public struct TimestampResponse
{
    public int timestamp;
    public int minimumAPIVersion;
    public int maintenanceUntilTimestamp;
}

[Serializable]
public struct EventResponse
{
    public Event gameEvent;
}

[Serializable]
public struct EventIsClosedResponse
{
    public bool eventIsClosed;
}
