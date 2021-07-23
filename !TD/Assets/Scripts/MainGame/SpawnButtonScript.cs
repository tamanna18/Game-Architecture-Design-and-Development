using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnButtonScript : MonoBehaviour
{
    public float cooldownTimer = 5;
    private TextMeshProUGUI timerBox;
    void Start()
    {
        timerBox = GetComponentInChildren<TextMeshProUGUI>();
        timerBox.enabled = false;
    }

    public void SpawnMinionWave(int minionType)
    {

        GetComponent<Button>().interactable = false;
        FindObjectOfType<MinionSpawner>().SpawnMinionWave((MinionType)minionType);
        gameObject.GetComponent<Image>().color = Color.red;
        StartCoroutine(CooldownTimer());
    }

    IEnumerator CooldownTimer()
    {
        float tempCooldownTimer = cooldownTimer;
        timerBox.enabled = true;
        while (tempCooldownTimer >= 0)
        {
            timerBox.text = tempCooldownTimer.ToString();
            yield return new WaitForSeconds(1);
            tempCooldownTimer--;
        }
        ResetButton();
    }

    void ResetButton()
    {
        timerBox.enabled = false;
        GetComponent<Button>().interactable = true;
        timerBox.text = cooldownTimer.ToString();
        gameObject.GetComponent<Image>().color = Color.white;
    }
}
