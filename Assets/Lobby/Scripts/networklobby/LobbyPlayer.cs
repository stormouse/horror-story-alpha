using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using GameEnum;

public class LobbyPlayer : NetworkLobbyPlayer
{
    /* player public properties */
    [SyncVar(hook = "OnSwitchTeam")]
    public TeamType team;
    public new string name;

    [Space]

    /* player UI */
    public Text nameText;
    public Button selectSpectatorBtn;
    public Button selectHunterBtn;
    public Button selectSurvivorBtn;
    public Button readyBtn;


    private void OnGUI()
    {
        if (!isLocalPlayer)
            return;

        if (GUI.Button(new Rect(Screen.width - 200, Screen.height - 80, 150, 60), (readyToBegin ? "Cancel" : "Ready")))
        {
            if (readyToBegin)
                OnClickNotReady();
            else
                OnClickReady();
        }
        if (GUI.Button(new Rect(Screen.width - 400, Screen.height - 80, 150, 60), "Hunter"))
        {
            CmdSwitchTeam(TeamType.Hunter);
        }
        if (GUI.Button(new Rect(Screen.width - 600, Screen.height - 80, 150, 60), "Survivor"))
        {
            CmdSwitchTeam(TeamType.Survivor);
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
