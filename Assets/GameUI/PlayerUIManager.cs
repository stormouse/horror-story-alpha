using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour {

    private static PlayerUIManager s_singleton;
    public static PlayerUIManager singleton { get { return s_singleton; } }
    private GameEnum.TeamType playerTeam;

    public GameObject UIContainer;
    public GameObject objectivePanel;

    public Sprite[] hunterSkillSprites;
    public Sprite[] survivorSkillSprites;
    public Sprite[] hunterSkillKeys;
    public Sprite[] survivorSkillKeys;
    public Sprite skillExhaustedSprite;

    public Image[] skillImage;
    public Image[] cooldownImage;
    public Image[] skillKeyImage;
    public Text[] itemCount;
    public Text[] objectiveIndicator;


    public Color noPowerColor = new Color(0.97255f, 0.49412f, 0.49412f);
    public Color fullPowerColor = new Color(0.52941f, 0.97255f, 0.49412f);



    bool bObjectivePanelActive;


    void Awake()
    {
        s_singleton = this;
        bObjectivePanelActive = false;
        objectivePanel.SetActive(false);
    }


    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            UpdateSurvivorCount(LevelManager.Singleton.SurvivorCount);
            UpdateBatteryCount(LevelManager.Singleton.PowerSourceCount);
            if (!bObjectivePanelActive)
            {
                bObjectivePanelActive = true;
                objectivePanel.SetActive(true);
            }
        }
        else
        {
            if (bObjectivePanelActive)
            {
                bObjectivePanelActive = false;
                objectivePanel.SetActive(false);
            }
        }

        if (LocalPlayerInfo.playerCharacter)
        {
            UpdateCompassOffset();
        }
    }


    public void Initialize()
    {
        playerTeam = LocalPlayerInfo.playerCharacter.Team;

        if (playerTeam == GameEnum.TeamType.Hunter)
        {
            for (int i = 0; i < 2; i++)
            {
                skillImage[i].sprite = hunterSkillSprites[i];
                skillKeyImage[i].sprite = hunterSkillKeys[i];
                itemCount[i].gameObject.SetActive(false);
            }
        }
        else if(playerTeam == GameEnum.TeamType.Survivor)
        {
            for (int i = 0; i < 2; i++)
            {
                skillImage[i].sprite = survivorSkillSprites[i];
                skillKeyImage[i].sprite = survivorSkillKeys[i];
            }
        }
        else
        {
            UIContainer.SetActive(false);
        }
        foreach (var img in cooldownImage)
        {
            img.enabled = false;
        }

        UpdateSurvivorCount(LevelManager.Singleton.SurvivorCount);
        UpdateBatteryCount(LevelManager.Singleton.PowerSourceCount);
        UpdateOpenExitCount(0, 3);
        var countableSlots = LocalPlayerInfo.playerObject.GetComponent<ICountableSlots>();
        for(int i = 0; i < 4; i++)
        {
            UpdateItemCount(i, countableSlots.GetCountOfIndex(i));
        }
        // UpdateToyCarCount(toyCarCount);
    }


    public void UpdateSurvivorCount(int count)
    {
        objectiveIndicator[0].text = count.ToString();
    }


    public void UpdateBatteryCount(int count)
    {
        objectiveIndicator[1].text = count.ToString();
        if(count <= 3)
        {
            UpdateOpenExitCount(3, 3);
        }
    }


    public void UpdateOpenExitCount(int count, int total)
    {
        string tmp = "";
        for (int i = 0; i < count; i++)
            tmp += "a";
        for (int i = count; i < total; i++)
            tmp += "X";
        float r = (float)count / (float)total;
        objectiveIndicator[2].text = tmp;
        objectiveIndicator[2].color = Color.Lerp(noPowerColor, fullPowerColor, r);
    }


    public void UpdateToyCarCount(int count)
    {
        objectiveIndicator[3].text = count.ToString();
    }


    public void UpdateItemCount(int index, int count)
    {
        if (index >= itemCount.Length) return;

        itemCount[index].text = string.Format("w{0}", count);
        if(count == 0)
        {
            skillImage[index].sprite = skillExhaustedSprite;
            skillImage[index].color = Color.gray;
        }
    }


    public void EnterCooldown(int index, float time)
    {
        StartCoroutine(CooldownCoroutine(index, time));  
    }


    IEnumerator CooldownCoroutine(int index, float timeLength)
    {
        cooldownImage[index].enabled = true;
        float startTime = Time.time;
        float now = startTime;
        while(now - startTime < timeLength)
        {
            now = Time.time;
            float r = (now - startTime) / timeLength;
            cooldownImage[index].fillAmount = Mathf.Lerp(1, 0, r);
            yield return new WaitForEndOfFrame();
        }
    }





    #region Compass Calculation

    public Image compass;
    float leftLimit = -24.0f;
    float rightLimit = -756.0f;

    void UpdateCompassOffset()
    {
        //var angle = LocalPlayerInfo.playerObject.transform.eulerAngles.y;
        var angle = Camera.main.transform.eulerAngles.y;
        var size = (leftLimit - rightLimit) / 360.0f;
        float x = -angle * size + leftLimit; // -0.0f = -24.0f
        if(x < rightLimit)
        {
            x = x - rightLimit + leftLimit;
        }
        float old_x = compass.rectTransform.anchoredPosition.x;
        compass.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(old_x, x, 0.8f), 0.0f);
    }

    #endregion Compass Calculation

}
