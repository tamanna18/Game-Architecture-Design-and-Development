using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
// public enum RewardType { None, DamageBoost, DefenceBoost, HealthBoost, Heal, WaveSizeIncrease, TimeBoost, Upgrade, Unlock };
public enum RewardType { None, DamageBoost, DefenceBoost, HealthBoost, Heal, WaveSizeIncrease, TimeBoost, Upgrade, Unlock };
public class RewardSystem : MonoBehaviour
{
    static RewardSystem instance;
    public static RewardSystem Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<RewardSystem>();
            }
            return instance;
        }
    }
    private static List<RewardType> excludeRewardsLeft = new List<RewardType> { RewardType.None };
    private static List<RewardType> bonusRewardMinigames = new List<RewardType> { RewardType.DamageBoost, RewardType.DefenceBoost, RewardType.Heal, RewardType.HealthBoost, RewardType.TimeBoost };
    private static List<RewardType> excludeRewardsRight = new List<RewardType> { RewardType.None };
    public Sprite[] rewardSprites;
    public GameObject chooseRewardPanelLeft;
    public GameObject chooseRewardPanelRight;
    private RewardType miniGameLeftReward;
    private RewardType miniGameLeftPreviouisReward;
    private RewardType miniGameRightReward;
    private RewardType miniGameRightPreviouisReward;
    private RewardType currentRewardToGive;
    public float damageBoost = 0.3f;
    public float defenceBoost = 0.1f;
    public float healthBoost = 0.1f;
    public float healAmount = 15f;
    public bool[] unlockedMeleeMinions;
    public bool[] unlockedRangedMinions;
    public float timeBoost = 30f;
    public int timeBoostsCount = 0;
    public float resetTimer = 2f;
    private TextMeshProUGUI currentTimerBox;
    public bool isBonusPossibleLeft = false;
    public bool isBonusPossibleRight = false;

    public void InitializeRewards()
    {
        miniGameLeftPreviouisReward = miniGameLeftReward;
        miniGameRightPreviouisReward = miniGameRightReward;
        miniGameLeftReward = GenerateReward(GameIdentifier.MiniGameLeft);
        isBonusPossibleLeft = bonusRewardMinigames.Contains(miniGameLeftReward);
        miniGameRightReward = GenerateReward(GameIdentifier.MiniGameRight);
        isBonusPossibleRight = bonusRewardMinigames.Contains(miniGameRightReward);

    }
    public RewardType GenerateReward(GameIdentifier gameIdentifier)
    {
        chooseRewardPanelLeft.SetActive(false);
        chooseRewardPanelRight.SetActive(false);
        Image rewardImage = null;
        switch (gameIdentifier)
        {
            case GameIdentifier.MiniGameLeft:
                rewardImage = GameObject.Find("RewardImageLeft").GetComponent<Image>();
                break;
            case GameIdentifier.MiniGameRight:
                rewardImage = GameObject.Find("RewardImageRight").GetComponent<Image>();
                break;
        }
        var excludeRewards = gameIdentifier == GameIdentifier.MiniGameLeft ? excludeRewardsLeft : excludeRewardsRight;
        var rewardsList = System.Linq.Enumerable.Range(0, System.Enum.GetValues(typeof(RewardType)).Length).Where(i => filterRewards((RewardType)i, gameIdentifier, excludeRewards));
        int randomIndex = Random.Range(0, rewardsList.Count());
        RewardType randomReward = (RewardType)rewardsList.ElementAt(randomIndex);
        var randomNumber = Random.value;
        if (randomNumber <= 0.25f && randomReward != RewardType.Upgrade && rewardsList.Contains((int)RewardType.Upgrade)) randomReward = RewardType.Upgrade;
        if (randomNumber <= 0.65f && randomReward != RewardType.Unlock && rewardsList.Contains((int)RewardType.Unlock)) randomReward = RewardType.Unlock;
        switch (randomReward)
        {
            case RewardType.DamageBoost:
                rewardImage.sprite = rewardSprites[0];
                rewardImage.color = Color.red;
                break;
            case RewardType.DefenceBoost:
                rewardImage.sprite = rewardSprites[1];
                rewardImage.color = Color.blue;
                break;
            case RewardType.HealthBoost:
                rewardImage.sprite = rewardSprites[2];
                rewardImage.color = Color.green;
                break;
            case RewardType.Heal:
                rewardImage.sprite = rewardSprites[3];
                rewardImage.color = Color.green;
                break;
            case RewardType.Upgrade:
                rewardImage.sprite = rewardSprites[4];
                rewardImage.color = Color.white;
                break;
            case RewardType.Unlock:
                rewardImage.sprite = rewardSprites[5];
                rewardImage.color = Color.white;
                break;
            case RewardType.TimeBoost:
                rewardImage.sprite = rewardSprites[6];
                rewardImage.color = Color.yellow;
                break;
            case RewardType.WaveSizeIncrease:
                rewardImage.sprite = rewardSprites[7];
                rewardImage.color = Color.white;
                break;
        }
        return randomReward;
    }

    public bool filterRewards(RewardType element, GameIdentifier gameIdentifier, List<RewardType> excludeRewards)
    {
        return !excludeRewards.Contains(element) && element != miniGameLeftPreviouisReward && element != miniGameRightPreviouisReward && (gameIdentifier == GameIdentifier.MiniGameRight ? element != miniGameLeftReward : true);
    }

    public void GiveReward(GameIdentifier gameIdentifier)
    {
        GameManager.Instance.miniGamesWon++;
        if (gameIdentifier == GameIdentifier.MiniGameLeft)
        {
            currentRewardToGive = miniGameLeftReward;
        }
        else
        {
            currentRewardToGive = miniGameRightReward;

        }

        switch (currentRewardToGive)
        {
            case RewardType.DamageBoost:
                //Boost damage of all minions
                BoostDamage(gameIdentifier);
                break;
            case RewardType.DefenceBoost:
                //Boost defence of all minions
                BoostDefence(gameIdentifier);
                break;
            case RewardType.HealthBoost:
                //Boost health of all minions
                BoostHealth(gameIdentifier);
                break;
            case RewardType.Heal:
                //Heal of all minions
                Heal(gameIdentifier);
                break;
            case RewardType.Upgrade:
                //Upgrade next minion
                ShowChoicePanels(gameIdentifier);
                break;
            case RewardType.Unlock:
                //Unlock next minion
                ShowChoicePanels(gameIdentifier);
                break;
            case RewardType.TimeBoost:
                //Increase Timer    
                BoostTime();
                break;
            case RewardType.WaveSizeIncrease:
                //Increase Wave Size with 1
                IncreaseWaveSize(gameIdentifier);
                break;
        }
        StartCoroutine(PickTimer());
    }

    void BoostDamage(GameIdentifier gameIdentifier)
    {
        GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");
        foreach (GameObject minion in allMinions)
        {
            minion.GetComponent<MinionScript>().BoostDamage(getBoost(damageBoost), gameIdentifier);
        }
        MinionScript.IncreaseDamageBoostMultiplier(getBoost(damageBoost), gameIdentifier);
    }

    void BoostDefence(GameIdentifier gameIdentifier)
    {
        GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");
        foreach (GameObject minion in allMinions)
        {
            minion.GetComponent<MinionScript>().BoostDefence(getBoost(defenceBoost), gameIdentifier);
        }
        MinionScript.IncreaseDefenceBoostMultiplier(getBoost(defenceBoost), gameIdentifier);
    }

    void BoostHealth(GameIdentifier gameIdentifier)
    {
        MinionScript.IncreaseHealthBoostMultiplier(getBoost(healthBoost), gameIdentifier);
    }

    void Heal(GameIdentifier gameIdentifier)
    {
        GameObject[] allMinions = GameObject.FindGameObjectsWithTag("Minion");
        foreach (GameObject minion in allMinions)
        {
            minion.GetComponent<MinionScript>().Heal(getBoost(healAmount), gameIdentifier);
        }
    }

    void BoostTime()
    {
        GameManager.Instance.timer += getBoost(timeBoost);
        timeBoostsCount++;
        if (timeBoostsCount >= 4)
        {
            excludeRewardsLeft.Add(RewardType.TimeBoost);
            excludeRewardsRight.Add(RewardType.TimeBoost);
        }
    }

    void IncreaseWaveSize(GameIdentifier gameIdentifier)
    {
        if (gameIdentifier == GameIdentifier.MiniGameLeft)
        {
            MinionSpawner.Instance.undeadGolemWaveSize++;
            MinionSpawner.Instance.woodGolemWaveSize++;
            if (MinionSpawner.Instance.undeadGolemWaveSize >= 4)
            {
                excludeRewardsLeft.Add(RewardType.WaveSizeIncrease);
            }
        }
        else
        {
            MinionSpawner.Instance.archerGolemWaveSize++;
            MinionSpawner.Instance.wizardGolemWaveSize++;
            if (MinionSpawner.Instance.archerGolemWaveSize >= 3)
            {
                excludeRewardsRight.Add(RewardType.WaveSizeIncrease);
            }
        }

    }

    void ShowChoicePanels(GameIdentifier gameIdentifier)
    {
        resetTimer = 5f;
        GameManager.Instance.lockLeft.transform.GetChild(0).gameObject.SetActive(false);
        GameManager.Instance.lockRight.transform.GetChild(0).gameObject.SetActive(false);
        GameObject panel = gameIdentifier == GameIdentifier.MiniGameLeft ? chooseRewardPanelLeft : chooseRewardPanelRight;
        panel.SetActive(true);
        if (currentRewardToGive == RewardType.Upgrade)
        {
            panel.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>().sprite = rewardSprites[4];
            panel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Upgrade";
            ActivatePanelButtonsAndInfos(gameIdentifier, false);

        }
        else if (currentRewardToGive == RewardType.Unlock)
        {
            panel.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>().sprite = rewardSprites[5];
            panel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Unlock";
            ActivatePanelButtonsAndInfos(gameIdentifier, true);
        }
    }

    void ActivatePanelButtonsAndInfos(GameIdentifier gameIdentifier, bool isUnlock)
    {
        Transform buttonsBar;
        Transform infosBar;
        if (gameIdentifier == GameIdentifier.MiniGameLeft)
        {
            buttonsBar = chooseRewardPanelLeft.transform.GetChild(2);
            infosBar = chooseRewardPanelLeft.transform.GetChild(3);
            SetRewardPanelButtons(buttonsBar, infosBar, isUnlock, unlockedMeleeMinions, new int[] { 0, 1, 2 });
            currentTimerBox = chooseRewardPanelLeft.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        }
        else
        {
            buttonsBar = chooseRewardPanelRight.transform.GetChild(2);
            infosBar = chooseRewardPanelRight.transform.GetChild(3);
            SetRewardPanelButtons(buttonsBar, infosBar, isUnlock, unlockedRangedMinions, new int[] { 0, 3, 4 });
            currentTimerBox = chooseRewardPanelRight.transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        }

    }

    void SetRewardPanelButtons(Transform buttonsBar, Transform infosBar, bool isUnlock, bool[] unlockedMinions, int[] minionTypes)
    {
        for (int index = 0; index < buttonsBar.childCount; index++)
        {
            if ((isUnlock && unlockedMinions[index])
                || (!isUnlock && !unlockedMinions[index]
                || (!isUnlock && MinionSpawner.Instance.isUpgraded[minionTypes[index]] && (MinionSpawner.Instance.GetUpgradeLevel((MinionType)minionTypes[index]) >= 2))))
            {
                buttonsBar.GetChild(index).gameObject.SetActive(false);
                infosBar.GetChild(index).gameObject.SetActive(false);
            }
            else
            {
                buttonsBar.GetChild(index).gameObject.SetActive(true);
                buttonsBar.GetChild(index).GetChild(0).GetComponentInChildren<Image>().sprite = MinionSpawner.Instance.GetUpgradeSprite((MinionType)minionTypes[index]);
                infosBar.GetChild(index).gameObject.SetActive(true);
                MinionScript minionScript = MinionSpawner.Instance.GetMinionPrefabScript((MinionType)minionTypes[index]);
                string infoText = isUnlock
                ? string.Format("<color=green>HP: {0}\n</color><color=red>Att: {1}\n</color><color=blue>Def: {2}</color>", minionScript.maxHealth, minionScript.attackDamage, minionScript.defence)
                : string.Format("<color=green>HP: +{0}\n</color><color=red>Att: +{1}\n</color><color=blue>Def: +{2}</color>", minionScript.GetUpgradedHealth(), minionScript.GetUpgradedAttack(), minionScript.GetUpgradedDefence());
                infosBar.GetChild(index).GetChild(0).GetComponent<TextMeshProUGUI>().text = infoText;
            }
        }
        SetRewardButtonsInteractable(true);
    }

    public void RewardButtonClick(int minionType)
    {
        if (Instance.currentRewardToGive == RewardType.Unlock)
        {
            UnlockMinion(minionType);
        }
        else
        {
            UpgradeMinion(minionType);
        }
    }

    public void UnlockMinion(int minionType)
    {
        SetRewardButtonsInteractable(false);
        int minionIdentifier;
        switch ((MinionType)minionType)
        {
            case MinionType.UndeadGolem:
                unlockedMeleeMinions[1] = true;
                minionIdentifier = 2;
                break;
            case MinionType.WoodGolem:
                unlockedMeleeMinions[2] = true;
                minionIdentifier = 3;
                break;
            case MinionType.ArcherGolem:
                unlockedRangedMinions[1] = true;
                minionIdentifier = 4;
                break;
            case MinionType.WizardGolem:
                unlockedRangedMinions[2] = true;
                minionIdentifier = 5;
                break;
            default:
                minionIdentifier = 2;
                break;
        }
        GameObject.Find("MinionButton" + minionIdentifier).GetComponent<Button>().interactable = true;
        GameObject.Find("LockImage" + (minionIdentifier - 1)).SetActive(false);

        if (!excludeRewardsLeft.Contains(RewardType.Unlock) && Instance.unlockedMeleeMinions[1] == true && Instance.unlockedMeleeMinions[2] == true)
        {
            excludeRewardsLeft.Add(RewardType.Unlock);
        }
        else if (!excludeRewardsRight.Contains(RewardType.Unlock) && Instance.unlockedRangedMinions[1] == true && Instance.unlockedRangedMinions[2] == true)
        {
            excludeRewardsRight.Add(RewardType.Unlock);
        }
        SetUpgradeRewardAvailable();
    }

    public void UpgradeMinion(int minionType)
    {
        SetRewardButtonsInteractable(false);
        MinionSpawner.Instance.isUpgraded[minionType] = true;
        MinionSpawner.Instance.upgradeLevel[minionType]++;
        GameObject.Find("MinionButton" + (minionType + 1)).GetComponent<Button>().transform.GetChild(0).GetComponent<Image>().sprite = MinionSpawner.Instance.GetUpgradeSprite((MinionType)minionType);
        SetUpgradeRewardAvailable();
    }

    void SetUpgradeRewardAvailable()
    {

        if ((unlockedMeleeMinions[0] && MinionSpawner.Instance.upgradeLevel[0] < 2)
         || (unlockedMeleeMinions[1] && MinionSpawner.Instance.upgradeLevel[1] < 2)
         || (unlockedMeleeMinions[2] && MinionSpawner.Instance.upgradeLevel[2] < 2))
        {
            excludeRewardsLeft.Remove(RewardType.Upgrade);
        }
        else if (!excludeRewardsLeft.Contains(RewardType.Upgrade))
        {
            excludeRewardsLeft.Add(RewardType.Upgrade);
        }

        if ((unlockedRangedMinions[0] && MinionSpawner.Instance.upgradeLevel[0] < 2)
         || (unlockedRangedMinions[1] && MinionSpawner.Instance.upgradeLevel[3] < 2)
         || (unlockedRangedMinions[2] && MinionSpawner.Instance.upgradeLevel[4] < 2))
        {
            excludeRewardsRight.Remove(RewardType.Upgrade);
        }
        else if (!excludeRewardsRight.Contains(RewardType.Upgrade))
        {
            excludeRewardsRight.Add(RewardType.Upgrade);
        }

    }

    void SetRewardButtonsInteractable(bool isInteractable)
    {
        foreach (Transform child in chooseRewardPanelLeft.transform.GetChild(2))
        {
            if (child.CompareTag("RewardButton"))
            {
                child.GetComponent<Button>().interactable = isInteractable;
                child.GetChild(0).GetComponent<Image>().color = isInteractable ? Color.white : Color.red;
            }
        }

        foreach (Transform child in chooseRewardPanelRight.transform.GetChild(2))
        {
            if (child.CompareTag("RewardButton"))
            {
                child.GetComponent<Button>().interactable = isInteractable;
                child.GetChild(0).GetComponent<Image>().color = isInteractable ? Color.white : Color.red;
            }
        }
    }

    public IEnumerator PickTimer()
    {
        while (resetTimer >= 0)
        {
            if (currentTimerBox != null)
            {
                string minutes = Mathf.Floor(resetTimer / 60).ToString("00");
                string seconds = Mathf.RoundToInt(resetTimer % 60).ToString("00");
                currentTimerBox.text = minutes + ":" + seconds;
            }
            if (resetTimer == 0)
            {
                QuestionSystem.Instance.resetQuestions();
                GameManager.Instance.minigameEnded = true;
                resetTimer = 2f;
                GameManager.Instance.timerBoxLeft.text = "";
                GameManager.Instance.timerBoxRight.text = "";
                break;
            }

            yield return new WaitForSeconds(1);
            resetTimer--;
        }
    }

    private float getBoost(float boost)
    {
        if (GameManager.Instance.isBonus)
        {
            return boost = 1.5f * boost;
        }
        return boost;
    }
}
