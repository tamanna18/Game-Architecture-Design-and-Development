using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private GameObject target;
    public float speed = 70f;
    public float damage = 15f;
    public Sprite[] fireAttackSprites;
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = target.transform.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;
        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }

    public void Seek(GameObject target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }

    void HitTarget()
    {
        if (target.tag == "Castle")
        {
            CastleScript.Instance.takeDamage(damage);
        }
        else if (target.tag == "Fortification")
        {
            target.GetComponent<FortificationScript>().takeDamage(damage);
        }
        else if (target.tag == "Tower")
        {
            target.GetComponent<TowerScript>().takeDamage(damage);
        }
        else
        {
            target.GetComponent<MinionScript>().takeDamage(damage);
        }
        Destroy(gameObject);
    }

    public Sprite GetFireAttackSprite(int index)
    {
        return fireAttackSprites[index];
    }

}
