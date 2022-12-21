using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
public enum ServerMessageType
{
    GetType,
    PostType,
    DeleteType
}

public class ServerMessage
{
    public ServerMessageType type = ServerMessageType.GetType;
    public string url;
    public Dictionary<string, object> data;

    public ServerMessage(ServerMessageType type, string url, Dictionary<string, object> data = null)
    {
        this.type = type;
        this.url = url;
        this.data = data;
    }
}