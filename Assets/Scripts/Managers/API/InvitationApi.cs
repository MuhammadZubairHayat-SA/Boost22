using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers.API
{
    public class InvitationApi
    {
        public static void Send (string invitedEmailAdress, string inviterName, string friendName, Action<bool, Enums.NetworkErrorType> callback)
        {
            var data = new Dictionary<string, object>();
			data["invitedEmailAddress"] = invitedEmailAdress;
			data["inviterName"] = inviterName;
			data["friendName"] = friendName;
			
			var serverMessage = new ServerMessage(ServerMessageType.PostType, "invite/email", data);
			Api.SendMessage(serverMessage, (invitationJson, error) =>
			{
				Debug.Log(invitationJson);

				if (invitationJson.Contains("email already invited"))
				{
					callback(false, Enums.NetworkErrorType.EmailAlreadyInvited);
                    return;
				}

				if (invitationJson == null || invitationJson.Contains("error") || invitationJson == "timeout")
				{
					callback(false, error);
					return;
				}

				callback(true, error);
			});
        }
	}
}