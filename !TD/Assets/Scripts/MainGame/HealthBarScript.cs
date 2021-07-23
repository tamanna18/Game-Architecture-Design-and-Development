using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarScript : MonoBehaviour
{
    public Image image;
    private float maxHealth;
    private bool showText;

    public void SetHealth(float health, bool showText)
    {
        this.showText = showText;
        if (showText) SetHealthText((health / maxHealth) * 100);
        image.fillAmount = health / maxHealth;
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        image.fillAmount = 1f;
        if (showText) SetHealthText(100);
    }

    void SetHealthText(float health)
    {
        this.GetComponentInChildren<TextMeshProUGUI>().SetText("{0}%", health);
    }
}
