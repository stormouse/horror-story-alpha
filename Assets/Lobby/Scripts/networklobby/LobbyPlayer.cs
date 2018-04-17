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
    public string playerName;

    [Space]

    /* player UI */
    public Text nameText;
    public Button joinSpectatorBtn;
    public Button joinHunterBtn;
    public Button joinSurvivorBtn;
    public Button addHunterAiBtn;
    public Button addSurvivorAiBtn;
    public Button removeHunterAiBtn;
    public Button removeSurvivorAiBtn;
    public Button readyBtn;
    public Image readyIndicator;

    public bool m_ready;


    public override void OnStartLocalPlayer()
    {
        if (s_localPlayer == null)
        {
            s_localPlayer = this;
            SetupUIComponents();
            CmdGivePlayerName(LocalPlayerInfo.playerName);
        }
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
        removeHunterAiBtn = RoomUI.singleton.removeHunterAiBtn;
        removeSurvivorAiBtn = RoomUI.singleton.removeSurvivorAiBtn;

        // show/hide add ai button
        if (isServer)
        {
            addHunterAiBtn.gameObject.SetActive(true);
            addSurvivorAiBtn.gameObject.SetActive(true);
            removeHunterAiBtn.gameObject.SetActive(true);
            removeSurvivorAiBtn.gameObject.SetActive(true);
            // bind listeners
            addHunterAiBtn.onClick.AddListener(OnClickAddHunterAiButton);
            addSurvivorAiBtn.onClick.AddListener(OnClickAddSurvivorAiButton);
            removeHunterAiBtn.onClick.AddListener(OnClickRemoveHunterAiButton);
            removeSurvivorAiBtn.onClick.AddListener(OnClickRemoveSurvivorAiButton);

        }
        else
        {
            addHunterAiBtn.gameObject.SetActive(false);
            addSurvivorAiBtn.gameObject.SetActive(false);
            removeHunterAiBtn.gameObject.SetActive(false);
            removeSurvivorAiBtn.gameObject.SetActive(false);
            // unbind listeners
            addHunterAiBtn.onClick.RemoveAllListeners();
            addSurvivorAiBtn.onClick.RemoveAllListeners();
            removeHunterAiBtn.onClick.RemoveAllListeners();
            removeSurvivorAiBtn.onClick.RemoveAllListeners();
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
        CmdSwitchTeam(TeamType.Spectator);
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
        LobbyManager.Singleton.AddHunterAI();
    }

    void OnClickAddSurvivorAiButton()
    {
        LobbyManager.Singleton.AddSurvivorAI();
    }

    void OnClickRemoveHunterAiButton()
    {
        LobbyManager.Singleton.RemoveHunterAI();
    }

    void OnClickRemoveSurvivorAiButton()
    {
        LobbyManager.Singleton.RemoveSurvivorAI();
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
    }


    void OnClickReady()
    {
        //SendReadyToBeginMessage();
        CmdSetReady(true);
    }


    void OnClickNotReady()
    {
        //SendNotReadyToBeginMessage();
        CmdSetReady(false);
    }

    public void NotReady()
    {
        CmdSetReady(false);
    }

    [Command]
    void CmdSetReady(bool ready)
    {
        //readyToBegin = ready;
        m_ready = ready;
        RpcSetReady(ready);
        
        if (ready)
        {
            LobbyManager.Singleton.CustomServerCheckReadyState();
        }
        else
        {
            LobbyManager.Singleton.PushLobbyStateToClient();
        }
    }

    [ClientRpc]
    void RpcSetReady(bool ready)
    {
        if (!isServer)
        {
            //readyToBegin = ready;
            m_ready = ready;
        }
    }
    

    [Command]
    void CmdGivePlayerName(string name)
    {
        playerName = name;
        RpcGivePlayerName(name);

        LobbyManager.Singleton.UpdateSlots();
    }

    [ClientRpc]
    void RpcGivePlayerName(string name)
    {
        playerName = name;
    }

}
