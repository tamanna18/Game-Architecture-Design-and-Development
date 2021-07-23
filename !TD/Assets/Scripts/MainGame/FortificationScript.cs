using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortificationScript : MonoBehaviour
{
    public float maxHealth = 500f;
    public float health = 500f;
    private HealthBarScript healthBar;
    private GameObject upperTower;
    private GameObject bottomTower;
    private GameObject gate;
    public Sprite[] gateDamagedSprites;
    public Sprite[] towerDamagedSprites;
    private bool isCracked;
    private bool destroyed;
    void Start()
    {
        healthBar = transform.GetChild(3).GetChild(0).transform.GetComponent<HealthBarScript>();
        healthBar.SetMaxHealth(health);
        upperTower = transform.GetChild(0).gameObject;
        bottomTower = transform.GetChild(1).gameObject;
        gate = transform.GetChild(2).gameObject;
        health = maxHealth;
    }

    public bool takeDamage(float damage)
    {
        if (health <= 0) return false;
        health -= damage;
        healthBar.SetHealth(health, false);
        if (health <= maxHealth / 2 && health > 0 && !isCracked)
        {
            gate.GetComponent<SpriteRenderer>().sprite = gateDamagedSprites[0];
            upperTower.GetComponent<SpriteRenderer>().sprite = towerDamagedSprites[0];
            upperTower.transform.localScale = new Vector3(upperTower.transform.localScale.x, 0.47f, upperTower.transform.localScale.z);
            bottomTower.GetComponent<SpriteRenderer>().sprite = towerDamagedSprites[0];
            bottomTower.transform.localScale = new Vector3(bottomTower.transform.localScale.x, 0.589f, bottomTower.transform.localScale.z);
            isCracked = true;
        }

        if (health <= 0 && !destroyed)
        {
            destroyed = true;
            gate.GetComponent<SpriteRenderer>().sprite = gateDamagedSprites[1];
            gate.transform.localScale = new Vector3(gate.transform.localScale.x, 0.99f, gate.transform.localScale.z);
            upperTower.GetComponent<SpriteRenderer>().sprite = towerDamagedSprites[1];
            upperTower.transform.localScale = new Vector3(upperTower.transform.localScale.x, 0.75f, upperTower.transform.localScale.z);
            bottomTower.GetComponent<SpriteRenderer>().sprite = towerDamagedSprites[1];
            bottomTower.transform.localScale = new Vector3(bottomTower.transform.localScale.x, 0.97f, bottomTower.transform.localScale.z);
            healthBar.SetHealth(0, false);
            Destroy(gameObject, 0.5f);
            return false;
        }
        return true;
    }
    public bool isDestroyed()
    {
        return destroyed;
    }

    public GameObject GetNearestTower(GameObject minion)
    {
        float distanceToUpperTower = Vector2.Distance(minion.transform.position, upperTower.transform.position);
        float distanceToBottomTower = Vector2.Distance(minion.transform.position, bottomTower.transform.position);
        if (distanceToUpperTower < distanceToBottomTower)
        {
            return upperTower;
        }
        else
        {
            return bottomTower;
        }
    }

    public void Heal(float heal)
    {
        if (health + heal > maxHealth)
        {
            health = maxHealth;
            CastleScript.Instance.attackDamage += 1;
        }
        else
        {
            health += heal;
        }
        healthBar.SetHealth(health, false);
    }
}
