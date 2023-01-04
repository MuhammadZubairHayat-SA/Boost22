using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using System;

public class UserManager
{

    [CanBeNull] private static User _loggedInUser;

    [CanBeNull]
    public static User LoggedInUser
    {
        get
        {
            if (_loggedInUser != null)
            {
                return _loggedInUser;
            }

            var userJson = PlayerPrefs.GetString("user");
            if (userJson != null && !userJson.Equals(""))
            {
                var user = JsonUtility.FromJson<User>(userJson);
                if (user != null && user.id > 0)
                {
                    _loggedInUser = user;
                    return user;
                }
            }

            return null;
        }
        set
        {
            _loggedInUser = value;
            PlayerPrefs.SetString("user", JsonUtility.ToJson(value));
            PlayerPrefs.Save();
        }
    }

    public static void LogOut()
    {
        _loggedInUser = null;
        PlayerPrefs.DeleteKey("user");
        PlayerPrefs.Save();
    }

    public static bool HasLoggedInUser()
    {
        return LoggedInUser != null;
    }

    public static bool HasLeagueGamesLeftToday ()
    {
        return LoggedInUser != null && LoggedInUser.gamesLeftToday + LoggedInUser.extraGamesLeftToday > 0;
    }

    // decreases the user's games counter by one (for display purposes)
    public static void AdjustLeagueGamesCount ()
    {
        if (LoggedInUser != null)
        {
            if (LoggedInUser.gamesLeftToday > 0)
            {
                LoggedInUser.gamesLeftToday--;
            }
            else if (LoggedInUser.extraGamesLeftToday > 0)
            {
                LoggedInUser.extraGamesLeftToday--;
                LoggedInUser.extraGames--;
            }
        }
    }

    public static IEnumerable<Player> GetRobotPlayers(Enums.GameType gameType)
    {
        var robots = new List<Player>();

        /*if (LoggedInUser != null)
        {
            foreach (var robotType in LoggedInUser.robotTypes)
            {
                robots.Add(new RobotPlayer(RandomAiUser(robotType, robots.Select(r => r.user.id).ToList()), robotType));
            }
        }
        else
        {*/
            robots.Add(new RobotPlayer(RandomAiUser(Enums.RobotType.Hard, robots.Select(r => r.user.id).ToList()), Enums.RobotType.Hard));
            robots.Add(new RobotPlayer(RandomAiUser(Enums.RobotType.Hard, robots.Select(r => r.user.id).ToList()), Enums.RobotType.Hard));
            robots.Add(new RobotPlayer(RandomAiUser(Enums.RobotType.Hard, robots.Select(r => r.user.id).ToList()), Enums.RobotType.Hard));
        //}

        robots.Shuffle();
        return robots;
    }

    private static User RandomAiUser(Enums.RobotType robotType, ICollection<int> notInIds)
    {
        var ais = AIs.Where(u => u.robotType == robotType && !notInIds.Contains(u.id));
        
        return ais.ToList()[UnityEngine.Random.Range(0, ais.Count() - 1)];
    }

    private static readonly User[] AIs =
    {
        new User
        {
            id = 1,
            username = "Martin W",
            robotType = Enums.RobotType.Easy
        },
        new User
        {
            id = 2,
            username = "Ms. Laura",
            robotType = Enums.RobotType.Easy
        },
        new User
        {
            id = 3,
            username = "FunkyJane",
            robotType = Enums.RobotType.Easy
        },
        new User
        {
            id = 4,
            username = "Horseman",
            robotType = Enums.RobotType.Easy
        },
        new User
        {
            id = 5,
            username = "Not Lisa",
            robotType = Enums.RobotType.Easy
        },
        new User
        {
            id = 6,
            username = "CamillaR",
            robotType = Enums.RobotType.Easy
        },
        new User
        {
            id = 7,
            username = "PowerSara",
            robotType = Enums.RobotType.Easy
        },
        new User
        {
            id = 8,
            username = "LarsDK",
            robotType = Enums.RobotType.Easy
        },
        new User
        {
            id = 9,
            username = "Crazy22",
            robotType = Enums.RobotType.Easy
        },
        new User
        {
            id = 10,
            username = "Simon E",
            robotType = Enums.RobotType.Easy
        },
        new User
        {
            id = 11,
            username = "Clara23",
            robotType = Enums.RobotType.Medium
        },
        new User
        {
            id = 12,
            username = "Henrik34",
            robotType = Enums.RobotType.Medium
        },
        new User
        {
            id = 13,
            username = "Sport33",
            robotType = Enums.RobotType.Medium
        },
        new User
        {
            id = 14,
            username = "AL 1989",
            robotType = Enums.RobotType.Medium
        },
        new User
        {
            id = 15,
            username = "Oliver G",
            robotType = Enums.RobotType.Medium
        },
        new User
        {
            id = 16,
            username = "Katja",
            robotType = Enums.RobotType.Medium
        },
        new User
        {
            id = 17,
            username = "MaxM",
            robotType = Enums.RobotType.Medium
        },
        new User
        {
            id = 18,
            username = "Sea You",
            robotType = Enums.RobotType.Medium
        },
        new User
        {
            id = 19,
            username = "Dr. 22",
            robotType = Enums.RobotType.Medium
        },
        new User
        {
            id = 20,
            username = "EmmaS",
            robotType = Enums.RobotType.Medium
        },
        new User
        {
            id = 21,
            username = "CardMan 2100",
            robotType = Enums.RobotType.Hard
        },
        new User
        {
            id = 22,
            username = "Boost Beast",
            robotType = Enums.RobotType.Hard
        },
        new User
        {
            id = 23,
            username = "Top 2",
            robotType = Enums.RobotType.Hard
        },
        new User
        {
            id = 24,
            username = "Never 22",
            robotType = Enums.RobotType.Hard
        },
        new User
        {
            id = 25,
            username = "The Winner",
            robotType = Enums.RobotType.Hard
        },
        new User
        {
            id = 26,
            username = "KC Master",
            robotType = Enums.RobotType.Hard
        },
        new User
        {
            id = 27,
            username = "Mads The Man",
            robotType = Enums.RobotType.Hard
        },
        new User
        {
            id = 28,
            username = "Denise",
            robotType = Enums.RobotType.Hard
        },
        new User
        {
            id = 29,
            username = "HH 2019",
            robotType = Enums.RobotType.Hard
        },
        new User
        {
            id = 30,
            username = "Taking Over",
            robotType = Enums.RobotType.Hard
        }
    };
}

public static class Extensions {

    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);//rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
