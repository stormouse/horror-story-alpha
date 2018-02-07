using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    public Button hostBtn;
    public Button joinBtn;
    public InputField ipInput;
    public LobbyManager lobbyManager;

    private void Start()
    {
        hostBtn.onClick.AddListener(StartHostCallback);
        joinBtn.onClick.AddListener(StartClientCallback);
    }

    void StartHostCallback()
    {
        lobbyManager.StartHost();
    }

    void StartClientCallback()
    {
        string ip = ipInput.text;
        int port = 7777;
        lobbyManager.networkAddress = ip;
        lobbyManager.networkPort = port;
        lobbyManager.StartClient();
    }


}
