using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoomUI : MonoBehaviour {
    public static RoomUI singleton = null;

    public Text nameText;
    public Button joinSpectatorBtn;
    public Button joinHunterBtn;
    public Button joinSurvivorBtn;
    public Button addHunterAiBtn;
    public Button removeHunterAiBtn;
    public Button addSurvivorAiBtn;
    public Button removeSurvivorAiBtn;
    public Button readyBtn;
    //public Button quitRoomBtn;

    public Image[] hunterImages = new Image[2];
    public Image[] survivorImages = new Image[4];
    public Image[] spectatorImages = new Image[2];
    public Image[] survivorReadyIndicators = new Image[4];
    public Image[] hunterReadyIndicators = new Image[2];
    public Image[] spectatorReadyIndicators = new Image[2];
    public Text[] survivorNameTexts = new Text[4];
    public Text[] hunterNameTexts = new Text[2];
    public Text[] spectatorNameTexts = new Text[2];


    public Sprite hunterAvatar;
    public Sprite survivorAvatar;
    public Sprite emptySlotAvatar;

    private void Awake()
    {
        singleton = this;
    }

    //private void Start()
    //{
    //    quitRoomBtn.onClick.AddListener(QuitRoom);
    //}

    //void QuitRoom()
    //{
    //    Destroy(LobbyManager.Singleton.gameObject);

    //    if (Network.isClient)
    //    {
    //        LobbyManager.singleton.client.Disconnect();
    //        LobbyManager.singleton.client.Shutdown();
    //    }
    //    else if (Network.isServer)
    //    {
    //        NetworkManager.singleton.StopClient();
    //        NetworkManager.singleton.StopHost();
    //        NetworkManager.singleton.StopMatchMaker();
    //        Network.Disconnect();
    //    }

    //    if (LobbyPlayer.localPlayer)
    //    {
    //        Destroy(LobbyPlayer.localPlayer.gameObject);
    //    }

    //    SceneManager.LoadScene(GameScene.MainMenu);
    //}
}
