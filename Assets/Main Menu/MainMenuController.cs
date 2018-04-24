using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public Button startButton;
    public Button instructionsButton;
    public Button creditButton;
    public Button quitButton;

    public GameObject instructionsPage;
    public Button closeInstructionButton;
    public GameObject creditPage;
    public Button closeCreditButton;
    

    void Start () {
        startButton.onClick.AddListener(ChangeScene);
        instructionsButton.onClick.AddListener(ShowInstructions);
        creditButton.onClick.AddListener(ShowCredit);
        quitButton.onClick.AddListener(QuitGame);
        closeInstructionButton.onClick.AddListener(HideInstructions);
        closeCreditButton.onClick.AddListener(HideCredit);
    }
	

    void ChangeScene()
    {
        SceneManager.LoadScene(GameScene.LobbyScene);
    }

    void ShowInstructions()
    {
        creditPage.SetActive(false);
        instructionsPage.SetActive(true);
    }

    void HideInstructions()
    {
        instructionsPage.SetActive(false);
    }

    void ShowCredit()
    {
        instructionsPage.SetActive(false);
        creditPage.SetActive(true);
    }
    
    void HideCredit()
    {
        creditPage.SetActive(false);
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
