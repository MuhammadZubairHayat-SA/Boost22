using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace Managers.API
{
    public class ScoreApi
    {
        public static void Add(List<Player> players, string gameIdentifier, int currentEventId, Enums.GameType gameType, int gameTime, Action<bool, Enums.NetworkErrorType> callback)
        {
            // Enable unlimited play without saving scores (ideal for displays, e.g. at conferences):
            // callback(true, Enums.NetworkErrorType.NoError);
            // return;

            var data = new Dictionary<string, object>();
            var dataUsers = new List<Dictionary<string, object>>();

            foreach (var player in players)
            {
                foreach (var pointType in player.pointTypes)
                {
                    var dataPointType = new Dictionary<string, object>();
                    dataPointType["userId"] = player.user.id;
                    dataPointType["gameType"] = (int) gameType;
                    dataPointType["pointType"] = (int) pointType;
                    dataPointType["gameTime"] = gameTime;
                    dataPointType["apiVersion"] = Constants.apiVersion;
                    dataUsers.Add(dataPointType);
                }
            }

            data["scores"] = dataUsers;

            data["deviceIdentifier"] = PlayerPrefsManager.GetDeviceIdentifier();
            data["gameIdentifier"] = gameIdentifier;
            if (currentEventId > 0)
            {
                data["eventId"] = currentEventId;
            }

            Debug.Log("DeviceID: " + PlayerPrefsManager.GetDeviceIdentifier());
            Debug.Log("User:" + UserManager.LoggedInUser.GetHashCode());

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "score/add_multiple", data);
            Api.SendMessage(serverMessage, (scoreJson, error) =>
            {
                if (scoreJson == null || scoreJson.Contains("error") || scoreJson == "timeout")
                {

                    callback(false, Api.getErrorTypeFromJson(json: scoreJson, error: error));
                    return;
                }

                callback(true, error);
            });
        }


        public static void KilledAppPunishment (User user, int eventId, Action<bool, Enums.NetworkErrorType> callback)
        {
            var player = new Player(user);
            var playerList = new List<Player>();
            playerList.Add(player);
            player.pointTypes.Add(Enums.PointType.Boost22_4th);

            ScoreApi.Add(playerList, Guid.NewGuid().ToString(), eventId, Enums.GameType.Boost22, 0, (result, networkErrorType) => {
                if (networkErrorType == Enums.NetworkErrorType.EventIsClosed)
                {
                    PlayerPrefsManager.ClearPlayerPrefsForKilledDuringGame();
                    callback(true, networkErrorType);
                    return;
                }

                callback(result, networkErrorType);
            });
        }


        public static void ReadyForPlayingEvent (User user, int currentEventId, Action<bool, Enums.NetworkErrorType> callback)
        {
            if (currentEventId > 0)
            {
                var player = new Player(user);
                var playerList = new List<Player>();
                playerList.Add(player);
                player.pointTypes.Add(Enums.PointType.ReadyForPlayingEvent);

                ScoreApi.Add(playerList, Guid.NewGuid().ToString(), currentEventId, Enums.GameType.Boost22, 0, callback);
            }
        }


        public static void GetWeek(Enums.GameType gameType, Action<Score[], Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["gameType"] = (int) gameType;
            
            var serverMessage = new ServerMessage(ServerMessageType.PostType, "score/get/week", data);
            Api.SendMessage(serverMessage, (json, error) =>
            {
                if (json == null || json.Contains("error") || json == "timeout")
                {
                    callback(new Score[0], error);
                    return;
                }

                // Debug.Log(json);

                var scoreboard = JsonUtility.FromJson<Scoreboard>(json);
                callback(scoreboard.scores, error);
            });
        }


        public static void GetMonth(Enums.GameType gameType, Action<Score[], Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["gameType"] = (int) gameType;

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "score/get/month", data);
            Api.SendMessage(serverMessage, (json, error) =>
            {
                if (json == null || json.Contains("error") || json == "timeout")
                {
                    callback(new Score[0], error);
                    return;
                }

                // Debug.Log(json);

                var scoreboard = JsonUtility.FromJson<Scoreboard>(json);
                callback(scoreboard.scores, error);
            });
        }


        public static void GetDay(Enums.GameType gameType, Action<Score[], Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["gameType"] = (int) gameType;

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "score/get/day", data);
            Api.SendMessage(serverMessage, (json, error) =>
            {
                if (json == null || json.Contains("error") || json == "timeout")
                {
                    callback(new Score[0], error);
                    return;
                }

                // Debug.Log(json);

                var scoreboard = JsonUtility.FromJson<Scoreboard>(json);
                callback(scoreboard.scores, error);
            });
        }


        public static void GetLatestRushEventResults(Enums.GameType gameType, int userId, Action<EventScoreboard, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["gameType"] = (int) gameType;
            if (userId > 0)
            {
                data["userId"] = userId;
            }

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "score/get/latest_event", data);
            Api.SendMessage(serverMessage, (json, error) =>
            {
                if (json == null || json.Contains("error") || json == "timeout")
                {
                    callback(new EventScoreboard(), error);
                    return;
                }

                // Debug.Log(json);

                var eventScoreboard = JsonUtility.FromJson<EventScoreboard>(json);
                callback(eventScoreboard, error);
            });
        }


        public static void GetCurrentEventResults(string eventId, Action<Score[], Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["eventId"] = eventId;

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "score/get/event", data);
            Api.SendMessage(serverMessage, (json, error) =>
            {
                if (json == null || json.Contains("error") || json == "timeout")
                {
                    callback(new Score[0], error);
                    return;
                }

                // Debug.Log(json);

                var scoreboard = JsonUtility.FromJson<Scoreboard>(json);
                callback(scoreboard.scores, error);
            });
        
        
        }


        public static void GetRunningEventResults(int eventId, Action<RunningEventResults, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["eventId"] = eventId;

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "score/get/minimal_event", data);
            Api.SendMessage(serverMessage, (json, error) =>
            {
                if (json == null || json.Contains("error") || json == "timeout")
                {
                    callback(null, error);
                    return;
                }

                // Debug.Log(json);

                var runningEventResults = JsonUtility.FromJson<RunningEventResults>(json);
                callback(runningEventResults, error);
            });
        }


        public static void GetNextRushEvent (Action<Event, Enums.NetworkErrorType> callback)
        {
            var serverMessage = new ServerMessage(ServerMessageType.GetType, "event/get/next_rush_event");
            Api.SendMessage(serverMessage, (json, error) =>
            {
                if (json == null || json.Contains("error") || json == "timeout")
                {
                    callback(null, error);
                    return;
                }

                // Debug.Log(json);

                var nextEventResponse = JsonUtility.FromJson<EventResponse>(json);
                callback(nextEventResponse.gameEvent, error);
            });
        }


        public static void GetEventIsClosed (int eventId, Action<bool, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["eventId"] = eventId;

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "score/get/event_closed", data);
            Api.SendMessage(serverMessage, (json, error) =>
            {
                if (json == null || json.Contains("error") || json == "timeout")
                {
                    callback(false, error);
                    return;
                }

                // Debug.Log(json);

                var eventIsClosedResponse = JsonUtility.FromJson<EventIsClosedResponse>(json);
                callback(eventIsClosedResponse.eventIsClosed, error);
            });
        }


        public static void GetRunningEventLeaders(int eventId, Action<EventLeaderboard, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
            data["eventId"] = eventId;

            var serverMessage = new ServerMessage(ServerMessageType.PostType, "score/get/event_leaderboard", data);
            Api.SendMessage(serverMessage, (json, error) =>
            {
                if (json == null || json.Contains("error") || json == "timeout")
                {
                    callback(null, error);
                    return;
                }

                // Debug.Log(json);

                var eventLeaders = JsonUtility.FromJson<EventLeaderboard>(json);
                callback(eventLeaders, error);
            });
        }

        public static void GetWinners(Action<Winners, Enums.NetworkErrorType> callback)
        {
            var serverMessage = new ServerMessage(ServerMessageType.GetType, "score/get/winners_gamescore");
            Api.SendMessage(serverMessage, (json, error) =>
            {
                if (json == null || json.Contains("error") || json == "timeout")
                {
                    callback(null, error);
                    return;
                }

                // Debug.Log(json);

                var winners = JsonUtility.FromJson<Winners>(json);
                callback(winners, error);
            });
        }

    }
}