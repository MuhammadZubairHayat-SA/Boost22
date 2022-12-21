using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum GameEventType { NoType = 0, ClosedEvent = 1, RushEventWinnerTakesItAll = 1001 };

    public enum GameType { Boost22 = 1, Turbo = 2 };

    public enum GameModeType { League = 1, Rush22 = 2, Playground = 3 };

    public enum PaymentMethodType { MobilePay = 1, AmazonGiftCard = 2 };

    public enum PointType
    {
        Boost22_1st = 1, Boost22_2nd = 2, Boost22_3rd = 3, Boost22_4th = 4,
        Turbo_1st = 11, Turbo_2nd = 12, Turbo_3rd = 13, Turbo_4th = 14,
        CleanSheet = 1000, FourOfAKind = 1001, KnockOut2 = 1002, KnockOut3 = 1003,
        ReadyForPlayingEvent = 3001
    }

    public enum TrendType { Up = 1, Down = 2, Unchanged = 3 };

    public enum GameStateType { HighAceMode, SwitchCardsMode, PlayMode };

    public enum PlayerType { User, Robot };

    public enum RobotType { Easy, Medium, Hard };

    public enum CardType {
        noCard = -1,
        clubs2 = 8,
        diamonds2 = 9,
        hearts2 = 10,
        spades2 = 11,
        clubs3 = 12,
        diamonds3 = 13,
        hearts3 = 14,
        spades3 = 15,
        clubs4 = 16,
        diamonds4 = 17,
        hearts4 = 18,
        spades4 = 19,
        clubs5 = 20,
        diamonds5 = 21,
        hearts5 = 22,
        spades5 = 23,
        clubs6 = 24,
        diamonds6 = 25,
        hearts6 = 26,
        spades6 = 27,
        clubs7 = 28,
        diamonds7 = 29,
        hearts7 = 30,
        spades7 = 31,
        clubs8 = 32,
        diamonds8 = 33,
        hearts8 = 34,
        spades8 = 35,
        clubs9 = 36,
        diamonds9 = 37,
        hearts9 = 38,
        spades9 = 39,
        clubs10 = 40,
        diamonds10 = 41,
        hearts10 = 42,
        spades10 = 43,
        clubs11 = 44,
        diamonds11 = 45,
        hearts11 = 46,
        spades11 = 47,
        clubs12 = 48,
        diamonds12 = 49,
        hearts12 = 50,
        spades12 = 51,
        clubs13 = 52,
        diamonds13 = 53,
        hearts13 = 54,
        spades13 = 55,
        clubs14 = 56,
        diamonds14 = 57,
        hearts14 = 58,
        spades14 = 59
    };

    private static PointType[] GetBoost22Positions() {
        // return new PointType[4] { PointType.Boost22_1st, PointType.Boost22_2nd, PointType.Boost22_3rd, PointType.Boost22_4th };
        return new PointType[4] { PointType.Boost22_4th, PointType.Boost22_3rd, PointType.Boost22_2nd, PointType.Boost22_1st };
    }

    private static PointType[] GetTurbo22Positions()
    {
        // return new PointType[4] { PointType.Turbo_1st, PointType.Turbo_2nd, PointType.Turbo_3rd, PointType.Turbo_4th };
        return new PointType[4] { PointType.Turbo_4th, PointType.Turbo_3rd, PointType.Turbo_2nd, PointType.Turbo_1st };
    }

    public static PointType[] GetGamePositions(GameType gameType)
    {
        if (gameType == GameType.Boost22)
        {
            return GetBoost22Positions();
        }
        else
        {
            return GetTurbo22Positions();
        }
    }

    public static int CardTypeToInt (CardType cardType)
	{
		return Mathf.FloorToInt((int)cardType / 4.0f);
	}

	public enum NetworkErrorType
	{
		NoError,
		NoNet,
		NoTimestampReturned,
		ApiCallError,
		TimeOut,
		WrongApiVersion,
		UsernameNotUnique,
		EmailInvalid,
		EmailNotFound,
		EmailNotUnique,
		AccountNotFound,
		Maintenance,
		WrongOldPassword,
        EmailAlreadyInvited,
        EventIsClosed,
        EventCanOnlyBePlayedFromOneDevice,
        EventNotFound,
        EventTypeIsNotSupported,
        NoGamesLeftToday,
        UnknownError
	}
}
