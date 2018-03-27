using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using GameEnum;

public class LobbyPlayer : NetworkLobbyPlayer
{
    /* shortcut to localplayer */
    private static LobbyPlayer s_localPlayer = null;
    public static LobbyPlayer localPlayer { get { return s_localPlayer; } }

    /* player public properties */
    [SyncVar(hook = "OnSwitchTeam")]
    public TeamType team;
    public new string name;

    [Space]

    /* player UI */
    public Text nameText;
    public Button joinSpectatorBtn;
    public Button joinHunterBtn;
    public Button joinSurvivorBtn;
    public Button addHunterAiBtn;
    public Button addSurvivorAiBtn;
    public Button readyBtn;


    public override void OnStartLocalPlayer()
    {
        s_localPlayer = this;
        SetupUIComponents();
    }


    private void SetupUIComponents()
    {

        RoomUI.singleton.gameObject.SetActive(true);

        // fetch button references
        readyBtn = RoomUI.singleton.readyBtn;
        joinSpectatorBtn = RoomUI.singleton.joinSpectatorBtn;
        joinHunterBtn = RoomUI.singleton.joinHunterBtn;
        joinSurvivorBtn = RoomUI.singleton.joinSurvivorBtn;
        addHunterAiBtn = RoomUI.singleton.addHunterAiBtn;
        addSurvivorAiBtn = RoomUI.singleton.addSurvivorAiBtn;

        // show/hide add ai button
        if (isServer)
        {
            addHunterAiBtn.gameObject.SetActive(true);
            addSurvivorAiBtn.gameObject.SetActive(true);
            // bind listeners
            addHunterAiBtn.onClick.AddListener(OnClickAddHunterAiButton);
            addSurvivorAiBtn.onClick.AddListener(OnClickAddSurvivorAiButton);
        }
        else
        {
            addHunterAiBtn.gameObject.SetActive(false);
            addSurvivorAiBtn.gameObject.SetActive(false);
            // unbind listeners
            addHunterAiBtn.onClick.RemoveAllListeners();
            addSurvivorAiBtn.onClick.RemoveAllListeners();
        }

        // bind other listeners
        readyBtn.onClick.AddListener(OnClickReadyButton);
        joinSpectatorBtn.onClick.AddListener(OnClickJoinSpectatorButton);
        joinHunterBtn.onClick.AddListener(OnClickJoinHunterButton);
        joinSurvivorBtn.onClick.AddListener(OnClickJoinSurvivorButton);
        
    }



    void OnClickReadyButton()
    {
        if (readyToBegin)
            OnClickNotReady();
        else
            OnClickReady();
    }


    void OnClickJoinSpectatorButton()
    {

    }

    void OnClickJoinHunterButton()
    {
        CmdSwitchTeam(TeamType.Hunter);
    }

    void OnClickJoinSurvivorButton()
    {
        CmdSwitchTeam(TeamType.Survivor);
    }

    void OnClickAddHunterAiButton()
    {
        AddHunterAI();
    }

    void OnClickAddSurvivorAiButton()
    {
        AddSurvivorAI();
    }


    //ServerOnly
    void AddHunterAI()
    {
        LobbyManager.Singleton.AddHunterAI();
        RpcAddHunterAI();
    }

    [ClientRpc]
    void RpcAddHunterAI()
    {
        if (!isServer)
        {
            LobbyManager.Singleton.AddHunterAI();
        }
    }


    //ServerOnly
    void AddSurvivorAI()
    {
        LobbyManager.Singleton.AddSurvivorAI();
        RpcAddSurvivorAI();
    }
    
    [ClientRpc]
    void RpcAddSurvivorAI()
    {
        if (!isServer)
        {
            LobbyManager.Singleton.AddSurvivorAI();
        }
    }

    [Command]
    void CmdSwitchTeam(TeamType _team)
    {
        Debug.Log("Change " + team.ToString() + " to " + _team.ToString());
        if (team == _team) return;
        if (!LobbyManager.Singleton.TeamIsFull(_team))
        {
            LobbyManager.Singleton.SwitchToTeam(this, _team);
        }
    }

    //[Hook]
    void OnSwitchTeam(TeamType _team)
    {
        team = _team;
        LobbyManager.Singleton.UpdateSlots();
    }


    void OnClickReady()
    {
        SendReadyToBeginMessage();
    }


    void OnClickNotReady()
    {
        SendNotReadyToBeginMessage();
    }


    [ClientRpc]
    public void RpcReadyForPlayScene()
    {
        var canvas = GameObject.Find("Lobby UI").GetComponent<Canvas>();
        if (canvas) canvas.enabled = false;
    }


    // Client
    public override void OnClientReady(bool ready)
    {
        if (ready)
        {
            // ChangeReadyButtonColor(Gray);
            // readyButton.interactable = false;
            // nameInput.interactable = false;
        }
        else
        {
            // ChangeReadyButtonColor(isLocalPlayer ? Red : Blue);
            if (isLocalPlayer)
            {
                // readyButton.interactable = true;
                // nameInput.interactable = true;
            }
        }
        LobbyManager.Singleton.UpdateSlots();
    }


    public override void OnClientEnterLobby()
    {
        if (isLocalPlayer)
        {
            readyToBegin = false;
            LobbyManager.Singleton.UpdateSlots();
            // readyBtn.interactable = true;
            // ChangeReadyButtonColor(Red);
        }
        else
        {
            readyToBegin = false;
            // ChangeReadyButtonColor(Blue);
        }
    }


    public override void OnClientExitLobby()
    {
        LobbyManager.Singleton.UpdateSlots();
    }
    
}
