using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour {

    private static PlayerUIManager s_singleton;
    public static PlayerUIManager singleton { get { return s_singleton; } }
    private GameEnum.TeamType playerTeam;

    public GameObject UIContainer;

    public Sprite[] hunterSkillSprites;
    public Sprite[] survivorSkillSprites;
    public Sprite skillExhaustedSprite;

    public Image[] skillImage;
    public Image[] cooldownImage;
    public Text[] itemCount;
    public Text[] objectiveIndicator;

    public Color noPowerColor = new Color(0.97255f, 0.49412f, 0.49412f);
    public Color fullPowerColor = new Color(0.52941f, 0.97255f, 0.49412f);

    private int survivorCount = 0;
    private int hunterCount = 0;
    private int batteryCount = 5;
    private int openExitCount = 0;
    private int toyCarCount = 0;


    string indexStr;
    string cooldownTimeStr;
    string numItemStr;
    string teammateCountStr;
    string batteryCountStr;
    string openExitCountStr;
    string toyCarCountStr;

    private void OnGUI()
    {
        return;

        // debug use
        GUILayout.Label("Skill Index");
        indexStr = GUILayout.TextField(indexStr);
        GUILayout.Label("Cooldown");
        cooldownTimeStr = GUILayout.TextField(cooldownTimeStr);
        GUILayout.Label("Item Count");
        numItemStr = GUILayout.TextField(numItemStr);

        if (GUILayout.Button("Cast"))
        {
            int i, count;
            float time;
            int.TryParse(indexStr, out i);
            float.TryParse(cooldownTimeStr, out time);
            int.TryParse(numItemStr, out count);

            UpdateItemCount(i, count);
            EnterCooldown(i, time);
        }

        GUILayout.Space(20);

        GUILayout.Label("Teammate Count");
        teammateCountStr = GUILayout.TextField(teammateCountStr);
        GUILayout.Label("Battery Count");
        batteryCountStr = GUILayout.TextField(batteryCountStr);
        GUILayout.Label("Open Exits");
        openExitCountStr = GUILayout.TextField(openExitCountStr);
        GUILayout.Label("Toy Car Count");
        toyCarCountStr = GUILayout.TextField(toyCarCountStr);

        if (GUILayout.Button("Update"))
        {
            int teammateCount;
            int batteryCount;
            int openExitCount;
            int toyCarCount;

            int.TryParse(teammateCountStr, out teammateCount);
            int.TryParse(batteryCountStr, out batteryCount);
            int.TryParse(openExitCountStr, out openExitCount);
            int.TryParse(toyCarCountStr, out toyCarCount);

            UpdateSurvivorCount(teammateCount);
            UpdateBatteryCount(batteryCount);
            UpdateOpenExitCount(openExitCount, 3);
            UpdateToyCarCount(toyCarCount);
        }
    }


    void Awake()
    {
        s_singleton = this;
    }


    public void Initialize()
    {
        foreach (var player in FindObjectsOfType<NetworkCharacter>())
        {
            if (player.isLocalPlayer)
            {
                playerTeam = player.Team;
            }
            if (player.Team == GameEnum.TeamType.Survivor)
            {
                survivorCount += 1;
            }
            else if (playerTeam == GameEnum.TeamType.Hunter)
            {
                hunterCount += 1;
            }
        }

        if (playerTeam == GameEnum.TeamType.Hunter)
        {
            for (int i = 0; i < 4; i++)
            {
                skillImage[i].sprite = hunterSkillSprites[i];
            }
        }
        else if(playerTeam == GameEnum.TeamType.Survivor)
        {
            for (int i = 0; i < 4; i++)
            {
                skillImage[i].sprite = survivorSkillSprites[i];
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

        UpdateSurvivorCount(survivorCount);
        UpdateBatteryCount(batteryCount);
        UpdateOpenExitCount(openExitCount, 3);
        UpdateToyCarCount(toyCarCount);
    }


    public void UpdateSurvivorCount(int count)
    {
        objectiveIndicator[0].text = count.ToString();
    }


    public void UpdateBatteryCount(int count)
    {
        objectiveIndicator[1].text = count.ToString();
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
        itemCount[index].text = string.Format("w {0}", count);
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


    

}
