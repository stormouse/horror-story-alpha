using UnityEngine;
using UnityEngine.Networking;


public static class GameNetworkMsg
{
    public static short SyncLobbyState = MsgType.Highest + 10;
}

public static class LocalPlayerInfo
{
    public static string playerName = "UNKNOWN";
}