using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour {
    public static RoomUI singleton = null;

    public Text nameText;
    public Button joinSpectatorBtn;
    public Button joinHunterBtn;
    public Button joinSurvivorBtn;
    public Button addHunterAiBtn;
    public Button addSurvivorAiBtn;
    public Button readyBtn;

    public Image[] hunterImages = new Image[2];
    public Image[] survivorImages = new Image[4];

    public Sprite hunterAvatar;
    public Sprite survivorAvatar;
    public Sprite emptySlotAvatar;

    private void Awake()
    {
        singleton = this;
    }
}
