using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    public Button hostBtn;
    public Button joinBtn;
    public InputField playerNameInput;
    public InputField ipInput;
    public LobbyManager lobbyManager;

    private void Start()
    {
        playerNameInput.text = "PLAYER" + Random.Range(1, 100).ToString();
        hostBtn.onClick.AddListener(StartHostCallback);
        joinBtn.onClick.AddListener(StartClientCallback);
    }

    void StartHostCallback()
    {
        LocalPlayerInfo.playerName = playerNameInput.text;
        lobbyManager.StartHost(); // no need to explicitly set ip:port
    }

    void StartClientCallback()
    {
        string ip = ipInput.text;
        int port = 7777;

        LocalPlayerInfo.playerName = playerNameInput.text;
        lobbyManager.networkAddress = ip;
        lobbyManager.networkPort = port;
        lobbyManager.StartClient();
    }


}
