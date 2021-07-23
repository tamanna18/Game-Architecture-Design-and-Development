using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleScript : MonoBehaviour
{
    static CastleScript instance;
    public static CastleScript Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<CastleScript>();
            }
            return instance;
        }
    }
    public float maxHealth = 200;
    public float health = 200;
    public HealthBarScript healthBar;
    public float attackDamage = 25f;
    public float range = 15f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private GameObject target;
    public Sprite[] castleDamagedSprites;
    private bool isCracked;
    private bool destroyed;

    void Start()
    {
        health = maxHealth;
        healthBar.SetMaxHealth(health);
        InvokeRepeating("UpdateTarget", 0f, 0.3f);
    }

    void UpdateTarget()
    {
        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestMinion = null;
        foreach (GameObject minion in minions)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, minion.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestMinion = minion;
            }
        }

        if (nearestMinion != null && shortestDistance <= range)
        {
            target = nearestMinion;
        }
        else
        {
            target = null;
        }
    }

    void Update()
    {

        if (target == null) return;

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void Shoot()
    {
        GameObject projectileObject = (GameObject)Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        ProjectileScript projectileScript = projectileObject.GetComponent<ProjectileScript>();

        if (projectileScript != null)
        {
            projectileScript.Seek(target, attackDamage);
        }
    }

    public bool takeDamage(float damage)
    {
        if (health <= 0) return false;
        health -= damage;
        healthBar.SetHealth(health, true);

        if (health <= maxHealth / 2 && health > 0 && !isCracked)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = castleDamagedSprites[0];
            isCracked = true;
        }

        if (health <= 0 && !destroyed)
        {
            destroyed = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = castleDamagedSprites[1];
            gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, 1f, gameObject.transform.localScale.z);
            healthBar.SetHealth(0, true);
            Destroy(gameObject, 1f);
            FindObjectOfType<GameManager>().EndGame(true);
            return false;
        }
        return true;
    }

    public void Heal(float heal)
    {
        if (health + heal > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += heal;
        }
        healthBar.SetHealth(health, true);

    }
}
