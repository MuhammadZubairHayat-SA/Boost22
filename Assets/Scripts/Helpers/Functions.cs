using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Functions
{
    private const string uuidPattern = "^[0-9A-Fa-f\\-]{12}";

    public static string GetInviteTokenFromClipboard()
    {
        string clipboardContent = UniClipboard.GetText();

        if (clipboardContent == null)
        {
            return null;
        }
        var regex = new Regex(uuidPattern);
        if (!regex.IsMatch(clipboardContent))
        {
            clipboardContent = null;
        }
        return clipboardContent;
    }
}