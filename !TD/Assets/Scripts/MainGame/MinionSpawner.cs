using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MinionType { GreenGolem, UndeadGolem, WoodGolem, ArcherGolem, WizardGolem };

public class MinionSpawner : MonoBehaviour
{
    static MinionSpawner instance;
    public static MinionSpawner Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<MinionSpawner>();
            }
            return instance;
        }
    }
    public Camera gameCamera;
    public GameObject minionPrefab;
    public int greenGolemWaveSize = 3;
    public int undeadGolemWaveSize = 3;
    public int woodGolemWaveSize = 3;
    public int archerGolemWaveSize = 3;
    public int wizardGolemWaveSize = 3;
    public static List<MinionType> meleeMinions = new List<MinionType> { MinionType.GreenGolem, MinionType.UndeadGolem, MinionType.WoodGolem };
    public static List<MinionType> rangedMinions = new List<MinionType> { MinionType.GreenGolem, MinionType.ArcherGolem, MinionType.WizardGolem };
    [Header("Minion Prefabs")]
    public GameObject greenGolemPrefab;
    public GameObject undeadGolemPrefab;
    public GameObject woodGolemPrefab;
    public GameObject archerGolemPrefab;
    public GameObject wizardGolem;
    [Header("Minion Upgrades")]
    public bool[] isUpgraded;
    public int[] upgradeLevel;
    public Sprite[] upgradeSpritesGreenGolem;
    public Sprite[] upgradeSpritesUndeadGolem;
    public Sprite[] upgradeSpritesWoodGolem;
    public Sprite[] upgradeSpritesArcherGolem;
    public Sprite[] upgradeSpritesWizardGolem;

    IEnumerator MinionWave(MinionType minionType)
    {
        for (int i = 0; i < GetMinionWaveSize(minionType); i++)
        {
            SpawnMinion(minionType);
            yield return new WaitForSeconds(0.3f);
        }

    }

    //minion wave size as param?
    public void SpawnMinionWave(MinionType minionType = MinionType.WoodGolem)
    {
        StartCoroutine(MinionWave(minionType));
    }

    void SpawnMinion(MinionType minionType = MinionType.GreenGolem)
    {
        //Spawn Minion
        GameObject newMinion;
        switch (minionType)
        {
            case MinionType.GreenGolem:
                newMinion = Instantiate(greenGolemPrefab);
                break;
            case MinionType.UndeadGolem:
                newMinion = Instantiate(undeadGolemPrefab);
                break;
            case MinionType.WoodGolem:
                newMinion = Instantiate(woodGolemPrefab);
                break;
            case MinionType.ArcherGolem:
                newMinion = Instantiate(archerGolemPrefab);
                break;
            case MinionType.WizardGolem:
                newMinion = Instantiate(wizardGolem);
                break;
            default:
                newMinion = Instantiate(greenGolemPrefab);
                break;
        }
        newMinion.transform.position = calculatePosition();
    }

    Vector3 calculatePosition()
    {
        float spawnY = Random.Range(0.35f, 0.66f);
        Vector3 spawnPosition = gameCamera.ViewportToWorldPoint(new Vector3(0f, spawnY, 0));
        spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, 0);
        return spawnPosition;
    }


    int GetMinionWaveSize(MinionType minionType)
    {
        switch (minionType)
        {
            case MinionType.GreenGolem:
                return greenGolemWaveSize;
            case MinionType.UndeadGolem:
                return undeadGolemWaveSize;
            case MinionType.WoodGolem:
                return woodGolemWaveSize;
            case MinionType.ArcherGolem:
                return archerGolemWaveSize;
            case MinionType.WizardGolem:
                return wizardGolemWaveSize;
            default:
                return 3;
        }
    }

    public int GetUpgradeLevel(MinionType minionType)
    {
        return upgradeLevel[(int)minionType];
    }

    public Sprite GetUpgradeSprite(MinionType minionType)
    {
        switch (minionType)
        {
            case MinionType.GreenGolem:
                return upgradeSpritesGreenGolem[GetUpgradeLevel(minionType)];
            case MinionType.UndeadGolem:
                return upgradeSpritesUndeadGolem[GetUpgradeLevel(minionType)];
            case MinionType.WoodGolem:
                return upgradeSpritesWoodGolem[GetUpgradeLevel(minionType)];
            case MinionType.ArcherGolem:
                return upgradeSpritesArcherGolem[GetUpgradeLevel(minionType)];
            case MinionType.WizardGolem:
                return upgradeSpritesWizardGolem[GetUpgradeLevel(minionType)];
            default:
                return upgradeSpritesGreenGolem[GetUpgradeLevel(minionType)];
        }
    }

    public MinionScript GetMinionPrefabScript(MinionType minionType)
    {
        switch (minionType)
        {
            case MinionType.GreenGolem:
                return greenGolemPrefab.GetComponent<MinionScript>();
            case MinionType.UndeadGolem:
                return undeadGolemPrefab.GetComponent<MinionScript>();
            case MinionType.WoodGolem:
                return woodGolemPrefab.GetComponent<MinionScript>();
            case MinionType.ArcherGolem:
                return archerGolemPrefab.GetComponent<MinionScript>();
            case MinionType.WizardGolem:
                return wizardGolem.GetComponent<MinionScript>();
            default:
                return greenGolemPrefab.GetComponent<MinionScript>();
        }
    }
}
