using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour
{

    public float attackDamage = 5f;
    public float range = 5f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private GameObject target;

    void Start()
    {
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

    void Shoot()
    {
        GameObject projectileObject = (GameObject)Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        ProjectileScript projectileScript = projectileObject.GetComponent<ProjectileScript>();

        if (projectileScript != null)
        {
            projectileScript.Seek(target, attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void takeDamage(float damage)
    {
        transform.parent.GetComponent<FortificationScript>().takeDamage(damage);
    }


}
