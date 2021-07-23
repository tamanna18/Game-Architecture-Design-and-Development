using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public MinionType minionType;
    [Header("Upgrade")]
    public float damageUpgrade;
    public float defenceUpgrade;
    public float healthUpgrade;
    [Header("Minion Stats boosts/multipliers")]
    public static float meleeDamageBoostMultiplier = 0f;
    public static int meleeDamageBoostCount = 0;
    public static float meleeDefenceBoostMultiplier = 0f;
    public static int meleeDefenceBoostCount = 0;
    public static float meleeHealthBoostMultiplier = 0f;
    public static int meleeHealthBoostCount = 0;
    public static float rangedDamageBoostMultiplier = 0f;
    public static int rangedDamageBoostCount = 0;
    public static float rangedDefenceBoostMultiplier = 0f;
    public static int rangedDefenceBoostCount = 0;
    public static float rangedHealthBoostMultiplier = 0f;
    public static int rangedHealthBoostCount = 0;
    [Header("Minion Stats")]
    public float maxHealth = 30;
    public float health = 30;
    public float defence = 0;
    public float attackDamage = 10;
    public float movementSpeed = 3;
    public float attackSpeed = 1f;
    public bool attackTarget = false;
    [Header("Ranged Minion Stats")]
    public float range = 13f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private GameObject castle;
    private GameObject[] fortifications;
    public GameObject currentTarget;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(movementSpeed, 0);
        castle = GameObject.FindGameObjectWithTag("Castle");
        fortifications = GameObject.FindGameObjectsWithTag("Fortification");
        if (isUpgraded())
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = MinionSpawner.Instance.GetUpgradeSprite(minionType);
            maxHealth += maxHealth * GetUpgradeLevel() * healthUpgrade;
            defence += defence * GetUpgradeLevel() * defenceUpgrade;
            attackDamage += attackDamage * GetUpgradeLevel() * damageUpgrade;
        }
        health = maxHealth;
        if (MinionSpawner.meleeMinions.Contains(minionType))
        {
            attackDamage *= Mathf.Pow((1 + GetMultiplier(meleeDamageBoostMultiplier)), meleeDamageBoostCount);
            defence *= Mathf.Pow((1 + GetMultiplier(meleeDefenceBoostMultiplier)), meleeDefenceBoostCount);
            health *= Mathf.Pow((1 + GetMultiplier(meleeHealthBoostMultiplier)), meleeHealthBoostCount);
        }
        if (MinionSpawner.rangedMinions.Contains(minionType))
        {
            attackDamage *= Mathf.Pow((1 + GetMultiplier(rangedDamageBoostMultiplier)), rangedDamageBoostCount);
            defence *= Mathf.Pow((1 + GetMultiplier(rangedDefenceBoostMultiplier)), rangedDefenceBoostCount);
            health *= Mathf.Pow((1 + GetMultiplier(rangedHealthBoostMultiplier)), rangedHealthBoostCount);
        }

        if (minionType == MinionType.ArcherGolem || minionType == MinionType.WizardGolem)
        {
            InvokeRepeating("UpdateTarget", 0f, 0.5f);
        }
    }

    void Update()
    {
        if (currentTarget == null || (minionType != MinionType.ArcherGolem && minionType != MinionType.WizardGolem)) return;
        if (fireCountdown <= 0f)
        {
            rb.velocity = new Vector2(0, 0);
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    void UpdateTarget()
    {
        float shortestDistance = Mathf.Infinity;
        GameObject nearestFortification = null;
        foreach (GameObject fortification in fortifications)
        {
            if (fortification != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, fortification.transform.position);
                if (distanceToEnemy < shortestDistance && !fortification.transform.GetComponent<FortificationScript>().isDestroyed())
                {
                    shortestDistance = distanceToEnemy;
                    nearestFortification = fortification;
                }
            }
        }
        if (nearestFortification != null && shortestDistance <= range + nearestFortification.transform.GetChild(2).GetComponent<SpriteRenderer>().bounds.extents.x)
        {
            currentTarget = nearestFortification.GetComponent<FortificationScript>().GetNearestTower(gameObject);
        }
        else if (castle != null && Vector3.Distance(transform.position, castle.transform.position) < range + castle.transform.GetComponent<SpriteRenderer>().bounds.extents.x)
        {
            currentTarget = castle;
        }
        else
        {
            currentTarget = null;
            rb.velocity = new Vector2(movementSpeed, 0);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }


    void Shoot()
    {
        GameObject projectileObject = (GameObject)Instantiate(projectilePrefab, firePoint.position, projectilePrefab.transform.rotation);
        ProjectileScript projectileScript = projectileObject.GetComponent<ProjectileScript>();
        if (minionType == MinionType.WizardGolem && isUpgraded())
        {
            projectileObject.GetComponent<SpriteRenderer>().sprite = projectileScript.GetFireAttackSprite(GetUpgradeLevel());
        }
        if (projectileScript != null)
        {
            projectileScript.Seek(currentTarget, attackDamage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Castle" || other.gameObject.tag == "Fortification")
        {
            currentTarget = other.gameObject;
            rb.velocity = new Vector2(0, 0);
            attackTarget = true;
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        while (attackTarget && currentTarget != null)
        {
            if (currentTarget.tag == "Castle")
            {
                attackTarget = CastleScript.Instance.takeDamage(attackDamage);
            }
            else if (currentTarget.tag == "Fortification")
            {
                attackTarget = currentTarget.GetComponent<FortificationScript>().takeDamage(attackDamage);
            }
            yield return new WaitForSeconds(attackSpeed);
        }
        rb.velocity = new Vector2(movementSpeed, 0);
    }

    public void takeDamage(float damage)
    {
        health -= damage - defence;
        if (isKilled())
        {
            attackTarget = false;
            gameObject.tag = "Untagged";
            Destroy(gameObject, 0.5f);
        }
    }

    public bool isKilled()
    {
        return health <= 0;
    }

    public void BoostDamage(float damageBoost, GameIdentifier gameIdentifier)
    {
        if (gameIdentifier == GameIdentifier.MiniGameLeft && MinionSpawner.meleeMinions.Contains(minionType))
        {
            attackDamage += attackDamage * GetMultiplier(damageBoost);
        }
        else if (gameIdentifier == GameIdentifier.MiniGameRight && MinionSpawner.rangedMinions.Contains(minionType))
        {
            attackDamage += attackDamage * GetMultiplier(damageBoost);
        }
    }

    public static void IncreaseDamageBoostMultiplier(float damageBoost, GameIdentifier gameIdentifier)
    {
        if (gameIdentifier == GameIdentifier.MiniGameLeft)
        {
            meleeDamageBoostMultiplier = damageBoost;
            meleeDamageBoostCount++;
        }
        else if (gameIdentifier == GameIdentifier.MiniGameRight)
        {
            rangedDamageBoostMultiplier = damageBoost;
            rangedDamageBoostCount++;
        }
    }

    public void BoostDefence(float defenceBoost, GameIdentifier gameIdentifier)
    {
        if (gameIdentifier == GameIdentifier.MiniGameLeft && MinionSpawner.meleeMinions.Contains(minionType))
        {
            defence += defence * GetMultiplier(defenceBoost);
        }
        else if (gameIdentifier == GameIdentifier.MiniGameRight && MinionSpawner.rangedMinions.Contains(minionType))
        {
            defence += defence * GetMultiplier(defenceBoost);
        }

    }

    public static void IncreaseDefenceBoostMultiplier(float defenceBoost, GameIdentifier gameIdentifier)
    {
        if (gameIdentifier == GameIdentifier.MiniGameLeft)
        {
            meleeDefenceBoostMultiplier = defenceBoost;
            meleeDefenceBoostCount++;
        }
        else if (gameIdentifier == GameIdentifier.MiniGameRight)
        {
            rangedDefenceBoostMultiplier = defenceBoost;
            rangedDefenceBoostCount++;
        }
    }

    public static void IncreaseHealthBoostMultiplier(float healthBoost, GameIdentifier gameIdentifier)
    {
        if (gameIdentifier == GameIdentifier.MiniGameLeft)
        {
            meleeHealthBoostMultiplier = healthBoost;
            meleeHealthBoostCount++;
        }
        else if (gameIdentifier == GameIdentifier.MiniGameRight)
        {
            rangedHealthBoostMultiplier = healthBoost;
            rangedHealthBoostCount++;
        }
    }

    public void Heal(float heal, GameIdentifier gameIdentifier)
    {
        if (gameIdentifier == GameIdentifier.MiniGameLeft && MinionSpawner.meleeMinions.Contains(minionType))
        {
            if (health + heal > maxHealth)
            {
                health = maxHealth;
            }
            else
            {
                health += heal;
            }
        }
        else if (gameIdentifier == GameIdentifier.MiniGameRight && MinionSpawner.rangedMinions.Contains(minionType))
        {
            if (health + heal > maxHealth)
            {
                health = maxHealth;
            }
            else
            {
                health += heal;
            }
        }

    }

    float GetMultiplier(float multiplier)
    {
        return (minionType == MinionType.GreenGolem ? multiplier * 0.5f : multiplier);
    }

    bool isUpgraded()
    {
        return MinionSpawner.Instance.isUpgraded[(int)minionType];
    }

    int GetUpgradeLevel()
    {
        return MinionSpawner.Instance.upgradeLevel[(int)minionType];
    }

    public float GetUpgradedHealth()
    {
        return maxHealth * (GetUpgradeLevel() + 1) * healthUpgrade;
    }

    public float GetUpgradedAttack()
    {
        return attackDamage * (GetUpgradeLevel() + 1) * damageUpgrade;
    }

    public float GetUpgradedDefence()
    {
        return defence * (GetUpgradeLevel() + 1) * defenceUpgrade;
    }



}
