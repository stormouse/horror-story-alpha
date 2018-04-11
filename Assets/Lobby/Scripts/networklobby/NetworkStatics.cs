using UnityEngine;
using UnityEngine.Networking;


public static class GameNetworkMsg
{
    public static short SyncLobbyState = MsgType.Highest + 10;
}

public static class LocalPlayerInfo
{
    public static string playerName = "UNKNOWN";  // this is used before playerObject is even spawned, 
                                                  // therefore not redundant
    public static GameObject playerObject;
    public static NetworkCharacter playerCharacter;
}

